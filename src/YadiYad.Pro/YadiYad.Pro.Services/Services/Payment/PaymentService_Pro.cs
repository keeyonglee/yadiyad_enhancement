using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;

using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.Payment;

namespace YadiYad.Pro.Services.Payment
{
    public partial class PaymentService_Pro : Nop.Services.Payments.IPaymentService_Pro
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentService_Pro(ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor,
            IPaymentPluginManager paymentPluginManager,
            PaymentSettings paymentSettings)
        {
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
            _paymentPluginManager = paymentPluginManager;
            _paymentSettings = paymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public virtual ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResult
                {
                    NewPaymentStatus = Nop.Core.Domain.Payments.PaymentStatus.Paid
                };
                return result;
            }

            //We should strip out any white space or dash in the CC number entered.
            if (!string.IsNullOrWhiteSpace(processPaymentRequest.CreditCardNumber))
            {
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace(" ", string.Empty);
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace("-", string.Empty);
            }

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var paymentMethod = _paymentPluginManager
                .LoadPluginBySystemName(processPaymentRequest.PaymentMethodSystemName, customer, processPaymentRequest.StoreId)
                ?? throw new NopException("Payment method couldn't be loaded");

            return paymentMethod.ProcessPayment(processPaymentRequest);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public virtual void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //already paid or order.OrderTotal == decimal.Zero
            if (postProcessPaymentRequest.CustomOrder.PaymentStatus == PaymentStatus.Paid)
                return;

            var customer = _customerService.GetCustomerById(postProcessPaymentRequest.CustomOrder.CustomerId);
            var paymentMethod = _paymentPluginManager
                .LoadPluginBySystemName(postProcessPaymentRequest.CustomOrder.PaymentMethodSystemName, customer, postProcessPaymentRequest.CustomOrder.StoreId)
                ?? throw new NopException("Payment method couldn't be loaded");

            paymentMethod.PostProcessPayment(postProcessPaymentRequest);
        }

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        public virtual string GetMaskedCreditCardNumber(string creditCardNumber)
        {
            if (string.IsNullOrEmpty(creditCardNumber))
                return string.Empty;

            if (creditCardNumber.Length <= 4)
                return creditCardNumber;

            var last4 = creditCardNumber.Substring(creditCardNumber.Length - 4, 4);
            var maskedChars = string.Empty;
            for (var i = 0; i < creditCardNumber.Length - 4; i++)
            {
                maskedChars += "*";
            }

            return maskedChars + last4;
        }

        /// <summary>
        /// Generate an order GUID
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        public virtual void GenerateOrderGuid(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                return;

            //we should use the same GUID for multiple payment attempts
            //this way a payment gateway can prevent security issues such as credit card brute-force attacks
            //in order to avoid any possible limitations by payment gateway we reset GUID periodically
            var previousPaymentRequest = _httpContextAccessor.HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            if (_paymentSettings.RegenerateOrderGuidInterval > 0 &&
                previousPaymentRequest != null &&
                previousPaymentRequest.OrderGuidGeneratedOnUtc.HasValue)
            {
                var interval = DateTime.UtcNow - previousPaymentRequest.OrderGuidGeneratedOnUtc.Value;
                if (interval.TotalSeconds < _paymentSettings.RegenerateOrderGuidInterval)
                {
                    processPaymentRequest.OrderGuid = previousPaymentRequest.OrderGuid;
                    processPaymentRequest.OrderGuidGeneratedOnUtc = previousPaymentRequest.OrderGuidGeneratedOnUtc;
                }
            }

            if (processPaymentRequest.OrderGuid == Guid.Empty)
            {
                processPaymentRequest.OrderGuid = Guid.NewGuid();
                processPaymentRequest.OrderGuidGeneratedOnUtc = DateTime.UtcNow;
            }
        }

        #endregion
    }
}
