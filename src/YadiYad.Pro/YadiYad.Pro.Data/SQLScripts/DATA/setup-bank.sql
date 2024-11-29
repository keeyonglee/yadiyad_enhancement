
DROP TEMPORARY TABLE IF EXISTS `tmpBank`;

CREATE TEMPORARY TABLE `tmpBank`(
  `BankName` varchar(200) NOT NULL,
  `ShortName` varchar(200) NOT NULL
);

INSERT INTO `tmpBank` 
(`BankName`, `ShortName`) VALUES 
('Maybank', 'MAYBANK'),
('CIMB', 'CIMB'),
('Affin Bank', 'AFFIN'),
('RHB', 'RHB'),
('Hong Leong Bank', 'HONG LEONG'),
('HSBC Bank', 'HSBC'),
('AmBank', 'AMBANK'),
('Standard Chartered Bank', 'STANDARD CHARTERED'),
('Public Bank', 'PUBLIC BANK'),
('Alliance Bank', 'ALLIANCE'),
('Agro Bank', 'AGRO'),
('Bank Muamalat', 'MUAMALAT'),
('UOB', 'UOB'),
('OCBC Bank', 'OCBC')
;

INSERT INTO Bank
(`Name`, `ShortName`, `Published`, `DisplayOrder`)
select `BankName`, `ShortName`, 1, 1
from `tmpBank` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM Bank lsr
	where tlsr.BankName = lsr.Name 
	limit 1);

UPDATE Bank b, tmpBank tb
set b.ShortName = tb.ShortName
WHERE b.Name = tb.Bankname;

/*SELECT *
FROM `tmpBank`
ORDER BY `BankName`;*/

DROP TABLE IF EXISTS `tmpBank`;


