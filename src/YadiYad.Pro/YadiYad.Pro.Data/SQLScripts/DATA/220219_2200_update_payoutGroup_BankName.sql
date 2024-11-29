
UPDATE PayoutGroup pg, Bank b
SET pg.BankName = b.ShortName
WHERE pg.BankName != ''
 AND pg.BankName = b.Name
 AND EXISTS (
	SELECT 0 FROM Bank b WHERE pg.BankName = b.Name LIMIT 1
 );