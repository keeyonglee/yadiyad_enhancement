
DROP TEMPORARY TABLE IF EXISTS `tmpRunningNumber`;

CREATE TEMPORARY TABLE `tmpRunningNumber`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,`LastId`INT NOT NULL
  ,`LastYear` INT NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpRunningNumber` 
(`Id`, `Name`,`LastId`,`LastYear`) VALUES 
(1, 'Refund', 0, 2021)
;

INSERT INTO `RunningNumber`
(`Id`, `Name`,`LastId`,`LastYear`)
select `Id`, `Name`,`LastId`,`LastYear`
from `tmpRunningNumber` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM `RunningNumber` lsr
	where tlsr.Id = lsr.Id 
	limit 1);


DROP TABLE IF EXISTS `tmpRunningNumber`;


