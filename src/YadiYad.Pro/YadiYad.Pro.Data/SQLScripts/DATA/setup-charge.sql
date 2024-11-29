
DROP TEMPORARY TABLE IF EXISTS `tmpCharge`;

CREATE TEMPORARY TABLE `tmpCharge`(
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProductTypeId` int(11) NOT NULL,
  `SubProductTypeId` int(11) NOT NULL,
  `ValidityDays` int(11) NOT NULL,
  `ValueType` int(11) NOT NULL,
  `Value` decimal(19,5) NOT NULL,
  `StartDate` datetime NOT NULL,
  `EndDate` datetime NULL,
  `IsActive` tinyint(1) NOT NULL,
  `Deleted` tinyint(1) NOT NULL DEFAULT 0,
  `MinRange` decimal(10,2) DEFAULT NULL,
  `MaxRange` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`Id`)
);

INSERT INTO `tmpCharge` 
(`ProductTypeId`,`SubProductTypeId`,`ValidityDays`,`ValueType`,`Value`,`StartDate`,`EndDate`,`IsActive`,`Deleted`, `MinRange`, `MaxRange`) VALUES 
(1,0,30,1,38.00000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(2,1,30,2,0.20000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(2,2,30,2,0.20000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(2,3,30,2,0.15000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(3,0,0,2,0.50000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(4,1,0,2,0.15000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(4,2,0,2,0.15000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(4,3,0,2,0.15000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(4,4,0,2,0.15000,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(41,0,0,2,0.1,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(51,0,0,2,0.1,"2021-01-01 00:00:00",null,1,0,null,null), -- 
(7,0,0,2,0.5,"2021-01-01 00:00:00",null,1,0,null,null), -- Moderator payout rate if consultation session completed
(7,1,0,2,0.5,"2021-01-01 00:00:00",null,1,0,null,null), -- Moderator payout rate if consultation session cancalled by buyer less than 24 hours
(7,2,0,2,0.15,"2021-01-01 00:00:00",null,1,0,null,null), -- Moderator payout rate if consultation session cancalled by buyer more than 24 hours and less than 72 hours
(7,3,0,2,0.15,"2021-01-01 00:00:00",null,1,0,null,null), -- Moderator payout rate if consultation session cancalled by buyer more than 72w hours
(7,4,0,2,0.05,"2021-01-01 00:00:00",null,1,0,null,null), -- Moderator payout rate if consultation session cancalled by seller
(32,0,0,2,0.05,"2021-01-01 00:00:00",null,1,0,null,null), -- Consultation fee escrow charge
(33,3,0,2,0.10,"2021-01-01 00:00:00",null,1,0,null,null), -- Consultation Buyer Cancellation Admin Charges more than 72 hours
(34,0,0,2,0.15,"2021-01-01 00:00:00",null,1,0,null,null), -- Consultation Engagement matching fee refund admin charges
(43,0,0,2,0.05,"2021-01-01 00:00:00",null,1,0,null,null), -- Service Buyer Cancellation Admin Charges
(53,0,0,2,0.15,"2021-01-01 00:00:00",null,1,0,null,null), -- Job Buyer Cancellation Admin Charges

-- for shuq
(0,0,0,2,0.12,"2021-01-01 00:00:00",null,1,0,0,10), -- tier1
(0,0,0,2,0.10,"2021-01-01 00:00:00",null,1,0,11,999999) -- tier2
;

INSERT INTO Charge
(`ProductTypeId`,`SubProductTypeId`,`ValidityDays`,`ValueType`,`Value`,`StartDate`,`EndDate`,`IsActive`,`Deleted`, `MinRange`, `MaxRange`, `CreatedById`, `UpdatedById`, `CreatedOnUTC`, `UpdatedOnUTC`)
select `ProductTypeId`,`SubProductTypeId`,`ValidityDays`,`ValueType`,`Value`,`StartDate`,`EndDate`,`IsActive`,`Deleted`, `MinRange`, `MaxRange`, 1, 1, NOW( ), NOW( )
from `tmpCharge` tc
WHERE NOT EXISTS
	(SELECT 1
	FROM Charge c
	WHERE c.ProductTypeId = tc.ProductTypeId
	AND c.SubProductTypeId = tc.SubProductTypeId
	LIMIT 1);

/*SELECT *
FROM `tmpCharge`;*/

DROP TABLE IF EXISTS `tmpCharge`;


