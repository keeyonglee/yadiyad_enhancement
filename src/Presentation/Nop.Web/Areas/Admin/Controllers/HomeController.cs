using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Payout;
using YadiYad.Pro.Services.Payout;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IHomeModelFactory _homeModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly PayoutBatchService _payoutBatchService;
        private readonly PayoutModelFactory _payoutModelFactory;
        private readonly IOrderModelFactory _orderModelFactory;
        #endregion

        #region Ctor

        public HomeController(AdminAreaSettings adminAreaSettings,
            ICommonModelFactory commonModelFactory,
            IHomeModelFactory homeModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext,
            ICustomerService customerService,
            PayoutBatchService payoutBatchService,
            PayoutModelFactory payoutModelFactory,
            IOrderModelFactory orderModelFactory)
        {
            _adminAreaSettings = adminAreaSettings;
            _commonModelFactory = commonModelFactory;
            _homeModelFactory = homeModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
            _customerService = customerService;
            _payoutBatchService = payoutBatchService;
            _payoutModelFactory = payoutModelFactory;
            _orderModelFactory = orderModelFactory;

        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            var customerId = _workContext.CurrentCustomer.Id;
            var customer = _customerService.GetCustomerById(customerId);
            var isOperator = _customerService.IsInCustomerRole(customer, NopCustomerDefaults.OperatorRoleName);
            var isAdmin = _customerService.IsAdmin(customer);
            var isVendor = _customerService.IsVendor(customer);
            if (isOperator || isAdmin)
            {
                return RedirectToAction("ShuqOperation");
            }
            else if (isVendor)
            {
                return RedirectToAction("Overview");
            }
            else
            {
                return RedirectToAction("Overview");
            }
        }

        [HttpPost]
        public virtual IActionResult NopCommerceNewsHideAdv()
        {
            _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_adminAreaSettings);

            return Content("Setting changed");
        }

        public virtual IActionResult Overview()
        {
            //prepare model
            var model = _homeModelFactory.PrepareDashboardModel(new DashboardModel());

            return View(model);
        }

        public virtual IActionResult VendorPayoutDetails(int id)
        {
            var payoutVendorDetails = _payoutBatchService.GetPayoutGroupsDetails(id);
            if (payoutVendorDetails == null)
                return RedirectToAction("Overview");

            //prepare model
            var model = _payoutModelFactory.PrepareVendorPayoutDetailsModel(null, payoutVendorDetails);

            return View(model);
        }

        public virtual IActionResult VendorPayoutOrderlist(int id)
        {
            var searchModel = new PayoutVendorSearchModel();
            searchModel.PayoutGroupId = id;
            //prepare model
            var model = _payoutModelFactory.PreparePayoutVendorOrderListSearchModel(searchModel);

            return View(model);
        }

        public virtual IActionResult ProOverview()
        {
            //prepare model
            var model = _homeModelFactory.PrepareDashboardModel(new DashboardModel());

            return View(model);
        }

        public virtual IActionResult ShuqOverview()
        {
            //prepare model
            var model = _homeModelFactory.PrepareDashboardModel(new DashboardModel());

            return View(model);
        }

        public virtual IActionResult ShuqOperation()
        {
            //display a warning to a store owner if there are some error
            if (_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
            {
                var warnings = _commonModelFactory.PrepareSystemWarningModels();
                if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail ||
                                            warning.Level == SystemWarningLevel.CopyrightRemovalKey ||
                                            warning.Level == SystemWarningLevel.Warning))
                    _notificationService.WarningNotification(
                        string.Format(_localizationService.GetResource("Admin.System.Warnings.Errors"),
                        Url.Action("Warnings", "Common")),
                        //do not encode URLs
                        false);
            }
            else
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.ManageShuqOperator))
                    return AccessDeniedView();
            }

            //prepare model
            var model = _homeModelFactory.PrepareDashboardModel(new DashboardModel());

            return View(model);
        }

        #endregion
    }
}