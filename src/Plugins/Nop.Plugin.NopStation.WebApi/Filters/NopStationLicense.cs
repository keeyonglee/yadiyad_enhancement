using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.NopStation.WebApi.Filters
{
    public partial class NopStationApiLicense : TypeFilterAttribute
    {
        #region Ctor

        public NopStationApiLicense() : base(typeof(NopStationApiLicenseFilter))
        {
        }

        #endregion

        #region Nested filter

        private class NopStationApiLicenseFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly INopStationLicenseService _nopStationLicenseService;

            #endregion

            #region Ctor

            public NopStationApiLicenseFilter(INopStationLicenseService nopStationLicenseService)
            {
                _nopStationLicenseService = nopStationLicenseService;
            }

            #endregion

            #region Methods

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                if(!_nopStationLicenseService.IsLicensed())
                    CreateNstAccessResponceMessage(filterContext);
            }

            private void CreateNstAccessResponceMessage(AuthorizationFilterContext filterContext)
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var response = new BaseResponseModel
                {
                    ErrorList = new List<string>
                    {
                        localizationService.GetResource("NopStation.WebApi.Response.InvalidLicense")
                    }
                };

                filterContext.Result = new BadRequestObjectResult(response);
                return;
            }

            #endregion
        }

        #endregion
    }
}