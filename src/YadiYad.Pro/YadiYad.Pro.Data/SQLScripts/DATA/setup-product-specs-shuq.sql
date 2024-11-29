DROP TEMPORARY TABLE IF EXISTS `tmpSpecificationAttribute`;

CREATE TEMPORARY TABLE `tmpSpecificationAttribute`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpSpecificationAttribute` 
(`Name`) VALUES 
('Colour'),
('Dimension'),
('Functionality'),
('Approximate Weight'),
('Fragile'),
('Other Information'),
('Materials'),
('Size'),
('Ingredients'),
('Advance Order'),
('Available In'),
('Product Description'),
('Way To Consume'),
('Storage Guideline'),
('Delivery Coverage'),
('Delivery Terms')
;

INSERT INTO `SpecificationAttribute`
(Name, DisplayOrder)
select `Name`, ROW_NUMBER()  OVER (Order By tsa.`Id` ASC)
from `tmpSpecificationAttribute` tsa
where NOT EXISTS
	(SELECT 0
	FROM `SpecificationAttribute` sa
	where tsa.Name = sa.Name 
	limit 1);

DROP TABLE IF EXISTS `tmpSpecificationAttribute`;



