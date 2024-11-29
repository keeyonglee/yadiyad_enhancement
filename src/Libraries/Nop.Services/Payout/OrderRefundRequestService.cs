using LinqToDB;
using Nop.Core.Domain.Payout;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Payout
{
    public class OrderRefundRequestService
    {
        private readonly IRepository<OrderRefundRequest> _orderRefundRequestRepo;
        public OrderRefundRequestService(
            IRepository<OrderRefundRequest> orderRefundRequestRepo
            )
        {
            _orderRefundRequestRepo = orderRefundRequestRepo;
        }

        public async Task<OrderRefundRequest> GetOrderRefundRequestByIdAsync(int id)
        {
            var orderRefundRequest = await _orderRefundRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                .FirstOrDefaultAsync();

            return orderRefundRequest;
        }
    }
}
