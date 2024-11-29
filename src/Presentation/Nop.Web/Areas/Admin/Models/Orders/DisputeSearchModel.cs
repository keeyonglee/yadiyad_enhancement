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
    public class DisputeSearchModel: BaseSearchModel
    {

        #region Properties
        public int GroupReturnRequestId { get; set; }
        [NopResourceDisplayName("Admin.ReturnRequests.SearchStartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.SearchEndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        public string DisputeDetails { get; set; }

        public int DisputeReasonId { get; set; }
        public int OrderId { get; set; }

        #endregion
    }
}