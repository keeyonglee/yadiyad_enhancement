using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class ServiceController : BaseController
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly ConsultationInvitationService _consultationInvitationService;
        #endregion

        #region Ctor

        public ServiceController(
            IMapper mapper,
            IWorkContext workContext,
            ServiceProfileService serviceProfileService,
            ConsultationInvitationService consultationInvitationService,
            ServiceApplicationService serviceApplicationService)
        {
            _mapper = mapper;
            _workContext = workContext;
            _serviceProfileService = serviceProfileService;
            _serviceApplicationService = serviceApplicationService;
            _consultationInvitationService = consultationInvitationService;
        }

        #endregion

        //[AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpGet("{id}")]
        public virtual IActionResult Get(int id)
        {
            var response = new ResponseDTO();
            var dto = _serviceProfileService.GetServiceProfileById(id);
            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("{id}")]
        public virtual IActionResult Update([FromBody] ServiceProfileDTO dto)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;

            var updateServiceProfile = _serviceProfileService.UpdateServiceProfile(customerId, customerId, dto);

            var updateDTO = _serviceProfileService.GetServiceProfileById(updateServiceProfile.Id);

            response.SetResponse(updateDTO);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpDelete("{id}")]
        public virtual IActionResult Delete([FromRoute]int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;

            _serviceProfileService.DeleteServiceProfile(customerId, customerId, id);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost()]
        public virtual IActionResult Insert([FromBody] ServiceProfileDTO dto)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var updateServiceProfile = _serviceProfileService.CreateServiceProfile(customerId, customerId, dto);

            var updateDTO = _serviceProfileService.GetServiceProfileById(updateServiceProfile.Id);

            response.SetResponse(updateDTO);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("user")]
        public virtual IActionResult GetUserService([FromBody] ListFilterDTO<ServiceProfileSearchFilterDTO> searchDTO)
        {
            searchDTO.AdvancedFilter.CustomerId = _workContext.CurrentCustomer.Id;
            var response = new ResponseDTO();

            var dto = _serviceProfileService.SearchServiceProfiles(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("userservice")]
        public virtual IActionResult GetUserIndividualService([FromBody] ListFilterDTO<ServiceProfileSearchFilterDTO> searchDTO)
        {
            searchDTO.AdvancedFilter.CustomerId = _workContext.CurrentCustomer.Id;
            var response = new ResponseDTO();

            var dto = _serviceProfileService.SearchServiceProfiles(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("search")]
        public IActionResult SearchService([FromBody] ListFilterDTO<ServiceProfileSearchFilterDTO> searchDTO)
        {
            searchDTO.RecordSize = 10;
            searchDTO.AdvancedFilter.ExcludeServiceTypeIds = new List<int> { (int)ServiceType.ProjectBased };
            searchDTO.AdvancedFilter.BuyerCustomerId = _workContext.CurrentCustomer.Id;
            var response = new ResponseDTO();

            var dto = _serviceProfileService.SearchServiceProfiles(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter,
                sortBy: searchDTO.SortBy);

            response.SetResponse(dto);

            return Ok(response);
        }

        [HttpPost("{id}/consultation/reviewed")]
        public IActionResult SearchService(int id, [FromBody] ListFilterDTO<ConsultationInvitationListingFilterDTO> searchDTO)
        {
            searchDTO.RecordSize = 10;

            var response = new ResponseDTO();

            var dto = _consultationInvitationService.GetServiceProfilePastConsultationInvitations(
                id,
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize);

            foreach (var data in dto.Data)
            {
                if (!string.IsNullOrWhiteSpace(data.OrganizationName))
                {
                    data.OrganizationName = data.OrganizationName.ToMask();
                }
            }
            response.SetResponse(dto);

            return Ok(response);
        }
    }
}
