using Nop.Core.Domain.Payments;

namespace Nop.Plugin.Payments.IPay88
{
    /// <summary>
    /// Represents paypal helper
    /// </summary>
    public class IPay88Helper
    {
        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">PayPal payment status</param>
        /// <param name="pendingReason">PayPal pending reason</param>
        /// <returns>Payment status</returns>
        public static PaymentStatus GetPaymentStatus(string paymentStatus, string pendingReason)
        {
            var result = PaymentStatus.Pending;

            if (paymentStatus == null)
                paymentStatus = string.Empty;

            if (pendingReason == null)
                pendingReason = string.Empty;

            switch (paymentStatus.ToLowerInvariant())
            {
                case "pending":
                    switch (pendingReason.ToLowerInvariant())
                    {
                        case "authorization":
                            result = PaymentStatus.Authorized;
                            break;
                        default:
                            result = PaymentStatus.Pending;
                            break;
                    }
                    break;
                case "processed":
                case "completed":
                case "canceled_reversal":
                    result = PaymentStatus.Paid;
                    break;
                case "denied":
                case "expired":
                case "failed":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                case "reversed":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }
            return result;
        }

        public static class PaymentStatusConstants
        {
            public static readonly string PENDING = "10";
            public static readonly string COMPLETED = "20";
            public static readonly string FAILED = "30";
        }

    }
}

