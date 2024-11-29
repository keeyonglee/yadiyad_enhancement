using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request model
    /// </summary>
    public partial class DisputeModel : BaseNopEntityModel
    {
        public DisputeModel()
        {
            ReturnList = new List<ReturnRequestModel>();
        }

        #region Properties
        public int DisputeReasonId { get; set; }
        public int DisputeActionId { get; set; }
        public string DisputeActionStr { get; set; }
        public string DisputeDetail { get; set; }
        public int GroupReturnRequestId { get; set; }
        public int OrderId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int CustomerId { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public IList<ReturnRequestModel> ReturnList { get; set; }
        [UIHint("Picture")]
        public int PictureId { get; set; }
        #endregion
    }
}