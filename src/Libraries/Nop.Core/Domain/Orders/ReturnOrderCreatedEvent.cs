namespace Nop.Core.Domain.Orders
{
    public class ReturnOrderCreatedEvent
    {
        public ReturnOrder ReturnOrder { get; }
        public ReturnOrderCreatedEvent(ReturnOrder returnOrder)
        {
            ReturnOrder = returnOrder;
        }
    }
}