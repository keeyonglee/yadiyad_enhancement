namespace Nop.Plugin.Payments.IPay88.Models
{
    public class IPay88PaymentResponseModel
    {
        public string MerchantCode { get; set; }
        public int PaymentId { get; set; }
        public string RefNo { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Remark { get; set; }
        public string TransId { get; set; }
        public string AuthCode { get; set; }
        public string Status { get; set; }
        public string ErrDesc { get; set; }
        public string Signature { get; set; }        
        public string CCName { get; set; }
        public string CCNo { get; set; }
        public string S_bankname { get; set; }
        public string S_country { get; set; }
        public string ProxyResponseURL { get; set; }
        public string HiddenToURL { get; set; }
        public string ActionType { get; set; }
        public string TokenId { get; set; }
        public string CCCOriTokenId { get; set; }
        public string PromoCode { get; set; }
        public string DiscountedAmount { get; set; }
        public string MTVersion { get; set; }
        public string MTLogId { get; set; }
    }
}
