using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Catalog
{
    public partial class ShuqBuyerReviewModel
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ReviewText { get; set; }
        public int ReviewRating { get; set; }
        public List<int> ReviewPictureIds { get; set; }
    }
}
