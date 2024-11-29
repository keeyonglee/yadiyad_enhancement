using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using Nop.Core.Domain.Media;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request model
    /// </summary>
    public partial class GroupReturnRequestModel : BaseNopEntityModel
    {
        #region Ctor

        public GroupReturnRequestModel()
        {
            ReturnList = new List<ReturnRequestModel>();
            Pictures = new List<ReturnRequestPictureModel>();
        }

        #endregion
        
        public IList<ReturnRequestModel> ReturnList { get; set; }
        public int ApproveStatusId { get; set; }
        public string ApproveStatusStr { get; set; }
        public string ApproveStatusDetailsStr { get; set; }
        public int ReturnRequestStatusId { get; set; }
        public string ReturnRequestStatusStr { get; set; }
        public int CustomerId { get; set; }
        public string CustomerInfo { get; set; }
        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }
        public DateTime createdOnUtc { get; set; }
        public DateTime updatedOnUtc { get; set; }
        [NopResourceDisplayName("Admin.GroupReturnRequest.Edit.IsInsuranceClaim")]
        public bool IsInsuranceClaim { get; set; }
        public bool HasInsuranceCover { get; set; }
        public decimal? InsuranceClaimAmt { get; set; }
        public int InsuranceRef { get; set; }
        public DateTime? InsurancePayoutDate { get; set; }
        public bool IsOrderShipped { get; set; }
        public bool IsCategoryValid { get; set; }
        public bool NeedReturnShipping { get; set; }
        public int ReturnConditionId { get; set; }
        public bool CanProductShip { get; set; }
        public IList<ReturnRequestPictureModel> Pictures { get; set; }
        public bool RequireBarCode { get; set; }
        public string TrackingNumberUrl { get; set; }

    }

    public partial class ReturnRequestPictureModel
    {
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
        public int DisplayOrder { get; set; }
        public string OverrideAltAttribute { get; set; }
        public string OverrideTitleAttribute { get; set; }
        public string ContentType { get; set; }
        public Guid DownloadGuid { get; set; }
        public string FileName { get; set; }
    }
}