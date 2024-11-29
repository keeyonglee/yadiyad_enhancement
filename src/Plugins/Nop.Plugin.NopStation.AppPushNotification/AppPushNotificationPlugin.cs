using Nop.Core;
using Nop.Data;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.NopStation.Core;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Tasks;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using Nop.Plugin.NopStation.Core.Helpers;
using Nop.Services.Cms;
using System.IO;
using System.Text;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.NopStation.AppPushNotification
{
    public class AppPushNotificationPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin, INopStationPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IRepository<AppPushNotificationTemplate> _pushNotificationTemplateRepository;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly INopDataProvider _nopDataProvider;
        private readonly INopFileProvider _nopFileProvider;

        public bool HideInWidgetList => true;

        #endregion

        #region Ctor

        public AppPushNotificationPlugin(ILocalizationService localizationService,
            IWebHelper webHelper,
            IRepository<AppPushNotificationTemplate> pushNotificationTemplateRepository,
            IScheduleTaskService scheduleTaskService,
            IPermissionService permissionService,
            INopStationCoreService nopStationCoreService,
            INopDataProvider nopDataProvider,
            INopFileProvider nopFileProvider)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
            _pushNotificationTemplateRepository = pushNotificationTemplateRepository;
            _scheduleTaskService = scheduleTaskService;
            _permissionService = permissionService;
            _nopStationCoreService = nopStationCoreService;
            _nopDataProvider = nopDataProvider;
            _nopFileProvider = nopFileProvider;
        }

        #endregion

        #region Utilities

        protected virtual void ExecuteSqlFile(string path)
        {
            var statements = new List<string>();

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                    statements.Add(statement);
            }

            foreach (var stmt in statements)
                _nopDataProvider.ExecuteNonQuery(stmt);
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        protected void InsertInitialData()
        {
            var scheduleTaskForNotification = _scheduleTaskService.GetTaskByType("Nop.Plugin.NopStation.AppPushNotification.QueuedAppPushNotificationSendTask, Nop.Plugin.NopStation.AppPushNotification");
            var scheduleTaskForCheckCampaign = _scheduleTaskService.GetTaskByType("Nop.Plugin.NopStation.AppPushNotification.AppPushNotificationCampaignSendTask, Nop.Plugin.NopStation.AppPushNotification");

            if (scheduleTaskForCheckCampaign == null)
            {
                _scheduleTaskService.InsertTask(new ScheduleTask()
                {
                    Enabled = true,
                    Name = "Check notification campaign",
                    Seconds = 60,
                    Type = "Nop.Plugin.NopStation.AppPushNotification.AppPushNotificationCampaignSendTask, Nop.Plugin.NopStation.AppPushNotification",
                });
            }

            if (scheduleTaskForNotification == null)
            {
                _scheduleTaskService.InsertTask(new ScheduleTask()
                {
                    Enabled = true,
                    Name = "Send push notification",
                    Seconds = 60,
                    Type = "Nop.Plugin.NopStation.AppPushNotification.QueuedAppPushNotificationSendTask, Nop.Plugin.NopStation.AppPushNotification",
                });
            }

            var messageTemplates = new List<AppPushNotificationTemplate>
            {
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.CustomerEmailValidationNotification,
                    Title = "%Store.Name%. Email validation",
                    Body = $"%Store.Name%, {Environment.NewLine}Check your email to activate your account. {Environment.NewLine}%Store.Name%",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.CustomerRegisteredWelcomeNotification,
                    Title = "Welcome to %Store.Name%",
                    Body = $"We welcome you to %Store.Name%.{Environment.NewLine}You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{Environment.NewLine}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{Environment.NewLine}Order History - View your history of purchases that you have made with us.{Environment.NewLine}Products Reviews - Share your opinions on products with our other customers.",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.CustomerWelcomeNotification,
                    Title = "Notification subscrition success",
                    Body = $"You have successfully subscribed for notification. {Environment.NewLine}%Store.Name%",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.CustomerRegisteredNotification,
                    Title = "%Store.Name%. New customer registration",
                    Body = $"%Store.Name%, {Environment.NewLine}A new customer registered with your store. Below are the customer's details:{Environment.NewLine}Full name: %Customer.FullName%{Environment.NewLine}Email: %Customer.Email%.",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.OrderCancelledCustomerNotification,
                    Title = "%Store.Name%. Your order cancelled",
                    Body = $"%Store.Name%, {Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}Your order has been cancelled. Below is the summary of the order.{Environment.NewLine}Order Number: %Order.OrderNumber%.",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.OrderCompletedCustomerNotification,
                    Title = "%Store.Name%. Your order completed",
                    Body = $"%Store.Name%, {Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}Your order has been completed. Below is the summary of the order.{Environment.NewLine}Order Number: %Order.OrderNumber%.",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.ShipmentDeliveredCustomerNotification,
                    Title = "Your order from %Store.Name% has been delivered.",
                    Body = $" %Store.Name%, {Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}Good news! You order has been delivered.{Environment.NewLine}Order Number: %Order.OrderNumber%.",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.OrderPlacedCustomerNotification,
                    Title = "Order receipt from %Store.Name%.",
                    Body = $"%Store.Name%, {Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}Thanks for buying from %Store.Name%. Order Number: %Order.OrderNumber%.",
                    Active = true,
                    ActionType = NotificationActionType.Order,
                    ActionValue = "%Order.OrderNumber%",
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.ShipmentSentCustomerNotification,
                    Title = "Your order from %Store.Name% has been shipped.",
                    Body = $" %Store.Name%, {Environment.NewLine}Hello %Order.CustomerFullName%!,{Environment.NewLine}Good news! You order has been shipped.{Environment.NewLine}Order Number: %Order.OrderNumber%",
                    Active = true,
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.OrderRefundedCustomerNotification,
                    Title = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = $"%Store.Name%, {Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}Thanks for buying from %Store.Name%. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.",
                    Active = false,
                    ActionType = NotificationActionType.Order,
                    ActionValue = "%Order.OrderNumber%",
                    SendImmediately = true
                },
                new AppPushNotificationTemplate
                {
                    Name = AppPushNotificationTemplateSystemNames.OrderPaidCustomerNotification,
                    Title = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"%Store.Name%, {Environment.NewLine}Hello %Order.CustomerFullName%,{Environment.NewLine}Thanks for buying from %Store.Name%. Order #%Order.OrderNumber% has been just paid. Order Number: %Order.OrderNumber%.",
                    Active = false,
                    ActionType = NotificationActionType.Order,
                    ActionValue = "%Order.OrderNumber%",
                    SendImmediately = true
                }
            };
            _pushNotificationTemplateRepository.Insert(messageTemplates);
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AppPushNotification/Configure";
        }

        public override void Install()
        {
            var dataSettings = DataSettingsManager.LoadSettings();

            if (dataSettings != null)
            {
                if (dataSettings.DataProvider == DataProviderType.SqlServer)
                {
                    var path = _nopFileProvider.MapPath("~/Plugins/NopStation.AppPushNotification/sql/mssql_sp.sql");
                    ExecuteSqlFile(path);
                }
                else if (dataSettings.DataProvider == DataProviderType.MySql)
                {
                    var path = _nopFileProvider.MapPath("~/Plugins/NopStation.AppPushNotification/sql/mysql_sp.sql");
                    ExecuteSqlFile(path);
                }
            }

            this.NopStationPluginInstall(new AppPushNotificationPermissionProvider());
            InsertInitialData();
            base.Install();
        }

        public override void Uninstall()
        {
            this.NopStationPluginUninstall(new AppPushNotificationPermissionProvider());
            _nopDataProvider.ExecuteNonQuery("DROP PROCEDURE NS_AppNotificationDeviceLoadAllPaged;");

            var scheduleTaskForNotification = _scheduleTaskService.GetTaskByType("Nop.Plugin.NopStation.AppPushNotification.QueuedPushNotificationSendTask, Nop.Plugin.NopStation.AppPushNotification");
            var scheduleTaskForCheckCampaign = _scheduleTaskService.GetTaskByType("Nop.Plugin.NopStation.AppPushNotification.CampaignSendTask, Nop.Plugin.NopStation.AppPushNotification");

            if (scheduleTaskForCheckCampaign != null)
                _scheduleTaskService.DeleteTask(scheduleTaskForCheckCampaign);
            if (scheduleTaskForNotification != null)
                _scheduleTaskService.DeleteTask(scheduleTaskForNotification);

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menu = new SiteMapNode()
            {
                Visible = true,
                IconClass = "fa-circle-o",
                Title = _localizationService.GetResource(
                    "Admin.NopStation.AppPushNotification.Menu.AppPushNotification")
            };

            #region Campaign

            if (_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageCampaigns))
            {
                var campaign = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Title = _localizationService.GetResource("Admin.NopStation.AppPushNotification.Menu.Campaigns"),
                    Url = "/Admin/AppPushNotificationCampaign/List",
                    SystemName = "AppPushNotificationCampaigns"
                };
                menu.ChildNodes.Add(campaign);
            }

            #endregion

            #region Template

            if (_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageTemplates))
            {
                var notificationTemplate = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Title = _localizationService.GetResource(
                        "Admin.NopStation.AppPushNotification.Menu.PushNotificationTemplates"),
                    Url = "/Admin/AppPushNotificationTemplate/List",
                    SystemName = "AppPushNotificationTemplates"
                };
                menu.ChildNodes.Add(notificationTemplate);
            }

            #endregion

            #region Queued notification

            if (_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageQueuedNotifications))
            {
                var queue = new SiteMapNode
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Title = _localizationService.GetResource(
                        "Admin.NopStation.AppPushNotification.Menu.QueuedPushNotifications"),
                    Url = "/Admin/AppQueuedPushNotification/List",
                    SystemName = "AppQueuedPushNotifications"
                };
                menu.ChildNodes.Add(queue);
            }

            #endregion

            #region Others

            if (_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageConfiguration))
            {
                var settings = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/Admin/AppPushNotification/Configure",
                    Title = _localizationService.GetResource("Admin.NopStation.AppPushNotification.Menu.Configuration"),
                    SystemName = "AppPushNotifications"
                };
                menu.ChildNodes.Add(settings);
            }

            #endregion

            if (_permissionService.Authorize(AppPushNotificationPermissionProvider.ManageConfiguration))
            {
                var documentation = new SiteMapNode()
                {
                    Title = _localizationService.GetResource("Admin.NopStation.Common.Menu.Documentation"),
                    Url = "https://www.nop-station.com/app-push-notification-v2-documentation",
                    Visible = true,
                    IconClass = "fa-genderless",
                    OpenUrlInNewTab = true
                };
                menu.ChildNodes.Add(documentation);

                _nopStationCoreService.ManageSiteMap(rootNode, menu, NopStationMenuType.Plugin);
            }
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.None", "None"),
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.Product", "Product"),
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.Category", "Category"),
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.Manufacturer", "Manufacturer"),
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.Vendor", "Vendor"),
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.Order", "Order"),
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.Topic", "Topic"),
                new KeyValuePair<string, string>("Enums.Nop.Plugin.NopStation.AppPushNotification.Domains.NotificationActionType.Account", "Account"),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Menu.AppPushNotification", "App push notification"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Menu.PushNotificationTemplates", "Notification templates"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Menu.Campaigns", "Campaigns"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Menu.QueuedPushNotifications", "Notification queue"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Menu.Configuration", "Configuration"),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Configuration.Fields.GoogleConsoleApiAccessKey", "Google console api access key"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Configuration.Fields.GoogleConsoleApiAccessKey.Hint", "The Google console api access key."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Configuration.Fields.ApplicationTypeId", "Application type"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Configuration.Fields.ApplicationTypeId.Hint", "Select mobile application type."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Configuration.Title", "App notification settings"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.Configuration.Updated", "App notification settings updated successfully."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.AddNew", "Add a new campaign"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.BackToList", "back to campaign list"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.EditDetails", "Edit campaign details"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List", "Campaigns"),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Name", "Name"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Name.Hint", "The name for this campaign."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Title", "Title"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Title.Hint", "The title for this campaign."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Body", "Body"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Body.Hint", "The template body."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ImageId", "Image"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ImageId.Hint", "The template image."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.CreatedOn", "Created on"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.CreatedOn.Hint", "The date when the campaign was created."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.SendingWillStartOn", "Sending will start on"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.SendingWillStartOn.Hint", "The date/time that the campaign will be sent."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.AddedToQueueOn", "Added to queue on"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.AddedToQueueOn.Hint", "The date/time that the campaign was added to queue."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.CustomerRoles", "Limited to customer roles"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.CustomerRoles.Hint", "Option to limit this campaign to a certain customer role. If you have multiple customer role, choose one or several from the list. If you don't use this option just leave this field empty."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.DeviceTypes", "Limited to device types"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.DeviceTypes.Hint", "Option to limit this campaign to a certain device type. If you don't use this option just leave this field empty."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.AllowedTokens", "Allowed notification tokens"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.AllowedTokens.Hint", "This is a list of the notification tokens you can use in your template"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ActionType", "Action type"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ActionType.Hint", "It determines which page will open in mobile app onclick the notification. (i.e. Product details, Category details)"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ActionValue", "Value"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ActionValue.Hint", "It determines the value of action type. (i.e. Product id, Category id)"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.LimitedToStoreId", "Limited to store"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.LimitedToStoreId.Hint", "Choose a store which subscribers will get this notification."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.IconId.Required", "The 'Icon' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Title.Required", "The 'Title' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Body.Required", "The 'Body' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.SendingWillStartOn.Required", "The 'Sending will start on' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Name.Required", "The 'Name' is required."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.CopyCampaign", "Copy campaign"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copy", "Copy campaign"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copied", "Campaign has been copied successfully."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copy.SendingWillStartOn", "Sending will start on"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copy.SendingWillStartOn.Hint", "The date/time that the new campaign will be sent."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copy.Name", "Name"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copy.Name.Hint", "The name for new campaign."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchKeyword", "Search keyword"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchKeyword.Hint", "Search campaign(s) by specific keywords."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchSendStartFromDate", "Send start from date"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchSendStartFromDate.Hint", "Search by send start from date."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchSendStartToDate", "Send start to date"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchSendStartToDate.Hint", "Search by send start from date."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.SendTestNotification", "Send test notification"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Created", "Campaign has been created successfully."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Updated", "Campaign has been updated successfully."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Deleted", "Campaign has been deleted successfully."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Name", "Name"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Name.Hint", "The template name."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Title", "Title"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Title.Hint", "The template title."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Body", "Body"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Body.Hint", "The template body."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ImageId", "Image"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ImageId.Hint", "The template image."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Active", "Active"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Active.Hint", "Check to active this template."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.AllowedTokens", "Allowed notification tokens"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.AllowedTokens.Hint", "This is a list of the notification tokens you can use in your template"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.LimitedToStores", "Limited to stores"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.LimitedToStores.Hint", "Option to limit this notification to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.SendImmediately", "Send immediately"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.SendImmediately.Hint", "Send notification immediately."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.DelayBeforeSend", "Delay before send"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.DelayBeforeSend.Hint", "A delay before sending the notification."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.CreatedOn", "Created on"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.CreatedOn.Hint", "The date/time that the notification was created."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ActionType", "Action type"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ActionType.Hint", "It determines which page will open in mobile app onclick the notification. (i.e. Product details, Category details)"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ActionValue", "Value"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ActionValue.Hint", "It determines the value of action type. (i.e. Product id, Category id)"),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchKeyword", "Search keyword"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchKeyword.Hint", "Search template(s) by specific keywords."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchActiveId", "Active"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchActiveId.Hint", "Search by a \"Active\" property."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchTemplateTypeId", "Template type"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchTemplateTypeId.Hint", "Search by a template type."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Tabs.Info", "Info"),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.AddNew", "Add a new template"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.BackToList", "back to template list"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.EditDetails", "Edit template details"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List", "Templates"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Title.Required", "The 'Title' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Name.Required", "The 'Name' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.IconId.Required", "The 'Icon' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.DelayBeforeSend.Required", "The 'Delay before send' is required."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.DelayBeforeSend.GreaterThanZero", "The 'Delay before send' must be greater than zero."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchActiveId.Active", "Active"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchActiveId.Inactive", "Inactive"),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Customer", "Customer"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Customer.Hint", "The customer."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Store", "Store"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Store.Hint", "A store name in which this notification was sent."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Title", "Title"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Title.Hint", "The notification title."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Body", "Body"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Body.Hint", "The notification body."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ImageUrl", "Image"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ImageUrl.Hint", "The notification image."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.CreatedOn", "Created on"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.CreatedOn.Hint", "The date/time that the notification was created."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SentOn", "Sent on"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SentOn.Hint", "The date/time that the notification was sent."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.DontSendBeforeDate", "Dont send before date"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.DontSendBeforeDate.Hint", "Hint"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SentOn.NotSent", "Not sent"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ActionType", "Action type"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ActionType.Hint", "It determines which page will open in mobile app onclick the notification. (i.e. Product details, Category details)"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ActionValue", "Value"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ActionValue.Hint", "It determines the value of action type. (i.e. Product id, Category id)"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SubscriptionId", "Subscription id"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SubscriptionId.Hint", "The subscription id."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SubscriptionId", "Subscription id"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SubscriptionId.Hint", "The subscription id."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SendImmediately", "Send immediately"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SendImmediately.Hint", "Send notification immediately after adding into queue."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SentTries", "Sent tries"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SentTries.Hint", "Number of attempts to send this notification."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.DeviceType", "Device type"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.DeviceType.Hint", "The type of the device."),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.EditDetails", "Edit queued notification details"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.BackToList", "back to queued notification list"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Requeue", "Requeue"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.List", "Notification queue"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.ViewDetails", "View details"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.All", "All"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Unknown", "Unknown"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Guest", "Guest"),

                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Updated", "Push notification campaign has been updated successfully."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.SendTestNotification", "Send test notification"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.SendTestNotification.Confirmation", "Are you sure want to send test notification?"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Deleted", "Queued push notification has been deleted successfully."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.TestNotification.Title", "Hello {0}"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.TestNotification.Guest", "Guest"),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.TestNotification.Body", "This is a test notification from {0}."),
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.TestNotification.SentSuccessFully", "Test notification has been sent successfully."),
                
                new KeyValuePair<string, string>("Admin.NopStation.AppPushNotification.QueuedPushNotifications.DeleteSent", "Delete (all sent)")
            };

            return list;
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "admin_webapidevice_list_buttons" };
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "AppPushNotification";
        }

        #endregion
    }
}