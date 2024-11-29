using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.IPay88.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.MerchantCode")]
        public string MerchantCode { get; set; }
        public bool MerchantCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.MerchantKey")]
        public string MerchantKey { get; set; }
        public bool MerchantKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.CurrencyCode")]
        public string CurrencyCode { get; set; }
        public bool CurrencyCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.ProductDesc")]
        public string ProductDesc { get; set; }
        public bool ProductDesc_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.PaymentId")]
        public string PaymentId { get; set; }
        public bool PaymentId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.ProxyPaymentURL")]
        public string ProxyPaymentURL { get; set; }
        public bool ProxyPaymentURL_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.PaymentUrl")]
        public string PaymentUrl { get; set; }
        public bool PaymentUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.PaymentRequeryUrl")]
        public string PaymentRequeryUrl { get; set; }
        public bool PaymentRequeryUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.PaymentPrefix")]
        public string PaymentPrefix { get; set; }
        public bool PaymentPrefix_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.IPay88.Fields.InvoicePrefix")]
        public string InvoicePrefix { get; set; }
        public bool InvoicePrefix_OverrideForStore { get; set; }
    }
}