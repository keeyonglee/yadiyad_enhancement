namespace Nop.Plugin.Payments.IPay88.Models
{
    public class IPay88PaymentResultModel
    {
        public int OrderId { get; set; }
        public bool IsPaymentSucceed { get; set; }
        public string ResultMessage { get; set; }
    }
}
