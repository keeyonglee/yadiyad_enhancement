using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Web.Framework.Controllers;

using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Payment;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Services.Payments;
using YadiYad.Pro.Services.Deposit;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class PaymentController : BasePaymentController
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly OrderService _orderService;
        private readonly IPaymentService_Pro _paymentService;
        private readonly DepositRequestService _depositRequestService;

        public PaymentController(
            IWorkContext workContext,
            AccountContext accountContext,
            OrderService orderService,
            DepositRequestService depositRequestService,
            IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory,
            IPaymentService_Pro paymentService_Pro)
        {
            _workContext = workContext;
            _accountContext = accountContext;
            _orderService = orderService;
            _depositRequestService = depositRequestService;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _paymentService = paymentService_Pro;
        }

        [HttpGet("option")]
        public IActionResult GetPaymentOptions()
        {
            var response = new ResponseDTO();
            var paymentOptionDTOs = new List<PaymentOptionDTO>
            {
                new PaymentOptionDTO
                {
                    Id = 1,
                    Name = "",
                    ImageUrl = "/img/pro/payment/duitnow-logo.png"
                },

                new PaymentOptionDTO
                {
                    Id = 2,
                    Name = "",
                    ImageUrl = "/img/pro/payment/tngewallet-logo.png"
                },
                new PaymentOptionDTO
                {
                    Id = 3,
                    Name = "",
                    ImageUrl = "/img/pro/payment/visa-logo.png"
                },
                new PaymentOptionDTO
                {
                    Id = 4,
                    Name = "",
                    ImageUrl = "/img/pro/payment/mastercard-logo.png"
                },
                new PaymentOptionDTO
                {
                    Id = 5,
                    Name = "",
                    ImageUrl = "/img/pro/payment/fpx-logo.png"
                }
            };

            response.SetResponse(paymentOptionDTOs);

            return Ok(response);
        }

        [HttpPost("")]
        public IActionResult GeneratePaymentLink([FromBody]OrderDTO orderDTO)
        {
            var paymentResponseURL = "/PaymentIPay88/RedirectToPaymentUrl";
            var redirectURL = HttpUtility.UrlEncode("");
            var paymentURL = $"{paymentResponseURL}?r={redirectURL}&o={orderDTO.Id}";

            var order = _orderService.GetCustomOrder(orderDTO.Id);

            //Redirection will not work on one page checkout page because it's AJAX request.
            //That's why we process it here
            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                CustomOrder = order
            };

            _paymentService.PostProcessPayment(postProcessPaymentRequest);


            var responseDTO = new ResponseDTO();
            var paymentRequestDTO = new PaymentRequestDTO
            {
                PaymentURL = paymentURL
            };

            responseDTO.SetResponse(paymentRequestDTO);

            return Ok(responseDTO);
        }

        [HttpGet("response")]
        public virtual IActionResult ProcessPaymentResponse(
            [FromQuery] int orderId,
            [FromQuery] int status = 1,
            [FromQuery(Name = "r")] string redirectURL = null)
        {
            var currentCustomerId = _workContext.CurrentCustomer.Id;

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var order = _orderService.GetOrderById(orderId);
            int productTypeId = 0;
            int refId = 0;

            if (order != null
                && order.OrderItems.Count() > 0)
            {
                var orderItem = order.OrderItems.First();
                productTypeId = orderItem.ProductTypeId;
                refId = orderItem.RefId;

                if(productTypeId == (int)ProductType.DepositRequest)
                {
                    var engagementOrderItem = _depositRequestService.GetDepositRequestByOrderItemId(orderItem.Id);
                    productTypeId = engagementOrderItem.ProductTypeId;
                    refId = engagementOrderItem.RefId;
                }
            }

            redirectURL = "";

            if (status == 0)
            {

                switch (productTypeId)
                {
                    case (int)ProductType.ApplyJobSubscription:
                        redirectURL = urlHelper.Action("Invites", "JobInvitation", new
                        {
                            area = "Pro"
                        });
                        break;
                    case (int)ProductType.ViewJobCandidateFullProfileSubscription:
                        redirectURL = urlHelper.Action("Candidate", "Job", new
                        {
                            area = "Pro"
                        });
                        break;
                    case (int)ProductType.ConsultationEngagementMatchingFee:
                    case (int)ProductType.ConsultationEngagementFee:
                        redirectURL = urlHelper.Action("Applicants", "Consultation", new
                        {
                            area = "Pro"
                        });
                        break;
                    case (int)ProductType.ServiceEnagegementMatchingFee:
                    case (int)ProductType.ServiceEnagegementFee:
                        redirectURL = urlHelper.Action("Requests", "ServiceApplication", new
                        {
                            area = "Pro"
                        });
                        break;
                    case (int)ProductType.JobEnagegementFee:
                        redirectURL = urlHelper.Action("Applicants", "JobApplication", new
                        {
                            area = "Pro"
                        });
                        break;
                }
            }
            else { 
                switch (productTypeId)
                {
                    case (int)ProductType.ApplyJobSubscription:
                        redirectURL = urlHelper.Action("Invites", "JobInvitation", new
                        {
                            area = "Pro"
                        });
                        break;
                    case (int)ProductType.ViewJobCandidateFullProfileSubscription:
                        redirectURL = urlHelper.Action("Candidate", "Job", new
                        {
                            area = "Pro",
                            id = refId
                        });
                        break;
                    case (int)ProductType.ConsultationEngagementMatchingFee:
                    case (int)ProductType.ConsultationEngagementFee:
                        redirectURL = urlHelper.Action("ConfirmedOrder", "Consultation", new
                        {
                            area = "Pro"
                        });
                        break;
                    case (int)ProductType.ServiceEnagegementMatchingFee:
                    case (int)ProductType.ServiceEnagegementFee:
                        redirectURL = urlHelper.Action("Confirms", "ServiceApplication", new
                        {
                            area = "Pro"
                        });
                        break;
                    case (int)ProductType.JobEnagegementFee:
                        redirectURL = urlHelper.Action("Hired", "JobApplication", new
                        {
                            area = "Pro"
                        });
                        break;
                }
            }

            _accountContext.ClearAccountSession();
            return Redirect(redirectURL);
        }
    }
}
