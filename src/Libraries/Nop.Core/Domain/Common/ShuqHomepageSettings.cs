using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class ShuqHomepageSettings: ISettings
    {
        public int EatsMaxFeaturedProducts { get; set; }
        public int MartMaxFeaturedProducts { get; set; }
    }
}