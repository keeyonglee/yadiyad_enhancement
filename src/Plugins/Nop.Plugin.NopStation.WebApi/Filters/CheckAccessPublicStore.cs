using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Localization;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.WebApi.Filters
{
    public class CheckAccessPublicStore : TypeFilterAttribute
    {
        #region Fields

        private readonly bool _ignoreFilter;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public CheckAccessPublicStore(bool ignore = false) : base(typeof(CheckAccessPublicStoreFilter))
        {
            _ignoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter => _ignoreFilter;

        #endregion

        #region Nested filter

        private class CheckAccessPublicStoreFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly IPermissionService _permissionService;

            #endregion

            #region Ctor

            public CheckAccessPublicStoreFilter(bool ignoreFilter, IPermissionService permissionService)
            {
                _ignoreFilter = ignoreFilter;
                _permissionService = permissionService;
            }

            #endregion

            #region Methods

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                //check whether this filter has been overridden for the Action
                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<CheckAccessPublicStore>().FirstOrDefault();

                //ignore filter (the action is available even if navigation is not allowed)
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //check whether current customer has access to a public store
                if (_permissionService.Authorize(StandardPermissionProvider.PublicStoreAllowNavigation))
                    return;

                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var response = new BaseResponseModel
                {
                    ErrorList = new List<string>
                    {
                        localizationService.GetResource("NopStation.WebApi.Response.CannotAccessPublicStore")
                    }
                };

                //customer hasn't access to a public store
                filterContext.Result = new UnauthorizedObjectResult(response);
            }

            #endregion
        }

        #endregion
    }
}