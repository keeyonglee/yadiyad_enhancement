
DROP TEMPORARY TABLE IF EXISTS `tmpReason`;

CREATE TEMPORARY TABLE `tmpReason`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,`EngagementType` varchar(200) NOT NULL
  ,`Party` int(6) NOT NULL
  ,`BlameSeller` int(6) NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpReason` 
(`Name`,`EngagementType`,`Party`,`BlameSeller`) VALUES 
('Session No longer required - Penalise (Service Charge No Refund) and refer to fees structure', 3, 1 ,0),
('Found other consultant through other channels', 3, 1 ,0),
('Consultant no show', 3, 1 ,1),
('Others', 3, 1 ,0),
('Unavailable due to Emergency - Hire Again', 3, 2 ,0),
('Organization no show', 3, 2 ,0),
('Others', 3, 2 ,0),
('Job Applicant no show', 1, 1 ,1),
('Others', 1, 1 ,0),
('Organization no show', 1, 2 ,0),
('Others', 1, 2 ,0),
('Seller no show', 2, 1 ,1),
('Others', 2, 1 ,0),
('Buyer no show', 2, 2 ,0),
('Others', 2, 2 ,0),
('Job no longer required', 1, 1 ,0),
('Job no longer required', 1, 2 ,0),
('Service no longer required', 2, 1 ,0),
('Service no longer required', 2, 2 ,0)
;

INSERT INTO Reason
(`Name`, `EngagementType`, `Party`, `Published`, `OptionalRehireOrRefund`, `AllowedAfterStart`, `BlameSeller`)
select `Name`, `EngagementType`, `Party`, 1,0,0,0
from `tmpReason` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM Reason lsr
	where tlsr.EngagementType = lsr.EngagementType 
	and tlsr.Party= lsr.Party 
	and tlsr.Name= lsr.Name
	limit 1);

UPDATE Reason r
INNER JOIN `tmpReason` tlsr
ON tlsr.EngagementType = r.EngagementType 
and tlsr.Party= r.Party 
and tlsr.Name= r.Name
SET r.BlameSeller = tlsr.`BlameSeller`;

/*
SELECT *
FROM `tmpSetting`
ORDER BY `Name`;
*/

DROP TABLE IF EXISTS `tmpReason`;


