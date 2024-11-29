using System;
using System.Collections.Generic;
using System.Text;

using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    public interface IOrderService_Pro
    {
        ICustomOrderEntity GetCustomOrder(int id);
        string GenerateOrderCustomNumber();
        //OrderDTO CreateOrder(int actorId, int customerId, OrderDTO dto);
        //OrderDTO UpdateOrder(int actorId, int customerId, OrderDTO dto);
        //ICustomOrderEntity GetCustomOrder(int id);
        //OrderDTO GetOrderById(int id);
        //PagedListDTO<OrderDTO> SearchOrders(
        //    int pageIndex = 0,
        //    int pageSize = int.MaxValue,
        //    OrderSearchFilterDTO filterDTO = null);
        //IPagedList<ProOrder> GetAll(
        //    int pageIndex = 0, int pageSize = int.MaxValue);
        //ProOrder GetById(int id);
        //IQueryable<PaymentDetailsDTO> GetPaymentDetailsQuery();
        //IPagedList<PaymentDetailsDTO> GetPaymentDetailsById(
        //    int pageIndex = 0,
        //    int pageSize = int.MaxValue,
        //    string keyword = null,
        //    PaymentDetailsSearchFilterDTO filterDTO = null);
    }
}
