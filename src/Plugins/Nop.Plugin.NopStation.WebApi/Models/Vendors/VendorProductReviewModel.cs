using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.WebApi.Models.Order
{
    public class VendorProductReviewModel
    {
        public VendorProductReviewModel()
        {
            Items = new List<ProductReviewModel>();
        }
        public PagerModel PagerModel { get; set; }
        public IList<ProductReviewModel> Items { get; set; }
    }
}
