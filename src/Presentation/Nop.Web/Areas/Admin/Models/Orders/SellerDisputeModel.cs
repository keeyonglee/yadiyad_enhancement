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
    public partial class SellerDisputeModel : BaseNopEntityModel
    {
        public SellerDisputeModel()
        {
            ReturnList = new List<ReturnRequestModel>();
            PictureId = new List<SellerDisputePicture>();
            AddPictureModel = new SellerDisputePictureModel();
            SellerDisputePictureSearchModel = new SellerDisputePictureSearchModel();
        }

        #region Properties
        public SellerDisputePictureSearchModel SellerDisputePictureSearchModel { get; set; }
        [NopResourceDisplayName("admin.dispute.fields.disputeReason")]
        public int DisputeReasonId { get; set; }
        [NopResourceDisplayName("admin.dispute.fields.disputeDetail")]
        public string DisputeDetail { get; set; }
        public int GroupReturnRequestId { get; set; }
        public string GroupReturnRequestApprovalStatus { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public int OrderId { get; set; }
        public IList<ReturnRequestModel> ReturnList { get; set; }

        public SellerDisputePictureModel AddPictureModel { get; set; }
        public IList<SellerDisputePicture> PictureId { get; set; }
        public bool CanRaiseDispute { get; set; }
        #endregion
    }
}