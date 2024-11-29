using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using System;

namespace Nop.Core.Domain.Orders
{
    public partial class ReturnRequestImage : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        public int GroupReturnRequestId { get; set; }
        public int PictureId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
