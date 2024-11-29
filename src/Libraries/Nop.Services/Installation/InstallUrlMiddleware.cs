using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Represents middleware that checks whether database is installed and redirects to installation URL in otherwise
    /// </summary>
    public class InstallUrlMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        public InstallUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext context, IWebHelper webHelper)
        {
            //whether database is installed
            if (!DataSettingsManager.DatabaseIsInstalled)
            {
                bool isInstallingDbPostRequest = context.Request.Method == "POST" && context.Request.Path == "/shuq/install";
                var thisPageUrl = webHelper.GetThisPageUrl(false); // localhost:55390/
                var installUrl = $"{webHelper.GetStoreLocation()}{NopInstallationDefaults.InstallPath}"; // localhost:55390/install
                // exception where it fails the check: posting when posting to install but webhelper says it's localhost:55390/

                // skip the database check if it is trying to install the database
                if (!isInstallingDbPostRequest)
                {
                    if (!thisPageUrl.StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //redirect
                        context.Response.Redirect(installUrl);
                        return;
                    }
                }
            }

            //or call the next middleware in the request pipeline
            await _next(context);
        }

        #endregion
    }
}