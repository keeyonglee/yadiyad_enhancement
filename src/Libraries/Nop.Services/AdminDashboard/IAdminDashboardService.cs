using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.AdminDashboard
{
    public partial interface IAdminDashboardService
    {
        int GetProJobPost(DateTime selectedDate);
        int GetProJobHired(DateTime selectedDate);
        int GetProServiceHired(DateTime selectedDate);
        int GetShuqOrders(DateTime selectedDate);
        decimal GetTransactionValueProMonthly(DateTime selectedDate);
        decimal GetTransactionValueProWeekly(DateTime selectedDate);
        decimal GetTransactionValueShuqMonthly(DateTime selectedDate);
        decimal GetTransactionValueShuqWeekly(DateTime selectedDate);
        int GetMemberSignUpMonthly(DateTime selectedDate);
        int GetMemberSignUpWeekly(DateTime selectedDate);
        int GetShuqVendorSignUpMonthly(DateTime selectedDate);
        int GetShuqVendorSignUpWeekly(DateTime selectedDate);
        int GetProOrganizationSignUpMonthly(DateTime selectedDate);
        int GetProOrganizationSignUpWeekly(DateTime selectedDate);
        decimal GetServiceChargeProMonthly(DateTime selectedDate);
        decimal GetServiceChargeProWeekly(DateTime selectedDate);
        decimal GetServiceChargeShuqMonthly(DateTime selectedDate);
        decimal GetServiceChargeShuqWeekly(DateTime selectedDate);
        List<TopSegmentsChartModel> GetTopJobCVSegments(List<string> colorCodes);
        List<TopSegmentsChartModel> GetTopJobCVSegmentsCharge(List<string> colorCodes);
        List<TopSegmentsChartModel> GetTopServiceSegments(List<string> colorCodes);
        List<TopSegmentsChartModel> GetTopServiceSegmentsCharge(List<string> colorCodes);
        PagedList<TopOrganizationsTableModel> GetTopOrganizations();
        PagedList<TopOrganizationsTableModel> GetTopIndividuals();
        int GetNumberOfOrdersByVendorId(int vendorId, DateTime from, DateTime to);
    }
}
