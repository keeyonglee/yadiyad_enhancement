using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request search model
    /// </summary>
    public class ReturnRequestItemSearchModel : BaseSearchModel
    {
        #region Ctor

        public ReturnRequestItemSearchModel()
        {
            ReturnRequestStatusList = new List<SelectListItem>();
        }

        #endregion

        #region Properties
        public int GroupReturnRequestId { get; set; }
        [NopResourceDisplayName("Admin.ReturnRequests.SearchStartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.SearchEndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.SearchCustomNumber")]
        public string CustomNumber { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.SearchReturnRequestStatus")]
        public int ReturnRequestStatusId { get; set; }

        public IList<SelectListItem> ReturnRequestStatusList { get; set; }

        #endregion
    }
}