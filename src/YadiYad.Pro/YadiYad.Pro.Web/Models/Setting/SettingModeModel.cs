using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Setting
{
    public class SettingModeModel : BaseNopModel
    {
        #region Properties

        public string ModeName { get; set; }

        public bool Enabled { get; set; }

        #endregion
    }
}
