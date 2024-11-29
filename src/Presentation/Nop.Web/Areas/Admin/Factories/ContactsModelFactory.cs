using Nop.Services.YadiyadReporting;
using Nop.Web.Areas.Admin.Models.Reportings;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using YadiYad.Pro.Core.Domain.Order;

namespace Nop.Web.Areas.Admin.Factories
{
    public class ContactsModelFactory
    {
        #region Fields

        private readonly IYadiyadReportingContactsService _yadiyadReportingContactsService;

        #endregion

        #region Ctor

        public ContactsModelFactory(IYadiyadReportingContactsService yadiyadReportingContactsService)
        {
            _yadiyadReportingContactsService = yadiyadReportingContactsService;
        }

        #endregion

        #region Methods

        #region PrepareViewModel

        public virtual ContactsSearchModel PrepareReportingContactsSearchModel(ContactsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual RevenueExpenseSearchModel PrepareReportingProRevenueSearchModel(RevenueExpenseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual RevenueExpenseSearchModel PrepareReportingProExpenseSearchModel(RevenueExpenseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual RevenueExpenseSearchModel PrepareReportingShuqRevenueSearchModel(RevenueExpenseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region PrepareTableModel

        public virtual ContactsListModel PrepareReportingContactsListModel(ContactsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _yadiyadReportingContactsService.SearchContactsOrganization(searchModel.CreatedFrom, searchModel.CreatedTo,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new ContactsListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new ContactsModel();
                    model.Name = x.Name;
                    model.EstablishedDate = x.EstablishedDate;
                    model.Address = x.Address;
                    model.CreatedOnUtc = x.OrganizationCreatedOnUtc;
                    model.ContactPersonEmail = x.ContactPersonEmail;
                    model.ContactPersonContact = x.ContactPersonContact;
                    model.BusinessSegment = x.BusinessSegment;
                    model.CreatedOnUtc = x.OrganizationCreatedOnUtc;
                    model.ContactPersonPosition = x.ContactPersonPosition;
                    model.ContactPersonName = x.ContactPersonName;
                    model.CompanyRegistrationNo = x.CompanyRegistrationNo;
                    return model;
                });
            });
            return model;

        }

        public virtual ContactsListModel PrepareReportingRegistrationOnlyListModel(ContactsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _yadiyadReportingContactsService.SearchContactsRegistrationOnly(searchModel.CreatedFrom, searchModel.CreatedTo,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new ContactsListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new ContactsModel();
                    model.IndividualEmail = x.IndividualEmail;
                    model.IndividualEmailVerified = x.IndividualEmailVerified;
                    model.CreatedOnUtc = x.CreatedOnUtc;
                    return model;
                });
            });
            return model;

        }

        public virtual ContactsListModel PrepareReportingRegistrationProfileListModel(ContactsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _yadiyadReportingContactsService.SearchContactsRegistrationProfile(searchModel.CreatedFrom, searchModel.CreatedTo,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new ContactsListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new ContactsModel();
                    model.IndividualEmail = x.IndividualEmail;
                    model.IndividualFullName = x.IndividualFullName;
                    model.IndividualContactNumber = x.IndividualContactNumber;
                    model.IndividualDOB = x.IndividualDOB;
                    model.CreatedOnUtc = x.CreatedOnUtc;
                    return model;
                });
            });
            return model;
        }

        public virtual ContactsListModel PrepareReportingIndividualServiceJobListModel(ContactsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _yadiyadReportingContactsService.SearchContactsIndividualServiceJob(searchModel.CreatedFrom, searchModel.CreatedTo,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new ContactsListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new ContactsModel();
                    model.IndividualUsername = x.IndividualUsername;
                    model.IndividualFullName = x.IndividualFullName;
                    model.IndividualContactNumber = x.IndividualContactNumber;
                    model.IndividualDOB = x.IndividualDOB;
                    model.CreatedOnUtc = x.CreatedOnUtc;
                    model.LatestServiceProfileCreatedOnUTC = x.LatestServiceProfileCreatedOnUTC;
                    model.JobCVCreatedOnUTC = x.JobCVCreatedOnUTC;
                    model.ServiceProfileCount = x.ServiceProfileCount;

                    return model;
                });
            });
            return model;
        }

        public virtual RevenueExpenseListModel PrepareReportingProRevenueListModel(RevenueExpenseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _yadiyadReportingContactsService.SearchProRevenueList(searchModel.CreatedFrom, searchModel.CreatedTo,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new RevenueExpenseListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new RevenueExpenseModel();
                    model.CreatedDate = x.CreatedDate;
                    model.ProductTypeId = x.ProductTypeId;
                    model.InvoiceNo = x.InvoiceNo;
                    model.InvoiceTo = x.InvoiceTo;
                    model.InvoiceAmount = Math.Round(x.InvoiceAmount, 2);
                    model.ProductType = ((ProductType)x.ProductTypeId).GetDescription();

                    return model;
                });
            });
            return model;

        }

        public virtual RevenueExpenseListModel PrepareReportingProExpenseListModel(RevenueExpenseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _yadiyadReportingContactsService.SearchProExpenseList(searchModel.CreatedFrom, searchModel.CreatedTo,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new RevenueExpenseListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new RevenueExpenseModel();
                    model.CreatedDate = x.CreatedDate;
                    model.ProductTypeId = x.ProductTypeId;
                    model.InvoiceNo = x.InvoiceNo;
                    model.InvoiceTo = x.InvoiceTo;
                    model.InvoiceAmount = Math.Round(x.InvoiceAmount, 2);
                    model.ProductType = x.ProductType;

                    return model;
                });
            });
            return model;

        }
        public virtual RevenueExpenseListModel PrepareReportingShuqRevenueListModel(RevenueExpenseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _yadiyadReportingContactsService.SearchShuqRevenueList(searchModel.CreatedFrom, searchModel.CreatedTo,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new RevenueExpenseListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new RevenueExpenseModel();
                    model.CreatedDate = x.CreatedDate;
                    model.InvoiceNo = x.InvoiceNo;
                    model.InvoiceTo = x.InvoiceTo;
                    model.InvoiceAmount = Math.Round(x.InvoiceAmount, 2);

                    return model;
                });
            });
            return model;

        }

        #endregion

        #endregion
    }
}
