
DROP TEMPORARY TABLE IF EXISTS `tmpSetting`;

CREATE TEMPORARY TABLE `tmpSetting`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,`Value` varchar(200) NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpSetting` 
(`Name`,`Value`) VALUES 
('catalogsettings.shuqeatproductsearchcoveragedistance', '5' ),
('VendorSettings.DefaultVendorPageSize', '12' ),
('VendorSettings.ShuqEatsCategoryId', '75' ),
('VendorSettings.ShuqMartCategoryId', '76' ),
('VendorSettings.ShuqEatsBusinessNatureVendorAttributeId', '1' ),
('VendorSettings.ShuqMartBusinessNatureVendorAttributeId', '2' ),
('ProductSetupSettings.DefaultAllowCustomerReviews', 'True' ),
('ShippingLalamoveSettings.MarkUpShippingFeePercentage', '0'),
('ShippingJntSettings.MarkUpShippingFeePercentage', '0.05'),
('storeinformationsettings.ShuqSellerGuidelineLink', 'http://tiny.cc/shuq-guideline'),
('storeinformationsettings.ShuqContactUsLink', 'https://www.yadiyad.com/shuq/contactus'),
('ShippingLalamoveSettings.MarketKL', 'MY_KUL'),
('ShippingLalamoveSettings.MarketJohorBahru', 'MY_JHB'),
('ShippingLalamoveSettings.MarketPenang', 'MY_NTL'),
('ShippingLalamoveSettings.AvailableStates01', 'Wilayah Persekutuan (KL), Selangor'),
('ShippingLalamoveSettings.AvailableStates02', 'Johor'),
('ShippingLalamoveSettings.AvailableStates03', 'Pulau Pinang'),
('ShoppingCartSettings.VendorMaxDeliveryDays', '7'),
('VendorNotificationSettings.ArrangeDriverRequestAdvanceMinutes', '60'),
('VendorNotificationSettings.ArrangeDriverRequestAdvanceHours', '48'),
('ShippingJntSettings.MaxWeight', '70'),
('ShippingLalamoveSettings.MaxWeightBike', '10'),
('ShippingLalamoveSettings.MaxWeightCar', '40'),
('ShippingJntSettings.ShipBeforeDateAdvanceDay', '3'),
('storeinformationsettings.shuqsellerappguidelinelink', 'tiny.cc/shuq-vendor-app-guideline'),
('PayoutBatchSettings.EndDateCutoff', '2'),
('CourierSettings.Eats', '2'),
('CourierSettings.Mart', '1'),
('ShippingBorzoSettings.BaseUrl', 'https://robotapitest-my.borzodelivery.com'),
('ShippingBorzoSettings.GetQuotation', '/api/business/1.2/calculate-order'),
('ShippingBorzoSettings.PlaceOrder', '/api/business/1.2/create-order'),
('ShippingBorzoSettings.GetOrders', '/api/business/1.2/orders'),
('ShippingBorzoSettings.ApiSecret', '539A5D1AF93F94A45A162DAFDD2B35263E3D3099'),
('ShippingBorzoSettings.Car', '7'),
('ShippingBorzoSettings.Motorcycle', '8'),
('ShippingBorzoSettings.MaxWeightCar', '40'),
('ShippingBorzoSettings.MaxWeightMotorcycle', '10'),
('ShippingBorzoSettings.MaxDeliveryFeeCar', '45'),
('ShippingBorzoSettings.MaxDeliveryFeeMotorcycle', '30'),
('ShippingBorzoSettings.Matter', 'Yadiyad Shuq Eats'),
('ShippingBorzoSettings.VendorUrl', 'VendorUrl'),
('ShippingBorzoSettings.CustomerUrl', 'CustomerUrl'),
('ShippingBorzoSettings.CallbackSecret', '2257731782BB5245E32F947A9FE84521ACBA7BC1'),
('ShippingBorzoSettings.IsClientNotificationEnabled', 'True'),
('ShippingBorzoSettings.IsContactPersonNotificationEnabled', 'True'),
('ShuqHomepageSettings.EatsMaxFeaturedProducts', '6'),
('ShuqHomepageSettings.MartMaxFeaturedProducts', '6')
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


