using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Order;

namespace Nop.Web.Areas.Admin.Models.Reportings
{
    public class RevenueExpenseModel : BaseNopEntityModel
    {
        public DateTime CreatedDate { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceTo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string ReferenceNo { get; set; }
        public decimal ReferenceAmount { get; set; }
        public int ProductTypeId { get; set; }
        public int InvoiceToId { get; set; }
        public string ProductType { get; set; }
        public string InvoiceAmountText
        {
            get
            {
                return String.Format("{0:.##}", InvoiceAmount);
            }
        }
        public string CreatedDateText
        {
            get
            {
                return CreatedDate.ToShortDateString();
            }
        }
    }
}
