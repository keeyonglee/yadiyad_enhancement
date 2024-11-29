using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Payout
{
    public class OrderPayoutRequestEvent
    {
        public OrderPayoutRequest OrderPayoutRequest { get; private set; }
        public PayoutAndGroupShuqDTO PayoutAndGroupDTO { get; private set; }
        public OrderPayoutRequestEvent(OrderPayoutRequest orderPayoutRequest, PayoutAndGroupShuqDTO payoutAndGroupDTO)
        {
            OrderPayoutRequest = orderPayoutRequest;
            PayoutAndGroupDTO = payoutAndGroupDTO;
        }
    }
}
