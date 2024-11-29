
DROP TEMPORARY TABLE IF EXISTS `tmpInterestHobby`;

CREATE TEMPORARY TABLE `tmpInterestHobby`(
  `InterestHobbyName` varchar(200) NOT NULL
);

INSERT INTO `tmpInterestHobby` 
(`InterestHobbyName`) VALUES 
('Arts & Crafts'),
('Autos & Vehicles'),
('Beauty & Wellness'),
('Books & Literature'),
('Business'),
('Computers & Electronics'),
('Finance'),
('Food & Drink'),
('Games'),
('Health & Fitness'),
('Home & Garden'),
('Investment'),
('Retirement Planning'),
('Sports'),
('Technology'),
('Travel & Leisure')
;

INSERT INTO InterestHobby
(`Name`)
select `InterestHobbyName`
from `tmpInterestHobby` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM InterestHobby lsr
	where tlsr.InterestHobbyName = lsr.Name 
	limit 1);

/*
SELECT *
FROM `tmpInterestHobby`
ORDER BY `InterestHobbyName`;
*/

DROP TABLE IF EXISTS `tmpInterestHobby`;


