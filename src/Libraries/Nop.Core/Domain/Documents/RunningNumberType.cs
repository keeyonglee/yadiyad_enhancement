using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Core.Domain.Documents
{
    public enum RunningNumberType
    {
        [Display(Name = "Refund")]
        [DisplayFormat(DataFormatString = "Refund/{{Year}}/{{RunningNumber:000000}}")]
        Refund = 1,

        [Display(Name = "YadiYad Shuq Vendor Invoice {{VendorId}}")]
        [DisplayFormat(DataFormatString = "Seller-INV-SHUQ/{{Year}}/{{VendorId}}/{{RunningNumber:0000}}")]
        VendorShuqInvoice = 2,

        [Display(Name = "YadiYad Pro Seller Invoice {{CustomerId}}")]
        [DisplayFormat(DataFormatString = "Seller-INV-PRO/{{Year}}/{{CustomerId}}/{{RunningNumber:0000}}")]
        SellerProInvoice = 3,

        [Display(Name = "Credit Note")]
        [DisplayFormat(DataFormatString = "CN/{{Year}}//{{RunningNumber:0000}}")]
        CreditNote = 4,

        [Display(Name = "YadiYad Pro Invoice")]
        [DisplayFormat(DataFormatString = "INV-PRO/{{Year}}/{{RunningNumber:0000}}")]
        ProInvoice = 5,

        [Display(Name = "YadiYad Pro Deposit Request")]
        [DisplayFormat(DataFormatString = "Deposit/{{Year}}/{{RunningNumber:000000}}")]
        DepositRequest = 6,

        [Display(Name = "YadiYad Pro Moderator Invoice {{CustomerId}}")]
        [DisplayFormat(DataFormatString = "Moderator-INV/{{Year}}/{{CustomerId}}/{{RunningNumber:000000}}")]
        ModeratorInvoice = 7,

        [Display(Name = "YadiYad Shuq Payout")]
        [DisplayFormat(DataFormatString = "INV-SHUQ/{{Year}}/{{RunningNumber:0000}}")]
        ShuqInvoice = 8
    }
}
