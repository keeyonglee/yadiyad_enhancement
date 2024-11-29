START TRANSACTION;

DROP TEMPORARY TABLE IF EXISTS `tmpNotificationTemplate`;

CREATE TABLE `tmpNotificationTemplate` (
    `ImageId` int(11) NOT NULL,
    `Active` tinyint(1) NOT NULL,
    `LimitedToStores` tinyint(1) NOT NULL,
    `SendImmediately` tinyint(1) NOT NULL,
    `DelayBeforeSend` int(11) DEFAULT NULL,
    `DelayPeriodId` int(11) NOT NULL,
    `ActionTypeId` int(11) NOT NULL,
    `ActionValue` varchar(255) DEFAULT NULL,
    `Name` longtext DEFAULT NULL UNIQUE,
    `Title` longtext DEFAULT NULL,
    `Body` longtext DEFAULT NULL
);

/* Action Type ID
 * 
    None = 0,
    Product = 10,
    Category = 20,
    Manufacturer = 30,
    Vendor = 40,
    Order = 50,
    Topic = 60,
    Account = 70
 */


INSERT INTO `tmpNotificationTemplate`(ImageId,Active,LimitedToStores,SendImmediately,DelayBeforeSend,DelayPeriodId,ActionTypeId,ActionValue,Name,Title,Body) VALUES
(0,1,0,1,NULL,0,50,'%Order.OrderId%','OrderPreparing.CustomerNotification','Seller started preparing your order',
'Hi %Order.CustomerFullName%,
The seller is preparing your order %Order.OrderId%'),

(0,1,0,1,NULL,0,50,'%Order.OrderId%','OrderReturnApprove.CustomerNotification','Return Request Approved',
'Hi %Order.CustomerFullName%,
The seller has approved your return request for order %Order.OrderId%, kindly return the item through our logistic partner within %OrderSettings.DaysForBuyerToShipOrder% days.'),

(0,1,0,1,NULL,0,50,'%Order.OrderId%','OrderRefundApprove.CustomerNotification','Refund Approved',
'Hi %Order.CustomerFullName%,
The seller has approved your refund request for order %Order.OrderId%, the refund will take up to 14 days for processing. Remember to update your bank information for the refund to be credited to your bank account.'),

(0,1,0,1,NULL,0,50,'%Order.OrderId%','OrderDisputeRaised.CustomerNotification','Seller raised dispute',
'Hi %Order.CustomerFullName%,
The seller has raised dispute for the return request for order %Order.OrderId%. Our Dispute Settlement team will investigate further and might reach out if further clarifications needed.'),

(0,1,0,1,NULL,0,50,'%Order.OrderId%','OrderDisputeOutcome.CustomerNotification','Partial Refund Approved for Return Request',
'Hi %Order.CustomerFullName%,
Our dispute settlement team have approved the refund of %Dispute.PartialAmount% for your order %Order.OrderId%'),

(0,1,0,1,NULL,0,50,'%Order.OrderId%','UnableToLocateDriver.VendorNotification','Unable to Locate Driver for Pick Up',
'Hi %Vendor.Name%,
We are unable to locate driver for Order %Order.OrderId%.
Please try again.'),

(0,1,0,1,NULL,0,10,'%Product.ID%','QuantityBelow.VendorNotification','%Product.Name% - Low quantity notification',
 'Hello %Vendor.Name%,
%Product.Name% (ID: %Product.ID%) low quantity
Quantity: %Product.StockQuantity%'),

(0,1,0,1,NULL,0,10,'%Product.ID%','QuantityBelow.AttributeCombination.VendorNotification','%Product.Name% - Low quantity notification',
 'Hello %Vendor.Name%,
%Product.Name% (ID: %Product.ID%) low quantity
%AttributeCombination.Formatted%
Quantity: %AttributeCombination.StockQuantity%')
;


INSERT INTO NS_AppPushNotificationTemplate
(ImageId, Active, LimitedToStores, SendImmediately, DelayBeforeSend, DelayPeriodId, ActionTypeId, ActionValue, Name, Title, Body)
SELECT ImageId, Active, LimitedToStores, SendImmediately, DelayBeforeSend, DelayPeriodId, ActionTypeId, ActionValue, Name, Title, Body
from `tmpNotificationTemplate` tmt
where NOT EXISTS
    (SELECT 0
     FROM `NS_AppPushNotificationTemplate` mt
     where tmt.Name = mt.Name);

DROP TABLE IF EXISTS `tmpNotificationTemplate`;

COMMIT;



