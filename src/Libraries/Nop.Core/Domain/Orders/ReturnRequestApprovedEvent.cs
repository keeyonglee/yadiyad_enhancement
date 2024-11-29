namespace Nop.Core.Domain.Orders
{
    public class ReturnRequestApprovedEvent
    {
        public GroupReturnRequest GroupReturnRequest { get; private set; }

        public ReturnRequestApprovedEvent(GroupReturnRequest groupReturnRequest)
        {
            GroupReturnRequest = groupReturnRequest;
        }
    }
}