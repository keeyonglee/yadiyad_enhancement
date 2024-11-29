using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Models.Payout;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.Payout;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/payout")]
    public class PayoutApiController : BaseApiController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly PayoutModelFactory _payoutModelFactory;
        private readonly PayoutBatchService _payoutBatchService;

        #endregion

        #region Ctor

        public PayoutApiController(ICustomerService customerService,
            IWorkContext workContext,
            PayoutModelFactory payoutModelFactory,
            PayoutBatchService payoutBatchService)
        {
            _customerService = customerService;
            _workContext = workContext;
            _payoutModelFactory = payoutModelFactory;
            _payoutBatchService = payoutBatchService;

        }

        #endregion

        #region Methods

        //My account / Payout List
        [HttpGet("payoutlist")]
        public virtual IActionResult VendorPayouts(
            PayoutGroupStatus? status,
            int pageNumber = 0,
            int pageSize = int.MaxValue)
        {
            if (!_customerService.IsVendor(_workContext.CurrentCustomer))
                return Unauthorized();

            int pageIndex = 0;
            if (pageNumber > 0)
                pageIndex = pageNumber - 1;

            var response = new GenericResponseModel<VendorPayoutListModel>();
            response.Data = _payoutModelFactory.PreparePayoutVendorListApiModel(_workContext.CurrentVendor.Id, status, pageIndex, pageSize);
            return Ok(response);
        }

        //My account / Payout details page
        [HttpGet("payoutdetails/{payoutGroupId:min(0)}")]
        public virtual IActionResult VendorPayoutDetails(int payoutGroupId)
        {
            if (!_customerService.IsVendor(_workContext.CurrentCustomer))
                return Unauthorized();

            var payoutGroupsDetails = _payoutBatchService.GetPayoutGroupsDetails(payoutGroupId);
            if (payoutGroupsDetails == null)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != payoutGroupsDetails.PayoutTo)
                return Unauthorized();

            var response = new GenericResponseModel<VendorPayoutDetailsModel>();
            response.Data = _payoutModelFactory.PrepareVendorPayoutDetailsApiModel(null, payoutGroupsDetails);

            return Ok(response);
        }

        //My account / Payout details page
        [HttpGet("payoutorders/{payoutGroupId:min(0)}")]
        public virtual IActionResult VendorPayoutOrders(int payoutGroupId,
            int pageNumber = 0,
            int pageSize = int.MaxValue)
        {
            if (!_customerService.IsVendor(_workContext.CurrentCustomer))
                return Unauthorized();

            var payoutGroupsDetails = _payoutBatchService.GetPayoutGroupsDetails(payoutGroupId);
            if (payoutGroupsDetails == null)
                return NotFound();

            if (_workContext.CurrentCustomer.Id != payoutGroupsDetails.PayoutTo)
                return Unauthorized();

            int pageIndex = 0;
            if (pageNumber > 0)
                pageIndex = pageNumber - 1;

            var response = new GenericResponseModel<VendorPayoutListModel>();
            response.Data = _payoutModelFactory.PreparePayoutVendorOrderListApiModel(payoutGroupId, pageIndex, pageSize);

            return Ok(response);
        }

        #endregion

    }
}
