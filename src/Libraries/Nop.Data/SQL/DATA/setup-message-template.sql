START TRANSACTION;

DROP TEMPORARY TABLE IF EXISTS `tmpMessageTemplate`;

CREATE TEMPORARY TABLE `tmpMessageTemplate`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Email` varchar(200) NOT NULL
  ,`Name` varchar(200) NOT NULL
  ,`Subject` varchar(1000) NOT NULL
  ,`Body` longtext NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpMessageTemplate` 
(`Email`, `Name`,`Subject`, `Body`) VALUES 
('noreply@yadiyad.com' ,'ProductPublish.ApprovalRequest'	,'%Store.Name%. Product Publish Request - Approval Required'	,
'"%Product.Name%" required product publish approval.
<br />
To go to link <a href="%Product.ProductURLForOperator%">click here</a>.')
,('noreply@yadiyad.com' ,'ProductPublish.Approved'	,'%Store.Name%. Product Publish Request - Approved'	,
'"%Product.Name%" publish request has been approved.
<br />
To go to link <a href="%Product.ProductURLForOperator%">click here</a>.')
,('noreply@yadiyad.com' ,'ProductPublish.Rejected'	,'%Store.Name%. Product Publish Request - Rejected'	,
'"%Product.Name%" publish request has been rejected.
<br />
To go to link <a href="%Product.ProductURLForOperator%">click here</a>.')
,('noreply@yadiyad.com' ,'VendorAccountApply.StoreOwnerNotification'	,'%Store.Name% New vendor account submitted.'	,
'<p>
%Customer.FullName% (%Customer.Email%) has just submitted for a vendor account. Details are below:
<br />
Vendor name: %Vendor.Name%
<br />
Vendor email: %Vendor.Email%
<br />
<br />
You can activate it in admin area.
</p>
')
,('noreply@yadiyad.com' ,'VendorAccountApprove.StoreOwnerNotification'	,'%Store.Name% Vendor Application - Appproved'	,
'<p>
Your vendor application has been approved. Details are below:
<br />
Vendor name: %Vendor.Name%
<br />
Vendor email: %Vendor.Email%
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'VendorAccountReject.StoreOwnerNotification'	,'%Store.Name% Vendor Application - Rejected'	,
'<p>
Your vendor application has been rejected. Details are below:
<br />
Vendor name: %Vendor.Name%
<br />
Vendor email: %Vendor.Email%
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderDisputeRaised.CustomerNotification'	,'Seller raised dispute'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
The seller has raised dispute for the return request for order %Order.OrderId%.
<br />
Our Dispute Settlement team will investigate further and might reach out if further clarifications needed.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderReturnApprove.CustomerNotification'	,'Return Request Approved'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
The seller has approved your return request for order %Order.OrderId%,
<br />
kindly return the item through our logistic partner within %OrderSettings.DaysForBuyerToShipOrder% days.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderRefundApprove.CustomerNotification'	,'Refund Approved'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
The seller has approved your refund request for order %Order.OrderId%, the refund will take up to 14 days for processing.
<br />
Remember to update your bank information for the refund to be credited to your bank account.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderDisputeOutcomeNoRefund.CustomerNotification'	,'Refund Rejected for Return Request'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
Our dispute settlement team have rejected the refund for your order %Order.OrderId%.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderDisputeOutcomeFullRefund.CustomerNotification'	,'Full Refund Approved for Return Request'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
Our dispute settlement team have approved the full refund of %Dispute.PartialAmount% for your order %Order.OrderId%.
<br />
<br />
Remember to update your bank information for the refund to be credited to your bank account.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderDisputeOutcomeFullRefundAndReturn.CustomerNotification'	,'Full Refund Approved for Return Request With Return of Product(s)'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
Our dispute settlement team have approved the full refund of %Dispute.PartialAmount% for your order %Order.OrderId%,
<br />
kindly return the item(s) through our logistic partner within %OrderSettings.DaysForBuyerToShipOrder% days.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderDisputeOutcomePartialRefundAndReturn.CustomerNotification'	,'Partial Refund Approved for Return Request With Return of Product(s)'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
Our dispute settlement team have approved the partial refund of %Dispute.PartialAmount% for your order %Order.OrderId%,
<br />
kindly return the item(s) through our logistic partner within %OrderSettings.DaysForBuyerToShipOrder% days.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderDisputeOutcomePartialRefund.CustomerNotification'	,'Partial Refund Approved for Return Request'	,
'<p>
Hi %Order.CustomerFullName%,
<br />
Our dispute settlement team have approved the partial refund of %Dispute.PartialAmount% for your order %Order.OrderId%.
<br />
Remember to update your bank information for the refund to be credited to your bank account.
<br />
<br />
</p>
')
,('noreply@yadiyad.com' ,'OrderPreparing.CustomerNotification'	,'Seller started preparing your order'	,
'<p>
<br />
<br />
Hi %Order.CustomerFullName%,
<br />
<br />
The seller is preparing your order %Order.OrderId%
<br />
<br />
Yours Sincerely.
<br />
Yadiyad
</p>
')
,('noreply@yadiyad.com' ,'NewPayout.VendorNotification'	,'New Payout received'	,
'<p>
<br />
<br />
Hi %Vendor.Name%,
<br />
<br />
Order Id: %Order.OrderId%.
<br />
Payout Amount: %OrderPayoutRequest.Price%
<br />
Payout Date: %OrderPayoutRequest.CreatedOn%
<br />
<br />
Yours Sincerely.
<br />
Yadiyad
</p>
')
,('noreply@yadiyad.com' ,'ReminderArrangeDriverRequestNow.VendorNotification'	,'Reminder Arrange Driver'	,
'<p>
<br />
<br />
Hi %Vendor.Name%,
<br />
<br />
Order Id: %Order.OrderId%.
<br />
Delivery Date: %Order.DeliveryDate%
<br />
Delivery Time: %Order.DeliveryTime%
<br />
<br />
Yours Sincerely.
<br />
Yadiyad
</p>
')
,('noreply@yadiyad.com' ,'UnableToLocateDriver.VendorNotification'	,'Unable to Locate Driver for Pick Up'	,
'<p>
<br />
<br />
Hi %Vendor.Name%,
<br />
<br />
We are unable to locate driver for Order %Order.OrderId%.
<br />
Please try again.
<br />
<br />
Yours Sincerely,
<br />
Yadiyad
</p>
')
,('noreply@yadiyad.com' ,'QuantityBelow.VendorNotification'	,'%Product.Name% - Low quantity notification'	,
'<p>
<br />
<br />
Hi %Vendor.Name%,
<br />
<br />
%Product.Name% (ID: %Product.ID%) low quantity.
<br />
Quantity: %Product.StockQuantity%
<br />
<br />
Yours Sincerely.
<br />
Yadiyad
</p>
')
,('noreply@yadiyad.com' ,'QuantityBelow.AttributeCombination.VendorNotification' ,'%Product.Name% - Low quantity notification'	,
'<p>
<br />
<br />
Hi %Vendor.Name%,
<br />
<br />
%Product.Name% (ID: %Product.ID%) low quantity.
<br />
%AttributeCombination.Formatted%
<br />
Quantity: %AttributeCombination.StockQuantity%
<br />
<br />
Yours Sincerely.
<br />
Yadiyad
</p>
')
;

INSERT INTO MessageTemplate
(Name, BccEmailAddresses, Subject, EmailAccountId, Body, IsActive, DelayBeforeSend, DelayPeriodId, AttachedDownloadId, LimitedToStores)
select tmt.Name, NULL,tmt. `Subject`, ea.Id, tmt.`Body`, 1, NULL, 0, 0, 0 
from `tmpMessageTemplate` tmt 
INNER JOIN EmailAccount ea 
ON ea.Email  = tmt.`Email`
where NOT EXISTS
	(SELECT 0
	FROM `MessageTemplate` mt
	where tmt.Name = mt.Name 
	limit 1);

-- UPDATE MessageTemplate mt
-- INNER JOIN `tmpMessageTemplate` tmt 
-- ON mt.Name  = tmt.Name
-- SET mt.Subject = tmt.Subject
-- , mt.Body  = tmt.Body;


DROP TABLE IF EXISTS `tmpMessageTemplate`;

COMMIT;


