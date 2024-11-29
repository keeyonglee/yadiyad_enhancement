using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using System;

namespace Nop.Core.Domain.Vendors
{
    public partial class VendorPicture : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        public int VendorId { get; set; }
        public int PictureId { get; set; }
        public int DisplayOrder { get; set; }
        public int VendorPictureType { get; set; }
    }
}
