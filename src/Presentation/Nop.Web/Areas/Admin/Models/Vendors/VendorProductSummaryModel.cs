using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    public partial class VendorProductSummaryModel : BaseNopModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public DateTime LastCreatedTime { get; set; }
    }
}