
DROP TEMPORARY TABLE IF EXISTS `tmpBusinessSegment`;

CREATE TEMPORARY TABLE `tmpBusinessSegment`(
  `BusinessSegmentName` varchar(200) NOT NULL
);

INSERT INTO `tmpBusinessSegment` 
(`BusinessSegmentName`) VALUES 
('Property'),
('Healthcare'),
('Education'),
('Consumer Products & Services'),
('Transportation & Logistics'),
('Utilities'),
('Energy'),
('Industrial Products & Services'),
('Technology'),
('Public Sector'),
('Telecommunications & Media'),
('Construction'),
('Plantation'),
('Financial Services'),
('Others')
;

INSERT INTO BusinessSegment
(`Name`)
select `BusinessSegmentName`
from `tmpBusinessSegment` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM BusinessSegment lsr
	where tlsr.BusinessSegmentName = lsr.Name 
	limit 1);

/*SELECT *
FROM `tmpBusinessSegment`
ORDER BY `BusinessSegmentName`;*/

DROP TABLE IF EXISTS `tmpBusinessSegment`;


