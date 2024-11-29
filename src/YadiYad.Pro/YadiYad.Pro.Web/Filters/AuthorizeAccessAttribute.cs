using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Security;

namespace YadiYad.Pro.Web.Filters
{

    /// <summary>
    /// Represents a filter attribute that confirms access to public store
    /// </summary>
    public sealed class AuthorizeAccessAttribute : TypeFilterAttribute
    {
        #region Fields

        private readonly List<string> _permissions = new List<string>();

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="permissions">Whether to ignore the execution of filter actions</param>
        public AuthorizeAccessAttribute(params string[] permissions) : base(typeof(AuthorizeAccessFilter))
        {
            foreach(var permission in permissions)
            {
                _permissions.Add(permission);
            }
            Arguments = new object[] { permissions };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public List<string> Permissions => _permissions;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to public store
        /// </summary>
        private class AuthorizeAccessFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly List<string> _permissions;
            private readonly IPermissionService _permissionService;
            private readonly IWebHelper _webHelper;
            private readonly IWorkContext _workContext;
            private readonly ICustomerService _customerService;

            #endregion

            #region Ctor

            public AuthorizeAccessFilter(
                string[] permissions, 
                IPermissionService permissionService, 
                IWebHelper webHelper, 
                IWorkContext workContext,
                ICustomerService customerService)
            {
                _permissions = new List<string>();
                foreach (var permission in permissions)
                {
                    _permissions.Add(permission);
                }
                _permissionService = permissionService;
                _webHelper = webHelper;
                _workContext = workContext;
                _customerService = customerService;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="filterContext">Authorization filter context</param>
            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (_permissions == null || _permissions.Count <= 0)
                {
                    if (_customerService.IsRegistered(_workContext.CurrentCustomer))
                        return;
                }
                else
                {
                    //check whether this filter has been overridden for the Action
                    var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                        .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                        .Select(filterDescriptor => filterDescriptor.Filter).OfType<AuthorizeAccessAttribute>().FirstOrDefault();

                    foreach (var permission in _permissions)
                    {
                        //check whether current customer has access to a public store
                        if (_permissionService.Authorize(permission))
                            return;
                    }
                }

                var returnUrl = _webHelper.GetRawUrl(filterContext.HttpContext.Request);

                //customer hasn't access to a public store
                if (string.IsNullOrEmpty(_workContext.CurrentCustomer.Username)
                    && returnUrl.ToLower().Contains("/pro"))
                {
                    filterContext.Result = new RedirectToRouteResult("ProLogin", new
                    {
                        returnUrl
                    });
                }
                else
                {
                    filterContext.Result = new ChallengeResult();
                }
            }

            #endregion
        }

        #endregion
    }
}
