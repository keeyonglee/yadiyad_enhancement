CREATE PROCEDURE `NS_AppNotificationDeviceLoadAllPaged`(
    StoreId		    int,
    DeviceTypeIds	    longtext,
    CustomerRoleIds	longtext,
    PageIndex			int,
    PageSize			int,
    OUT TotalRecords 	int
)
BEGIN
	DECLARE `sql_command` longtext;
    DECLARE `PageLowerBound` int;
	DECLARE `PageUpperBound` int;
	DECLARE `RowsToReturn` int;
    
	drop temporary table if EXISTS PageIndex;
	CREATE TEMPORARY TABLE PageIndex
	(
		`IndexId` int NOT NULL AUTO_INCREMENT,
		`DeviceId` int NOT NULL,
        primary key (`IndexId`)
	);
	
	SET DeviceTypeIds = ifnull(DeviceTypeIds, '');
	SET CustomerRoleIds = ifnull(CustomerRoleIds, '');

	SET @sql_command = 'INSERT INTO PageIndex (DeviceId) Select Id from NS_ApiDevice nad ';

	set @sql_command = CONCAT(@sql_command , 'WHERE 1 = 1 ');

	if(CustomerRoleIds != '') then
		SET @sql_command = CONCAT(@sql_command , 'AND nad.CustomerId in (Select distinct ccm.Customer_Id from Customer_CustomerRole_Mapping ccm where ccm.CustomerRole_Id in (' , CustomerRoleIds , ')) ');
end if;
	
	if(DeviceTypeIds != '') then
		SET @sql_command = CONCAT(@sql_command, 'AND nad.DeviceTypeId in (' , DeviceTypeIds , ') ');
end if;
		
	if(StoreId > 0) then
		SET @sql_command = CONCAT(@sql_command, 'AND nad.StoreId = ' , CAST(StoreId as UNSIGNED) , ' ');
end if;
	
	-- PRINT (@sql)
prepare stmt from @sql_command;
EXECUTE stmt;

-- paging

SET RowsToReturn = PageSize * (PageIndex + 1);	
	SET PageLowerBound = PageSize * PageIndex;
	SET PageUpperBound = PageLowerBound + PageSize + 1;
	
	-- total records
	SET TotalRecords = Found_rows();
	
	-- return products
SELECT wad.*
FROM
    `PageIndex` pi
        INNER JOIN NS_ApiDevice wad on wad.Id = pi.DeviceId
WHERE
        pi.IndexId > PageLowerBound AND
        pi.IndexId < PageUpperBound
ORDER BY pi.IndexId
    limit `RowsToReturn`;

DROP TEMPORARY TABLE PageIndex;
End