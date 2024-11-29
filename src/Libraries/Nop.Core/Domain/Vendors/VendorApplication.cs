using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using System;

namespace Nop.Core.Domain.Vendors
{
    public partial class VendorApplication : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        public int CustomerId { get; set; }
        public string StoreName { get; set; }
        public int BusinessNatureCategoryId { get; set; }
        public int? CategoryId { get; set; }
        public string ProposedCategory { get; set; }
        public string AdminComment { get; set; }
        public bool? IsApproved { get; set; }
        public bool Deleted { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
