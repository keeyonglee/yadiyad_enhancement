using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.BankAccount
{
    public class BankAccountModel : BaseNopEntityModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int BankId { get; set; }
        [NopResourceDisplayName("Admin.BankAccount.Fields.AccountNumber")]
        public string AccountNumber { get; set; }
        [NopResourceDisplayName("Admin.BankAccount.Fields.AccountHolderName")]
        public string AccountHolderName { get; set; }
        [UIHint("PDFViewer")]
        public int BankStatementDownloadId { get; set; }
        public string Comment { get; set; }
        public bool? IsVerified { get; set; }
        public string Status { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        [NopResourceDisplayName("Admin.BankAccount.Fields.CreatedOnUTC")]
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
        public string BankName { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public Guid BankStatementDownloadGuid { get; set; }
        public int IdentityType { get; set; }
        [NopResourceDisplayName("Admin.BankAccount.Fields.IdentityTypeName")]
        public string IdentityTypeName { get; set; }
        public string Identity { get; set; }
    }
}
