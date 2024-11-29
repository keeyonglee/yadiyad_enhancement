using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Order;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class OrderController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly YadiYadProBankAccountSettings _yadiYadProBankAccountSettings;
        private readonly DepositRequestService _depositRequestService;
        private readonly OrderProcessingService _orderProcessingService;
        private readonly OrderService _orderService;
        private readonly JobApplicationService _jobApplicationService;

        public OrderController(
            YadiYadProBankAccountSettings yadiYadProBankAccountSettings,
            IWorkContext workContext,
            DepositRequestService depositRequestService,
            OrderService orderService,
            OrderProcessingService orderProcessingService,
            JobApplicationService jobApplicationService
            )
        {
            _workContext = workContext;
            _depositRequestService = depositRequestService;
            _yadiYadProBankAccountSettings = yadiYadProBankAccountSettings;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _jobApplicationService = jobApplicationService;
        }


        public IActionResult ApplyJob()
        {
            return PartialView("_ApplyJob");
        }

        public IActionResult ApplyView()
        {
            return PartialView("_ApplyView");
        }

        public IActionResult PayConsultationFee()
        {
            return PartialView("_PayConsultationFee");
        }

        public IActionResult PayJobEscrow()
        {
            return PartialView("_PayJobEscrow");
        }

        public IActionResult PayDepositRequest()
        {
            return PartialView("_PayDepositRequest");
        }

        public IActionResult PayProjectDepositRequest([FromRoute(Name ="id")]int jobApplicationId)
        {
            var actorId = _workContext.CurrentCustomer.Id;
            var dto = _depositRequestService.GetDepositRequestByProductTypeRefId(actorId, jobApplicationId, (int)ProductType.JobEnagegementFee);

            if (dto == null)
            {
                dto = new DepositRequestDTO();
            }

            //only support offset all
            var offsetableOrderItem = _orderService.GetOrderItemBForOffsetByProductTypeRefId(actorId, ProductType.JobEnagegementFee, jobApplicationId);
            var engagement = _jobApplicationService.GetJobApplicationById(jobApplicationId);

            if (offsetableOrderItem != null)
            {
                dto.OffsetableAmount = offsetableOrderItem.Price;
                dto.OffsetableEngagementCode = offsetableOrderItem.EngagementCode;
                dto.Amount = engagement.PayAmount;
            }

            dto.ProductTypeId = (int)ProductType.JobEnagegementFee;
            dto.ProductStatus = engagement.JobApplicationStatus;

            dto.TransfereeBankAccountNo = _yadiYadProBankAccountSettings.BankAccountNo;
            dto.TransfereeBankName = _yadiYadProBankAccountSettings.BankName;

            return PartialView("_PayProjectDepositRequest", dto);
        }

        public IActionResult PaymentDetails()
        {
            return View();
        }

        public IActionResult ReturnRefund_Customer()
        {
            return View();
        }        


    }
}
