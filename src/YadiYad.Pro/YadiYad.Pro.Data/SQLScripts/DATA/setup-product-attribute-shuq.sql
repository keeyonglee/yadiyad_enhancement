
DROP TEMPORARY TABLE IF EXISTS `tmpProductAttribute`;

CREATE TEMPORARY TABLE `tmpProductAttribute`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpProductAttribute` 
(`Name`) VALUES 
('Portion Sizing'),
('Spicy'),
('Select Delivery')
;

INSERT INTO `ProductAttribute`
(Name)
select `Name`
from `tmpProductAttribute` tpa
where NOT EXISTS
	(SELECT 0
	FROM `ProductAttribute` pa
	where tpa.Name = pa.Name 
	limit 1);

DROP TABLE IF EXISTS `tmpProductAttribute`;


