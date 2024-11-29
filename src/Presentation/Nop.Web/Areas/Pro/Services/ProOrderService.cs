using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Web.Areas.Pro.Models.ProOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Order;

namespace Nop.Web.Areas.Pro.Services
{
    public class ProOrderService
    {
        #region Fields

        private readonly IRepository<ProInvoice> _proInvoiceRepository;
        private readonly IRepository<ProOrder> _proOrderRepository;
        private readonly IRepository<Customer> _customerRepository;

        #endregion

        #region Ctor

        public ProOrderService(IRepository<ProInvoice> proInvoiceRepository,
            IRepository<ProOrder> proOrderRepository,
            IRepository<Customer> customerRepository)
        {
            _proInvoiceRepository = proInvoiceRepository;
            _proOrderRepository = proOrderRepository;
            _customerRepository = customerRepository;
        }

        #endregion

        #region Methods
        public virtual IPagedList<ProInvoice> GetAll2(
          int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _proInvoiceRepository.Table;

            query = query.OrderBy(n => n.Id);

            var data = new PagedList<ProInvoice>(query, pageIndex, pageSize);

            return data;
        }

        public virtual ProInvoice GetById(int id)
        {
            if (id == 0)
                return null;

            return _proInvoiceRepository.GetById(id);
        }

        //public virtual IPagedList<ProOrderModel> GetAll(
        //  int pageIndex = 0, int pageSize = int.MaxValue)
        //{
        //    var query = from pi in _proInvoiceRepository.Table
        //                where pi.Deleted == false
        //                from po in _proOrderRepository.Table
        //                where pi.OrderId == po.Id
        //                select new ProOrderModel
        //                {
        //                    Id = pi.Id,
        //                    CustomerId = pi.CreatedById,
        //                    OrderTotal = po.OrderTotal,
        //                    CreatedOnUTC = pi.CreatedOnUTC,
        //                    BaseInvoiceNumber = pi.BaseInvoiceNumber,
        //                    InvoiceNumber = pi.InvoiceNumber,
        //                    InvoiceType = pi.InvoiceType,
        //                };

        //    var data = new PagedList<ProOrderModel>(query, pageIndex, pageSize);

        //    return data;
        //}

        public virtual IPagedList<ProOrderModel> SearchOrders(string orderNumber, DateTime? orderDate, int paymentStatusId,
         int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from po in _proOrderRepository.Table
                        join c in _customerRepository.Table on po.CustomerId equals c.Id
                        where po.Deleted == false
                        select new ProOrderModel
                        {
                            Id = po.Id,
                            OrderNumber = po.CustomOrderNumber,
                            CustomerId = po.CustomerId,
                            PaymentStatusId = po.PaymentStatusId,
                            OrderTotal = po.OrderTotal,
                            OrderDate = po.CreatedOnUTC,
                            PaidOnUTC = po.PaidOnUTC,
                            Email = c.Email
                        };

            if (!string.IsNullOrEmpty(orderNumber))
                query = query.Where(o => o.OrderNumber == orderNumber);

            if (paymentStatusId > 0)
                query = query.Where(o => o.PaymentStatusId == paymentStatusId);

            if (orderDate.HasValue)
                query = query.Where(o => orderDate.Value <= o.OrderDate);

            query = query.OrderByDescending(x => x.Id);
            var data = new PagedList<ProOrderModel>(query, pageIndex, pageSize);

            return data;
        }
        #endregion
    }
}
