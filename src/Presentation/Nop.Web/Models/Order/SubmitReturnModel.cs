using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System;

namespace Nop.Web.Models.Order
{
    public partial class SubmitReturnModel
    {
        public SubmitReturnModel()
        {
            Items = new List<OrderItemModel>();
        }

        public IList<OrderItemModel> Items { get; set; }
        public int OrderId { get; set; }
        public string Comments { get; set; }
        public int ReturnRequestReasonId { get; set; }
        public int ReturnRequestActionId { get; set; }
        public List<int> ReturnRequestImageId { get; set; }
        public partial class OrderItemModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string AttributeInfo { get; set; }

            public string UnitPrice { get; set; }

            public int Quantity { get; set; }
        }
    }
}