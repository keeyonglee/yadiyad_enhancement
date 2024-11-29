using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.FactoriesPro;

namespace YadiYad.Pro.Web.Components
{
    public class SettingModePro : NopViewComponent
    {
        #region Fields

        private readonly SettingModelFactory _settingModelFactory;

        #endregion

        #region Ctor

        public SettingModePro(SettingModelFactory settingModelFactory)
        {
            _settingModelFactory = settingModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="modeName">Setting mode name</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string modeName = "settings-advanced-mode")
        {
            //prepare model
            var model = _settingModelFactory.PrepareSettingModeModel(modeName);

            return View(model);
        }

        #endregion
    }
}
