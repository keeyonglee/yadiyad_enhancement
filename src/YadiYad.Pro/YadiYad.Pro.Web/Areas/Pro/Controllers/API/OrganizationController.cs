using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Services.DTO.Organization;
using Nop.Services.Media;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Filters;
using Nop.Services.Security;
using Nop.Services.Directory;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class OrganizationController : BaseController
    {
        #region Fields

        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly ICountryService _countryService;

        #endregion

        #region Ctor

        public OrganizationController(
            IWorkContext workContext,
            AccountContext accountContext,
            IPictureService pictureService,
            ICountryService countryService,
            OrganizationProfileService organizationProfileService)
        {
            _workContext = workContext;
            _accountContext = accountContext;
            _pictureService = pictureService;
            _organizationProfileService = organizationProfileService;
            _countryService = countryService;
        }

        #endregion

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationProfile))]
        [HttpGet()]
        public virtual IActionResult GetOrganizationProfile()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var result = _organizationProfileService.GetOrganizationProfileByCustomerId(customerId);

            if (result != null 
                && result.PictureId.HasValue)
            {
                var picture = _pictureService.GetPictureById(result.PictureId.Value)
                              ?? throw new Exception("Picture cannot be loaded");

                result.LogoImage = _pictureService.GetPictureUrl(ref picture);
            }

            if (result == null)
            {
                result = new OrganizationProfileDTO();
                result.ContactPersonEmail = _workContext.CurrentCustomer.Email;
                var country = _countryService.GetCountryByTwoLetterIsoCode("MY");

                result.CountryId = country.Id;
                result.CountryName = country.Name;
            }

            response.SetResponse(result);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationProfile))]
        [HttpPost()]
        public IActionResult SubmitOrganizationProfile([FromBody]OrganizationProfileDTO dto)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
                return Ok(response);
            }

            if (dto.Id == 0)
            {
                var existingProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(_workContext.CurrentCustomer.Id);
                if (existingProfile != null)
                {
                    throw new Exception("Organization profile already exist");
                }

                dto.CustomerId = _workContext.CurrentCustomer.Id;
                dto.CreatedById = _workContext.CurrentCustomer.Id;
                dto.CreatedOnUTC = DateTime.UtcNow;
                _organizationProfileService.CreateOrganizationProfile(dto);
            }
            else
            {
                var existing = _organizationProfileService.GetOrganizationProfileById(dto.Id);
                dto.CustomerId = existing.CustomerId;
                dto.CreatedById = existing.CreatedById;
                dto.CreatedOnUTC = existing.CreatedOnUTC;

                dto.UpdatedById = _workContext.CurrentCustomer.Id;
                dto.UpdatedOnUTC = DateTime.UtcNow;
                _organizationProfileService.UpdateOrganizationProfile(dto);
            }

            _accountContext.ClearAccountSession();

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

    }
}
