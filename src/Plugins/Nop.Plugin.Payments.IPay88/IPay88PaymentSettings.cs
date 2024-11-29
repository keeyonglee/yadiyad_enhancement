using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.IPay88
{
    /// <summary>
    /// Represents settings of the IPay88 Standard payment plugin
    /// </summary>
    public class IPay88PaymentSettings : ISettings
    {
        public string PaymentUrl { get; set; }
        public string PaymentRequeryUrl { get; set; }
        public string PaymentPrefix { get; set; }
        public string InvoicePrefix { get; set; }
        public string ProxyPaymentURL { get; set; }

        //configurable through admin
        public string MerchantCode { get; set; }
        public string MerchantKey { get; set; }
        public string ProductDesc { get; set; }
        public string CurrencyCode { get; set; }
        public int PaymentId { get; set; }

        public bool IsEnableTestPayment { get; set; }
    }
}
