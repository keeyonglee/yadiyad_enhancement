using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Models
{
    public class CategoryIconModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.Fields.Category")]
        public int CategoryId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.Fields.Category")]
        public string CategoryName { get; set; }

        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.Fields.Picture")]
        [UIHint("Picture")]
        public int IconId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.Fields.Picture")]
        public string PictureUrl { get; set; }
        
        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.Fields.CategoryBanner")]
        [UIHint("Picture")]
        public int CategoryBannerId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.Fields.CategoryBanner")]
        public string CategoryBannerUrl { get; set; }
    }
}
