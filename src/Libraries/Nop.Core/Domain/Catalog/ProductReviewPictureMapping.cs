using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductReviewPictureMapping : BaseEntity
    {
        public int ProductReviewId { get; set; }
        public int PictureId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
