SELECT 
	po.CustomerId 
	, pi2.InvoiceTo 
	, pi2.RefType 
	, pi2 .InvoiceNumber
	, pi2.CreatedOnUTC 
	, pi2.Id  
FROM ProInvoice pi2 
INNER JOIN ProOrderItem poi
ON poi.InvoiceId = pi2.Id 
AND pi2.RefType IN (0,1)
INNER JOIN ProOrder po 
ON po.Id  = poi.OrderId 
ORDER BY 
	pi2.CreatedOnUTC DESC;

START TRANSACTION;
UPDATE ProInvoice pi2 
INNER JOIN ProOrderItem poi
ON poi.InvoiceId = pi2.Id 
AND pi2.RefType IN (0,1)
INNER JOIN ProOrder po 
ON po.Id  = poi.OrderId 
SET pi2.InvoiceTo = po.CustomerId 
	, pi2.RefType  = 1
WHERE pi2.InvoiceTo = 0
AND pi2.RefType = 0;
COMMIT