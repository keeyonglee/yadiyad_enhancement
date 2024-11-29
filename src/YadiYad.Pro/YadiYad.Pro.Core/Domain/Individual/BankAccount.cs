using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Individual
{
    public class BankAccount : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public int BankId { get; set; }
        public string Comment { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public int BankStatementDownloadId { get; set; }
        public int IdentityType { get; set; }
        public string Identity { get; set; }
        public string SaltKey { get; set; }
        public bool? IsVerified { get; set; }
    }
}