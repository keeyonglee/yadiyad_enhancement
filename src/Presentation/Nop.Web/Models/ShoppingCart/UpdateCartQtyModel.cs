using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart
{
    public partial class UpdateCartQtyModel
    {
        public UpdateCartQtyModel()
        {
        }
        public int cartItemId { get; set; }
        public int qty { get; set; }

    }
}