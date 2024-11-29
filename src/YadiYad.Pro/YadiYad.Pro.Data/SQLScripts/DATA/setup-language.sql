
DROP TEMPORARY TABLE IF EXISTS `tmpCommunicateLanguage`;

CREATE TEMPORARY TABLE `tmpCommunicateLanguage`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`CommunicateLanguageName` varchar(200) NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpCommunicateLanguage` 
(`CommunicateLanguageName`) VALUES 
('English'),
('Bahasa Malaysia'),
('Mandarin'),
('Japanese'),
('Korean'),
('Bahasa Indonesia'),
('Bengali'),
('Arabic'),
('Dutch'),
('Filipino'),
('French'),
('German'),
('Hebrew'),
('Hindi'),
('Italian'),
('Portuguese'),
('Russian'),
('Spanish'),
('Tamil'),
('Thai'),
('Vietnamese')
;

INSERT INTO `CommunicateLanguage`
(Name, DisplayOrder, Published)
select `CommunicateLanguageName`, ROW_NUMBER()  OVER (Order By tlsr.`Id` ASC), 1
from `tmpCommunicateLanguage` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM `CommunicateLanguage` lsr
	where tlsr.CommunicateLanguageName = lsr.Name 
	limit 1);

/*
SELECT *
FROM `tmpCommunicateLanguage`
ORDER BY `CommunicateLanguageName`;
*/

DROP TABLE IF EXISTS `tmpCommunicateLanguage`;


