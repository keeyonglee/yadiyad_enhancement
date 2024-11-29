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
    public partial class DisputeSubmitModel : BaseNopEntityModel
    {
        public DisputeSubmitModel()
        {
            ReturnList = new List<ReturnRequestModel>();
            CustomerPictureIds = new List<ReturnRequestImage>();
            SellerPictureIds = new List<SellerDisputePicture>();
        }

        #region Properties
        public string DisputeReasonStr { get; set; }
        public int DisputeReasonId { get; set; }
        public string DisputeDetail { get; set; }
        [NopResourceDisplayName("admin.dispute.fields.action")]
        public int DisputeAction { get; set; }
        
        [NopResourceDisplayName("admin.dispute.fields.partial")]
        public decimal? PartialAmount { get; set; }
        public int VendorId { get; set; }
        [NopResourceDisplayName("admin.dispute.fields.vendor")]
        public string VendorName { get; set; }
        [NopResourceDisplayName("admin.dispute.fields.raiseclaim")]
        public bool RaiseClaim { get; set; }
        public int GroupReturnRequestId { get; set; }
        public int OrderId { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public IList<ReturnRequestModel> ReturnList { get; set; }
        [UIHint("Picture")]
        public int PictureId { get; set; }
        public IList<ReturnRequestImage> CustomerPictureIds { get; set; }
        public IList<SellerDisputePicture> SellerPictureIds { get; set; }
        public bool CanProductShip { get; set; }
        public bool NeedReturnShipping { get; set; }
        [NopResourceDisplayName("admin.dispute.fields.totalReturnAmount")]
        public decimal TotalReturnAmount { get; set; }
        public bool IsReturnDispute { get; set; }
        #endregion
    }
}