using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class ConsultationController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly ConsultationProfileService _consultationProfileService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly ConsultationInvitationService _consultationInvitationService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly IndividualProfileService _individualProfileService;

        #endregion

        #region Ctor

        public ConsultationController(
            IWorkContext workContext,
            AccountContext accountContext,
            ConsultationProfileService consultationProfileService,
            ConsultationInvitationService consultationInvitationService,
            ServiceProfileService serviceProfileService,
            ProWorkflowMessageService proWorkflowMessageService,
            IndividualProfileService individualProfileService)
        {
            _workContext = workContext;
            _accountContext = accountContext;
            _consultationProfileService = consultationProfileService;
            _consultationInvitationService = consultationInvitationService;
            _serviceProfileService = serviceProfileService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _individualProfileService = individualProfileService;
        }

        #endregion

        #region Methods

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpPost("list")]
        public IActionResult GetConsultationProfilesByOrganizationProfileId([FromBody] ListFilterDTO<ConsultationListSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.OrganizationProfileId = _accountContext.CurrentAccount.OrganizationProfileId.Value;
            var data = _consultationProfileService.GetConsultationProfilesByOrganizationProfileId(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);
            var dto = new PagedListDTO<ConsultationProfileDTO>(data);

            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("approval/list")]
        public IActionResult GetConsultationProfileApproval([FromBody] ListFilterDTO<ConsultationAdvsReviewSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            var data = _consultationProfileService.GetConsultationProfilesChecking(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);
            var dto = new PagedListDTO<ConsultationProfileDTO>(data);

            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpGet("{id}")]
        public virtual IActionResult GetConsultationProfileById(int id)
        {
            var response = new ResponseDTO();
            var data = _consultationProfileService.GetConsultationProfileById(id);

            if (data == null || data.DeletedByUser)
            {
                return NotFound();
            }

            response.SetResponse(data);
            return Ok(response);
        }

        [HttpPost("{id}")]
        public IActionResult SubmitConsultationProfile(int id, [FromBody] ConsultationProfileDTO dto)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                dto.OrganizationProfileId = _accountContext.CurrentAccount.OrganizationProfileId.Value;
                _consultationProfileService.CreateConsultationProfile(_workContext.CurrentCustomer.Id, dto);
            }
            else
            {
                _consultationProfileService.UpdateConsultationProfile(_workContext.CurrentCustomer.Id, dto);
            }
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpDelete("{id}")]
        public virtual IActionResult DeleteConsultationProfileById(int id)
        {
            var response = new ResponseDTO();
            var ciToUpdate = _consultationProfileService.DeleteConsultationProfile(_workContext.CurrentCustomer.Id, id);

            // send email
            ciToUpdate.ForEach(x =>
            {
                _proWorkflowMessageService.SendConsultationDeclinedByOrganization(_workContext.WorkingLanguage.Id, x);
            });

            return Ok(response);
        }

        [HttpPost("{id}/candidates")]
        public IActionResult SearchCandidates(int id, [FromBody] ListFilterDTO<ServiceProfileSearchFilterDTO> query)
        {
            query.AdvancedFilter.ConsultationProfileId = id;
            query.AdvancedFilter.ServiceTypeId = (int)ServiceType.Consultation;

            var response = new ResponseDTO();

            var dto = _serviceProfileService.SearchServiceProfiles(
                query.Offset,
                query.RecordSize,
                query.Filter,
                query.AdvancedFilter,
                id);

            response.SetResponse(dto);

            return Ok(response);
        }

        [HttpPost("{id}/invite")]
        public IActionResult InviteCandidates(int id, [FromBody] List<int> serviceProfileIds)
        {
            var response = new ResponseDTO();

            var dto = new ConsultationInvitationDTO
            {
                ConsultationProfileId = id,
                ServiceProfileIds = serviceProfileIds
            };

            _consultationInvitationService.CreateConsultationInvitation(_workContext.CurrentCustomer.Id, dto);
            var consultationProfileDto = _consultationProfileService.GetConsultationProfileById(id);
            if (consultationProfileDto.IsApproved == true)
            {
                foreach (var spId in serviceProfileIds)
                {
                    var customerId = _serviceProfileService.GetCustomerId(spId);
                    var indProfileDTO = _individualProfileService.GetIndividualProfileByCustomerId(customerId);
                    _proWorkflowMessageService.SendConsultationCandidateInvited(_workContext.WorkingLanguage.Id, consultationProfileDto, indProfileDTO);

                }
            }
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("approval/{id}")]
        public IActionResult ApprovalProfile(int id, [FromBody] ConsultationProfileDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                response.SetResponse(ResponseStatusCode.Warning);

            }
            else
            {
                _consultationProfileService.UpdateConsultationProfileApproval(id, dto);
                if (dto.IsApproved == true)
                {
                    var customerIdList = _consultationInvitationService.GetAllIndividualIdFromConsultationProfile(id);
                    var consultationProfileDTO = _consultationProfileService.GetConsultationProfileById(id);
                    response.SetResponse(true);
                    foreach (var customerId in customerIdList)
                    {
                        var indProfileDTO = _individualProfileService.GetIndividualProfileByCustomerId(customerId);
                        _proWorkflowMessageService.SendConsultationCandidateInvited(_workContext.WorkingLanguage.Id, consultationProfileDTO, indProfileDTO);
                    }
                }
                else
                {
                    response.SetResponse(false);
                }
            }
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [HttpPost("complete/{id}")]
        public IActionResult CompleteProfile(int id, [FromBody] ConsultationProfileDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                response.SetResponse(ResponseStatusCode.Warning);

            }
            else
            {
                _consultationProfileService.UpdateConsultationProfileApproval(id, dto);
            }
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        #endregion

    }
}
