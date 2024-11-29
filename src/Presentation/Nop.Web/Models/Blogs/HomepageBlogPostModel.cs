using Nop.Core.Domain.Blogs;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Blogs
{
    public partial class HomepageBlogPostModel : BaseNopModel, ICloneable
    {
        public HomepageBlogPostModel()
        {
            BlogPost = new List<BlogPostModel>();
        }

        public int WorkingLanguageId { get; set; }
        public IList<BlogPostModel> BlogPost { get; set; }

        public object Clone()
        {
            //we use a shallow copy (deep clone is not required here)
            return MemberwiseClone();
        }
    }
}
