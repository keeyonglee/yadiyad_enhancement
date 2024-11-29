using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Order;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.WebApi.Models.Catalog
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            Categories = new List<CategoryModel>();
        }
        public List<CategoryModel> Categories { get; set; }
    }
}
