using Nop.Web.Framework.Models;

namespace YadiYad.Pro.Web.Models.Customer
{
    public partial class GdprConsentProModel : BaseNopEntityModel
    {
        public string Message { get; set; }

        public bool IsRequired { get; set; }

        public string RequiredMessage { get; set; }

        public bool Accepted { get; set; }
    }
}