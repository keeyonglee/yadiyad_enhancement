using Nop.Core;
using Nop.Core.Domain.YadiyadReporting.DTO;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.YadiyadReporting
{
    public  interface IYadiyadReportingContactsService : ICustomerReportService
    {
        IPagedList<ReportingContactsOrganizationDTO> SearchContactsOrganization(DateTime? createdFrom = null, DateTime? createdTo = null,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);
        IPagedList<ReportingContactsOrganizationDTO> SearchContactsRegistrationOnly(DateTime? createdFrom, DateTime? createdTo,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);

        IPagedList<ReportingContactsOrganizationDTO> SearchContactsRegistrationProfile(DateTime? createdFrom, DateTime? createdTo,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);
        IPagedList<ReportingContactsOrganizationDTO> SearchContactsIndividualServiceJob(DateTime? createdFrom, DateTime? createdTo,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);
        IPagedList<ReportingProRevenueListDTO> SearchProRevenueList(DateTime? createdFrom = null, DateTime? createdTo = null,
           int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);
        IPagedList<ReportingProRevenueListDTO> SearchProExpenseList(DateTime? createdFrom = null, DateTime? createdTo = null,
          int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);
        IPagedList<ReportingProRevenueListDTO> SearchShuqRevenueList(DateTime? createdFrom = null, DateTime? createdTo = null,
          int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);

        IList<ReportingContactsOrganizationDTO> GetAllOrganizationsExport(DateTime? createdFrom, DateTime? createdTo);
        IList<ReportingContactsOrganizationDTO> GetAllRegistrationOnlyExport(DateTime? createdFrom, DateTime? createdTo);
        IList<ReportingContactsOrganizationDTO> GetAllRegistrationProfileExport(DateTime? createdFrom, DateTime? createdTo);
        IList<ReportingContactsOrganizationDTO> GetAllIndividualServiceJobExport(DateTime? createdFrom, DateTime? createdTo);
        IList<ReportingProRevenueListDTO> GetAllProRevenueExport(DateTime? createdFrom, DateTime? createdTo);
        IList<ReportingProRevenueListDTO> GetAllProExpensesExport(DateTime? createdFrom, DateTime? createdTo);
        IList<ReportingProRevenueListDTO> GetAllShuqRevenueExport(DateTime? createdFrom, DateTime? createdTo);

    }
}
