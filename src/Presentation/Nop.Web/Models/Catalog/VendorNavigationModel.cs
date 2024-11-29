using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial class VendorNavigationModel : BaseNopModel
    {
        public VendorNavigationModel()
        {
            Vendors = new List<VendorBriefInfoModel>();
        }

        public IList<VendorBriefInfoModel> Vendors { get; set; }

        public int TotalVendors { get; set; }
    }

    public partial class VendorBriefInfoModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public string PictureUrl { get; set; }
        public Address Address { get; set; }
        public string StateProvince { get; set; }
        public int ProductQty { get; set; }
        public int RatingSum { get; set; }
        public int TotalReviews { get; set; }
        public bool Online { get; set; }
    }
}