UPDATE Vendor v
INNER JOIN Customer c
ON c.VendorId = v.Id 
INNER JOIN VendorApplication va 
ON va.CustomerId  = c.Id 
SET v.CategoryId = va.BusinessNatureCategoryId 