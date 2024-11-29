using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Subscription;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class OrderController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly OrderService _orderService;
        private readonly OrderProcessingService _orderProcessingService;

        #endregion

        #region Ctor

        public OrderController(
            IWorkContext workContext,
            OrderService orderService,
            OrderProcessingService orderProcessingService)
        {
            _workContext = workContext;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
        }

        #endregion

        [HttpPost("{id}")]
        public IActionResult SubmitOrder([FromBody] SubmitOrderDTO dto, [FromRoute]int id = 0)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            var orderItem = _orderProcessingService.CreateOrder(dto);


            response.SetResponse(orderItem);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public virtual IActionResult Get(int id)
        {
            var response = new ResponseDTO();
            var dto = _orderService.GetOrderById(id);
            response.SetResponse(dto);

            return Ok(response);
        }

        [HttpPost("list")]
        public IActionResult SearchOrder([FromBody] ListFilterDTO<OrderSearchFilterDTO> searchDTO)
        {
            searchDTO.RecordSize = 10;

            var response = new ResponseDTO();

            var dto = _orderService.SearchOrders(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.AdvancedFilter);

            response.SetResponse(dto);

            return Ok(response);
        }

        [HttpGet("applyjob")]
        public IActionResult ApplyJob()
        {
            var response = new ResponseDTO();
            var subscription = new SubscriptionDTO
            {
                Id = 1,
                Name = "Apply Job",
                Code = "APPLYJOB",
                Features = new List<string>
                {
                    {"Unlimited Job Application" },
                    {"One Month Validity" }
                },
                Fee = 30m
            };
            response.SetResponse(subscription);

            return Ok(response);
        }

        [HttpGet("payconsultationfee")]
        public IActionResult PayConsultationFee(int consultationInvitationId)
        {
            var response = new ResponseDTO();
            var orderDTO = new OrderDTO
            {
                Id = 1,
                TotalPayableAmount = 1000.00m,
                TransactionNo = "0034730",
                MoreInfo = new
                {
                    ConsultationJobServiceChargeRate = 0.75
                }
            };
            response.SetResponse(orderDTO);

            return Ok(response);
        }

        [HttpPost("paymentlist")]
        public IActionResult GetPaymentDetailsByCustomerId([FromBody] ListFilterDTO<PaymentDetailsSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.CustomerId = _workContext.CurrentCustomer.Id;
            var data = _orderService.GetPaymentDetailsById(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            var dto = new PagedListDTO<PaymentDetailsDTO>(data);

            response.SetResponse(dto);
            return Ok(response);
        }
    }
}
