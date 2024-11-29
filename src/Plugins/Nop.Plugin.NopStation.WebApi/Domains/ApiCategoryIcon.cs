using Nop.Core;

namespace Nop.Plugin.NopStation.WebApi.Domains
{
    public partial class ApiCategoryIcon : BaseEntity
    {
        public int CategoryId { get; set; }

        public int PictureId { get; set; }

        public int CategoryBannerId { get; set; }
    }
}