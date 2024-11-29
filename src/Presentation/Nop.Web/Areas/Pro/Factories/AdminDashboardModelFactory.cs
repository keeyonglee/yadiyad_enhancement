using Nop.Services.AdminDashboard;
using Nop.Web.Areas.Pro.Models.AdminDashboard;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Factories
{
    public class AdminDashboardModelFactory : IAdminDashboardModelFactory
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardModelFactory(
            IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        public TopOrganizationsListModel PrepareTopIndividualsListModel(TopOrganizationsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var topInd = _adminDashboardService.GetTopIndividuals();

            var model = new TopOrganizationsListModel().PrepareToGrid(searchModel, topInd, () =>
            {
                return topInd.Select(entity =>
                {
                    var topIndModel = new TopOrganizationsModel
                    {
                        ContactPerson = entity.ContactPerson,
                        ContactPersonEmail = entity.ContactPersonEmail,
                        ContactPersonPhone = entity.ContactPersonPhone,
                        JobEngagementAmount = entity.JobEngagementAmount
                    };
                    return topIndModel;
                });
            });

            return model;
        }

        public TopOrganizationsListModel PrepareTopOrganizationsListModel(TopOrganizationsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var topOrg = _adminDashboardService.GetTopOrganizations();

            var model = new TopOrganizationsListModel().PrepareToGrid(searchModel, topOrg, () =>
            {
                return topOrg.Select(entity =>
                {
                    var topOrgModel = new TopOrganizationsModel
                    {
                        Name = entity.Name,
                        ContactPerson = entity.ContactPerson,
                        ContactPersonEmail = entity.ContactPersonEmail,
                        ContactPersonPhone = entity.ContactPersonPhone,
                        TotalCandidateHired = entity.TotalCandidateHired,
                        JobPostCount = entity.JobPostCount
                    };
                    return topOrgModel;
                });
            });

            return model;
        }
    }
}
