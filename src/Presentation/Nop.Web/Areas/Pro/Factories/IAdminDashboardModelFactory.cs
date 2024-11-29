using Nop.Web.Areas.Pro.Models.AdminDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Factories
{
    public partial interface IAdminDashboardModelFactory
    {
        TopOrganizationsListModel PrepareTopOrganizationsListModel(TopOrganizationsSearchModel searchModel);
        TopOrganizationsListModel PrepareTopIndividualsListModel(TopOrganizationsSearchModel searchModel);
    }
}
