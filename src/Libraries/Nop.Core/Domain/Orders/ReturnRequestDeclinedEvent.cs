namespace Nop.Core.Domain.Orders
{
    public class ReturnRequestDeclinedEvent
    {
        public GroupReturnRequest GroupReturnRequest { get; }

        public ReturnRequestDeclinedEvent(GroupReturnRequest groupReturnRequest)
        {
            GroupReturnRequest = groupReturnRequest;
        }
    }
}