using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.DTO.Base
{
    public enum AppResponse : int
    {
        [Description("Invalid username or password.")]
        InvalidCredential = 401
    }
}
