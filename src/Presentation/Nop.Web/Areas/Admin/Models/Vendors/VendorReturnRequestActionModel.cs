using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    public partial class VendorReturnRequestActionModel : BaseNopModel
    {
        public int OrderId { get; set; }
        public int GroupReturnRequestId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReturnRequestDate { get; set; }
        public DateTime ActBefore { get; set; }
    }
}