using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.Payout;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Route("pro/[controller]")]
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class PayoutRequestController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly PayoutRequestService _payoutRequestService;
        private readonly FeeCalculationService _feeCalculationService;
        #endregion

        #region Ctor

        public PayoutRequestController(
            IWorkContext workContext,
            PayoutRequestService payoutRequestService,
            FeeCalculationService feeCalculationService)
        {
            _workContext = workContext;
            _payoutRequestService = payoutRequestService;
            _feeCalculationService = feeCalculationService;
        }

        #endregion

        [HttpGet("{id}")]
        public IActionResult Index(
            [FromRoute] int id,
            [FromQuery] int refId,
            [FromQuery] int productTypeId)
        {
            var customerId = _workContext.CurrentCustomer.Id;
            var recordDTO = new PayoutRequestDTO();

            if (id == 0)
            {
                recordDTO = _payoutRequestService.GetNewPayoutRequest(customerId, productTypeId, refId);
            }
            else
            {
                recordDTO = _payoutRequestService.GetPayoutRequest(customerId, id);
            }


            return PartialView("_Details", recordDTO);
        }
    }
}
