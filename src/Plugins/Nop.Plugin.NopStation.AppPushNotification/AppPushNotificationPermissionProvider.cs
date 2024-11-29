using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.AppPushNotification
{
    public class AppPushNotificationPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageCampaigns = new PermissionRecord { Name = "NopStation app push notification. Manage Campaigns", SystemName = "NopStationAppPushNotificationManageCampaigns", Category = "NopStation" };
        public static readonly PermissionRecord ManageReports = new PermissionRecord { Name = "NopStation app push notification. Manage Reports", SystemName = "NopStationAppPushNotificationManageReports", Category = "NopStation" };
        public static readonly PermissionRecord ManageTemplates = new PermissionRecord { Name = "NopStation app push notification. Manage Templates", SystemName = "NopStationAppPushNotificationManageTemplates", Category = "NopStation" };
        public static readonly PermissionRecord ManageQueuedNotifications = new PermissionRecord { Name = "NopStation app push notification. Queued Notifications", SystemName = "NopStationAppPushNotificationManageQueuedNotification", Category = "NopStation" };
        public static readonly PermissionRecord ManageSmartGroups = new PermissionRecord { Name = "NopStation app push notification. Manage Smart Groups", SystemName = "NopStationAppPushNotificationManageSmartGroups", Category = "NopStation" };
        public static readonly PermissionRecord ManageConfiguration = new PermissionRecord { Name = "NopStation app push notification. Manage Configuration", SystemName = "NopStationAppPushNotificationManageConfiguration", Category = "NopStation" };

        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorRoleName,
                    new[]
                    {
                        ManageCampaigns,
                        ManageReports,
                        ManageTemplates,
                        ManageQueuedNotifications,
                        ManageSmartGroups,
                        ManageConfiguration
                    }
                )
            };
        }

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageCampaigns,
                ManageReports,
                ManageTemplates,
                ManageQueuedNotifications,
                ManageSmartGroups,
                ManageConfiguration
            };
        }
    }
}
