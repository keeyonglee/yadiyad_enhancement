namespace Nop.Core.Domain.Orders
{
    public class OrderDisputeSettlementOutcomeEvent
    {
        public Dispute Dispute { get; }
        public GroupReturnRequest GroupReturnRequest { get; }

        public OrderDisputeSettlementOutcomeEvent(Dispute dispute, GroupReturnRequest groupReturnRequest)
        {
            Dispute = dispute;
            GroupReturnRequest = groupReturnRequest;
        }
    }
}