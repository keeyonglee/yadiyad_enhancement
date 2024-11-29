namespace Nop.Core.Domain.Orders
{
    public class OrderCompletedEvent
    {
        public Order Order { get; private set; }

        public OrderCompletedEvent(Order order)
        {
            Order = order;
        }
    }
}