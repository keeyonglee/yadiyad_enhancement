using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Web.Infrastructure;
using Nop.Services.Media;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Enums;
using YadiYad.Pro.Web.Filters;
using Nop.Services.Security;
using Nop.Services.Directory;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{

    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class IndividualController : BaseController
    {
        #region Fields
        public const string INDIVIDUAL_PROFILE_PICTURE = "{0}/INDIVIDUAL_PROFILE/PICTURE/";
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly IndividualProfileService _individualProfileService;
        private readonly IndividualInterestHobbyService _individualInterestHobbyService;
        private readonly IPictureService _pictureService;
        private readonly AccountContext _accountContext;
        private readonly ICountryService _countryService;
        private readonly ProIndividualTourSettings _proIndividualTourSettings;

        #endregion

        #region Ctor

        public IndividualController(
            IMapper mapper,
            IWorkContext workContext,
            IndividualProfileService individualProfileService,
            IndividualInterestHobbyService individualInterestHobbyService,
            IPictureService pictureService,
            ICountryService countryService,
            AccountContext accountContext,
            ProIndividualTourSettings proIndividualTourSettings)
        {
            _mapper = mapper;
            _workContext = workContext;
            _individualProfileService = individualProfileService;
            _individualInterestHobbyService = individualInterestHobbyService;
            _pictureService = pictureService;
            _accountContext = accountContext;
            _countryService = countryService;
            _proIndividualTourSettings = proIndividualTourSettings;

        }

        #endregion

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        [HttpGet()]
        public virtual IActionResult GetIndividualProfile()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var result = _individualProfileService.GetIndividualProfileByCustomerId(customerId);
            if (result != null
                && result.PictureId.HasValue)
            {
                var picture = _pictureService.GetPictureById(result.PictureId.Value)
                              ?? throw new Exception("Picture cannot be loaded");

                result.ProfileImage = _pictureService.GetPictureUrl(ref picture);
            }

            if (result == null)
            {
                result = new IndividualProfileDTO();
                result.Email = _workContext.CurrentCustomer.Email;
                var country = _countryService.GetCountryByTwoLetterIsoCode("MY");

                result.CountryId = country.Id;
                result.CountryName = country.Name;
            }
            result.SetDelay = _proIndividualTourSettings.SetDelay;

            response.SetResponse(result);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        [HttpPost()]
        public IActionResult SubmitIndividualProfile([FromBody] IndividualProfileDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (dto.Id == 0)
            {
                var existingProfile = _individualProfileService.GetIndividualProfileByCustomerId(_workContext.CurrentCustomer.Id);
                if(existingProfile != null)
                {
                    throw new Exception("Individual profile already exist");
                }

                dto.CustomerId = _workContext.CurrentCustomer.Id;
                dto.CreatedById = _workContext.CurrentCustomer.Id;
                dto.CreatedOnUTC = DateTime.UtcNow;
                _individualProfileService.CreateIndividualProfile(dto);
            }
            else
            {
                var existing = _individualProfileService.GetIndividualProfileById(dto.Id);
                dto.CreatedById = existing.CreatedById;
                dto.CreatedOnUTC = existing.CreatedOnUTC;
                dto.CustomerId = existing.CustomerId;
                dto.IsOnline = existing.IsOnline;
                dto.SSTRegNo = existing.SSTRegNo;
                dto.IsTourCompleted = existing.IsTourCompleted;
                _individualProfileService.UpdateIndividualProfile(_workContext.CurrentCustomer.Id, dto);
            }

            //if (dto.File != null)
            //{
            //    string filePath = AmazonS3Service.ConstructDirectory(INDIVIDUAL_PROFILE_PICTURE, "images");
            //    var result = _amazonS3Service.UploadObject(new UploadFileRequest()
            //    {
            //        FileName = dto.Picture.FileName, // optional
            //        FolderPath = filePath,
            //        File = dto.Picture.File,
            //        //DeleteFileUrl = currentAttachment.Url // upload and delete exisiting file (Replace)
            //    }).Result;

            //    var attachment = _pictureProService.CreatePicture(result);
            //}

            _accountContext.ClearAccountSession();

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        [HttpPost("updateSSTRegNo")]
        public IActionResult UpdateIndividualSSTRegNo([FromBody] IndividualProfileUpdateSSTRegNoDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            var existing = _individualProfileService.GetIndividualProfileByCustomerId(_workContext.CurrentCustomer.Id);
            existing.SSTRegNo = dto.SSTRegNo;

            existing.UpdatedById = _workContext.CurrentCustomer.Id;
            existing.UpdatedOnUTC = DateTime.UtcNow;
            _individualProfileService.UpdateIndividualSSTRegNo(existing);

            _accountContext.ClearAccountSession();

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        [HttpGet("submitTour")]
        public virtual IActionResult SubmitIndividualTour ()
        {
            var response = new ResponseDTO();

            var existing = _individualProfileService.GetIndividualProfileByCustomerId(_workContext.CurrentCustomer.Id);
            existing.IsTourCompleted = true;

            existing.UpdatedById = _workContext.CurrentCustomer.Id;
            existing.UpdatedOnUTC = DateTime.UtcNow;
            _individualProfileService.UpdateIndividualSSTRegNo(existing);

            _accountContext.ClearAccountSession();
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

    }
}
