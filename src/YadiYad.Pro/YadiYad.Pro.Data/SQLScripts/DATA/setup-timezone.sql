
DROP TEMPORARY TABLE IF EXISTS `tmpTimzone`;

CREATE TEMPORARY TABLE `tmpTimzone`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,`Offset` INT NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpTimzone` 
(`Name`,`Offset`) VALUES 
('Malaysia', 8 )
;

INSERT INTO `TimeZone`
(Name, Offset)
select `Name`, `Offset`
from `tmpTimzone` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM `TimeZone` lsr
	where tlsr.Name = lsr.Name 
	limit 1);

/*
SELECT *
FROM `tmpTimzone`
ORDER BY `Name`;
*/

DROP TABLE IF EXISTS `tmpTimzone`;


