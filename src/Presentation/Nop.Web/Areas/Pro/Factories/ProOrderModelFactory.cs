using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Pro.Models.ProOrder;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Order;
using Nop.Web.Areas.Pro.Services;

namespace Nop.Web.Areas.Pro.Factories
{
    public class ProOrderModelFactory
    {
        #region Fields

        private readonly OrderService _orderService;
        private readonly InvoiceService _invoiceService;
        private readonly ProOrderService _proOrderService;

        #endregion

        #region Ctor

        public ProOrderModelFactory(OrderService orderService,
            InvoiceService invoiceService,
            ProOrderService proOrderService)
        {
            _orderService = orderService;
            _invoiceService = invoiceService;
            _proOrderService = proOrderService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare order search model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>Order search model</returns>
        public virtual ProOrderSearchModel PrepareOrderSearchModel(ProOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged order list model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>Order list model</returns>
        public virtual ProOrderListModel PrepareOrderListModel(ProOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var orderProModel = _proOrderService.SearchOrders(searchModel.OrderNumber, searchModel.OrderDate, searchModel.PaymentStatusId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            var model = new ProOrderListModel().PrepareToGrid(searchModel, orderProModel, () =>
            {
                return orderProModel.Select(order =>
                {
                    var orderModel = new ProOrderModel
                    {
                        Id = order.Id,
                        OrderNumber = order.OrderNumber,
                        CustomerId = order.CustomerId,
                        PaymentStatusId = order.PaymentStatusId,
                        OrderTotal = order.OrderTotal,
                        OrderDate = order.OrderDate,
                        PaidOnUTC = order.PaidOnUTC,
                        Email = order.Email
                    };

                    return orderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare order model
        /// </summary>
        /// <param name="model">Order model</param>
        /// <param name="order">Order</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Order model</returns>
        public virtual ProOrderModel PrepareOrderModel(ProOrderModel model, ProOrder order, bool excludeProperties = false)
        {
            if (order != null)
            {
                //fill in model values from the entity
                model ??= new ProOrderModel
                {
                    Id = order.Id,
   
                };

            }

            return model;
        }

        #endregion
    }
}
