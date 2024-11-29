using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Vendors
{
    public partial class VendorApplicationSampleProductPicture : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        public int VendorApplicationId { get; set; }
        public int PictureId { get; set; }
    }
}
