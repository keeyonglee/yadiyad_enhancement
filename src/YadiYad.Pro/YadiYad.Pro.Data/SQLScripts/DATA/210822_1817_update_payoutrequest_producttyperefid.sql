UPDATE PayoutRequest pr 
INNER JOIN ProOrderItem poi 
ON poi.Id  = pr.OrderItemId 
SET 
	pr.ProductTypeId = poi.ProductTypeId 
	, pr.RefId  = poi.RefId 