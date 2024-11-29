using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class YadiYadProBankAccountSettings : ISettings
    {
        public string BankName { get; set; }
        public string BankAccountNo { get; set; }
    }
}
