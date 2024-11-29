using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request model
    /// </summary>
    public partial class ReturnOrderModel : BaseNopEntityModel
    {
        #region Ctor

        public ReturnOrderModel()
        {
            ReturnList = new List<ReturnRequestModel>();
        }

        #endregion

        public IList<ReturnRequestModel> ReturnList { get; set; }
        public bool IsShipped { get; set; }
        public int GroupReturnRequestId { get; set; }
        public decimal? EstimatedShippingExclTax { get; set; }
        public decimal? EstimatedShippingInclTax { get; set; }
        public decimal? ActualShippingExclTax { get; set; }
        public decimal? ActualShippingInclTax { get; set; }
        public DateTime? createdOnUtc { get; set; }
        public DateTime? updatedOnUtc { get; set; }


    }
}