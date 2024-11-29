namespace Nop.Core.Domain.Orders
{
    public class OrderPreparedEvent
    {
        public OrderPreparedEvent(Order order)
        {
            Order = order;
        }
        
        public Order Order { get; private set; }
    }
}