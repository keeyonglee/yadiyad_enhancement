
DROP TEMPORARY TABLE IF EXISTS `tmpSetting`;

CREATE TEMPORARY TABLE `tmpSetting`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,`Value` varchar(1000) NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpSetting` 
(`Name`,`Value`) VALUES 
('YadiYadProBankAccountSettings.BankName', 'Maybank' ),
('YadiYadProBankAccountSettings.BankAccountNo', '1234-5678-90' ),
('PayoutRequestSettings.AutoApprovalDays', '3' ),
('ProHomeSettings.InfoPrimaryVideoUrl', 'https://www.youtube.com/embed/tK-oN19dNFE?autoplay=1&mute=1&rel=0'),
('ProEngagementSettings.DaysAbleToCancel', '5'),
('ProEngagementSettings.DaysMinActivePayToViewInviteOnCancellation', '15'),
('PayoutRequestSettings.CsvMaxRecord', '99'),
('PayoutRequestSettings.CsvMaxTotalAmount', '150000'),
('ShippingJntSettings.TrackingUrl', 'https://www.jtexpress.my/tracking/'),
('ShippingLalamoveSettings.ApiKey', 'pk_test_bd6243c6c727a6b6ed6fc55f63d2f884'),
('ShippingLalamoveSettings.ApiSecret', 'sk_test_GPfIJJk4wYTAPoT/N9bGNhv1OqIGhF2fPi+3e+fDgNO454ioMCbdB5/VgY33NzwC'),
('ShippingLalamoveSettings.BaseUrl', 'https://rest.sandbox.lalamove.com'),
('ShippingLalamoveSettings.TotalFeeCurrency', 'MYR'),
('ShippingLalamoveSettings.GetQuotation', '/v2/quotations'),
('ShippingLalamoveSettings.PlaceOrder', '/v2/orders'),
('ShippingJntSettings.OrderKey', 'AKe62df84bJ3d8e4b1hea2R45j11klsb'),
('ShippingJntSettings.TrackingKey', 'ffe62df84bb3d8e4b1eaa2c22775014d'),
('ShippingJntSettings.BaseUrl', 'http://47.57.89.30'),
('ShippingJntSettings.Username', 'TEST'),
('ShippingJntSettings.ApiKey', 'TES123'),
('ShippingJntSettings.CustomerCode', 'ITTEST0001'),
('ShippingJntSettings.MessageType', 'TRACK'),
('ShippingJntSettings.ECompanyId', 'TEST'),
('ShippingJntSettings.CreateOrder', '/blibli/order/createOrder'),
('ShippingJntSettings.Tracking', '/common/track/trackings'),
('ShippingJntSettings.ConsignmentNote', '/jandt_web/print/facelistAction!print.action'),
('ShippingJntSettings.ServiceType', '6'),
('ShippingJntSettings.GoodsType', 'PARCEL'),
('ShippingJntSettings.PayType', 'PP_PM'),
('ShippingLalamoveSettings.Market', 'MY_KUL'),
('ConsultationJobSetting.InvitationAutoRejectAfterWorkingDays', '5'),
('ShippingLalamoveSettings.Motorcycle', 'MOTORCYCLE'),
('ShippingLalamoveSettings.Car', 'CAR'),
('ShippingSettings.IsShipMandatory', 'True'),
('AdminDashboardSettings.CategoryChartColourMapping', '#FF0000, #00FF00, #0000FF, #FFFF00, #00FFFF, #FF00FF, #C0C0C0, #FF6347, #FFD700, #9ACD32, #008B8B, #1E90FF, #4B0082, #FF1493, #D2691E, #B0C4DE, #000080, #008000, #808000, #800000, #FFA500, #6495ED, #DDA0DD, #F5DEB3, #DEB887, #7FFFD4, #DC143C, #7CFC00, #4B0082, #FFE4E1'),
('ShippingLalamoveSettings.CancelOrder', '/v2/orders/{0}/cancel'),
('OrderSettings.DaysForSellerToRespondReturnRequest', '5'),
('OrderSettings.DaysForSellerToShipOrder', '30'),
('OrderSettings.DaysForBuyerToShipOrder', '5'),
('OrderSettings.DaysForSellerToCompleteInspection', '3'),
('ShippingSettings.MaxAutoRetries','3'),
('MediaSettings.MaxReviewImages','5'),
('ShippingLalamoveSettings.CarCoverageLimit', '30'),
('ShippingLalamoveSettings.BikeCoverageLimit', '25'),
('ProIndividualTourSettings.SetDelay', '5000')
;

INSERT INTO `Setting`
(Name, Value, StoreId)
select `Name`, `Value`, 0
from `tmpSetting` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM `Setting` lsr
	where tlsr.Name = lsr.Name 
	limit 1);

/*
SELECT *
FROM `tmpSetting`
ORDER BY `Name`;
*/

DROP TABLE IF EXISTS `tmpSetting`;


