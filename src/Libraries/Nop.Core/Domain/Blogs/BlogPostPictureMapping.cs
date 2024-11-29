using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Blogs
{
    public partial class BlogPostPictureMapping : BaseEntity
    {
        public int BlogPostId { get; set; }
        public int PictureId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
