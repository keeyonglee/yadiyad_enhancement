using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.BankAccount
{
    public class BankAccountSearchFilterDTO
    {
        public bool? IsVerified { get; set; }
        public string AccountHolderName { get; set; }
    }
}
