DROP TEMPORARY TABLE IF EXISTS `tmpSpecificationAttributeAndOptions`;

CREATE TEMPORARY TABLE `tmpSpecificationAttributeAndOptions`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`SAName` varchar(200) NOT NULL
  ,`SAOptionName` varchar(200) NOT NULL
  ,PRIMARY KEY (`Id`)		
);

INSERT INTO `tmpSpecificationAttributeAndOptions` 
(`SAName`, `SAOptionName`) VALUES 
('Dietary Description' ,'Muslim friendly'),
('Dietary Description' ,'Diabetic friendly'),
('Dietary Description' ,'Dairy free'),
('Dietary Description' ,'Lactose free'),
('Dietary Description' ,'Vegetarian'),
('Dietary Description' ,'Vegan'),
('Dietary Description' ,'Toddlers (1-4yrs) friendly'),
('Dietary Description' ,'Children (4-12yrs) friendly'),
('Dietary Description' ,'Low Salt'),
('Dietary Description' ,'Nuts Free '),
('Dietary Description' ,'Egg Free'),
('Dietary Description' ,'Corn Free'),
('Dietary Description' ,'Soy Free'),
('Dietary Description' ,'GMO Free '),
('Dietary Description' ,'Trans Fat Free'),
('Dietary Description' ,'No Herbs'),
('Dietary Description' ,'No Sugar Added'),
('Dietary Description' ,'Organic'),
('Dietary Description' ,'Alcohol Free'),
('Dietary Description' ,'Ketogenic'),
('Dietary Description' ,'Low Carb')
;

INSERT INTO SpecificationAttribute 
(`Name`, `DisplayOrder`)
SELECT x.`SAName`, ROW_NUMBER() over (order by x.`SAName` ASC) `DisplayOrder`
FROM (SELECT DISTINCT `SAName`
FROM `tmpSpecificationAttributeAndOptions` tsao
where NOT EXISTS
	(SELECT 0
	FROM SpecificationAttribute sa
	where tsao.`SAName` = sa.`Name` 
	limit 1)) x
order by 2;

INSERT INTO SpecificationAttributeOption
(Name, SpecificationAttributeId, DisplayOrder)
SELECT `SAOptionName`, sa.id, ROW_NUMBER() over (order by tsao.`SAName` ASC) 
FROM `tmpSpecificationAttributeAndOptions` tsao
INNER JOIN SpecificationAttribute sa
ON sa.Name = tsao.`SAName`
where NOT EXISTS
	(SELECT 0
	FROM SpecificationAttributeOption sao
	where tsao.`SAOptionName` = sao.Name 
	AND sao.SpecificationAttributeId  = sa.Id 
	limit 1)
ORDER BY tsao.`SAName`, tsao.`SAOptionName`;
	
DROP TEMPORARY TABLE IF EXISTS `tmpSpecificationAttributeAndOptions`;