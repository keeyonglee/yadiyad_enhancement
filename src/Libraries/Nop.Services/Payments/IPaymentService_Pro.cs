using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Payments
{
    public interface IPaymentService_Pro
    {
        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest);

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest);

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        string GetMaskedCreditCardNumber(string creditCardNumber);

        /// <summary>
        /// Generate an order GUID
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        void GenerateOrderGuid(ProcessPaymentRequest processPaymentRequest);
    }
}
