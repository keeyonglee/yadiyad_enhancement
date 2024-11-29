using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System;

namespace Nop.Web.Models.Order
{
    public partial class OrderCancellationModel
    {
        public int OrderId { get; set; }
        public int CancellationReasonId { get; set; }
        public string CancellationReasonText
        {
            get
            {
                var text =
                CancellationReasonId == 0
                ? OrderCancellationReason.Address.GetDescription()
                : CancellationReasonId == (int)OrderCancellationReason.Address
                ? OrderCancellationReason.Address.GetDescription()
                : CancellationReasonId == (int)OrderCancellationReason.Inquiries
                ? OrderCancellationReason.Inquiries.GetDescription()
                : CancellationReasonId == (int)OrderCancellationReason.Modify
                ? OrderCancellationReason.Modify.GetDescription()
                : CancellationReasonId == (int)OrderCancellationReason.Others
                ? OrderCancellationReason.Others.GetDescription()
                : "Unknown";

                return text;
            }
        }
    }
}