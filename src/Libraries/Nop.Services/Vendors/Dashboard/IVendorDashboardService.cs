using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using Nop.Core.Domain.Vendors;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Google.Protobuf.WellKnownTypes;
using LinqToDB.Common;

namespace Nop.Services.Vendors.Dashboard
{
    public interface IVendorDashboardService
    {
        int OpenOrderCount(int vendorId);
        int CompletedOrderCount(int vendorId);
        
        /// <summary>
        /// Returns (PreviousMonthOrderCount, CurrentMonthOrderCount)
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        (int, int) MonthlyOrderSummary(int vendorId);
        
        IList<VendorDashboardService.VendorOrderShipment> OrdersPendingShipment(int vendorId);
        IList<VendorDashboardService.ProductSummary> TopSoldProducts(int vendorId, int topCount = 5, int? days = null);
        IList<VendorDashboardService.ProductSummary> TopReturnedProducts(int vendorId, int topCount = 5, int? days = null);
        IList<VendorDashboardService.VendorReturnRequestAction> OrdersNeedReturnAction(int vendorId);
    }
}