
-- create store procedure 
DROP PROCEDURE IF EXISTS usp_SearchServiceProfile;
DELIMITER //
CREATE DEFINER=`admin`@`%` PROCEDURE `usp_SearchServiceProfile`(
	`Keyword` text,
		
	`CategoryIds` text,
    `ExpertiseIds` text,
    `ExclServiceTypeIds` text,
    `YearExperience` INT,
    `ServiceTypeId` INT,
    `ServiceModelId` INT,
    `StateProvinceId` INT,
    `ConsultationProfileId` INT,
    `CustomerId` INT,	
    `BuyerCustomerId` INT,
	
	`PageIndex` INT,
	`PageSize` INT,
	out `TotalRecords` INT
)
    READS SQL DATA
    SQL SECURITY INVOKER
BEGIN
	DECLARE strLen INT Default 0 ;
	DECLARE str VARCHAR(255);
	SET @sql_command = '';

	-- create table table for all serviceProfile 
	DROP TEMPORARY TABLE IF EXISTS `tempServiceProfileIds`;
	CREATE TEMPORARY TABLE tempServiceProfileIds (
		Id int NOT NULL auto_increment,
		ServiceProfileId int NOT NULL,
        PRIMARY KEY (id)
	);
	-- create table table for multiple category ids
	DROP TEMPORARY TABLE IF EXISTS `tempCategoryIds`;
	CREATE TEMPORARY TABLE tempCategoryIds (
		Id INT NOT NULL 
	);
	-- create table table for multiple expertise ids
	DROP TEMPORARY TABLE IF EXISTS `tempExpertiseIds`;
	CREATE TEMPORARY TABLE tempExpertiseIds (
		Id INT NOT NULL 
	);
	-- create table table for multiple exclServiceType ids
	DROP TEMPORARY TABLE IF EXISTS `tempExclServiceTypeIds`;
	CREATE TEMPORARY TABLE tempExclServiceTypeIds (
		Id INT NOT NULL 
	);

	-- get category id in table
	SET strLen=0;
	IF `CategoryIds` != '' AND `CategoryIds` IS NOT NULL
	THEN
		loop_getCategoryIds: LOOP
			SET strLen=strLen+1;
			SET str=fn_SplitStr(`CategoryIds`,",",strLen);
			IF str='' 
				THEN LEAVE loop_getCategoryIds;
			END IF;
			INSERT INTO tempCategoryIds VALUES (str);
		END LOOP loop_getCategoryIds;
	END IF;
	-- get expertise id in table
	SET strLen=0;
	IF `ExpertiseIds` != '' AND `ExpertiseIds` IS NOT NULL
	THEN
		loop_getExpertiseIds: LOOP
			SET strLen=strLen+1;
			SET str=fn_SplitStr(`ExpertiseIds`,",",strLen);
			IF str='' 
				THEN LEAVE loop_getExpertiseIds;
			END IF;
			INSERT INTO tempExpertiseIds VALUES (str);
		END LOOP loop_getExpertiseIds;
	END IF;
	-- get exclServiceType id in table
	SET strLen=0;
	IF `ExclServiceTypeIds` != '' AND `ExclServiceTypeIds` IS NOT NULL
	THEN
		loop_getExclServiceTypeIds: LOOP
			SET strLen=strLen+1;
			SET str=fn_SplitStr(`ExclServiceTypeIds`,",",strLen);
			IF str='' 
				THEN LEAVE loop_getExclServiceTypeIds;
			END IF;
			INSERT INTO tempExclServiceTypeIds VALUES (str);
		END LOOP loop_getExclServiceTypeIds;
	END IF;

	SET @sql_command = CONCAT (
		@sql_command
		,'	SELECT DISTINCT sp.Id'
		,'	FROM ServiceProfile sp '
		,'	LEFT JOIN ServiceAcademicQualification saq '
		,'	ON saq.ServiceProfileId = sp.Id AND !saq.Deleted'
		,'	LEFT JOIN ServiceLicenseCertificate slc '
		,'	ON slc.ServiceProfileId = sp.Id AND !slc.Deleted'
		,'	LEFT JOIN ServiceExpertise se '
		,'	ON se.ServiceProfileId = sp.Id AND !se.Deleted'
		,'	INNER JOIN Expertise e '
		,'	ON e.Id = se.ExpertiseId'
		,'	INNER JOIN IndividualProfile ip '
		,'	ON ip.CustomerId = sp.CustomerId'
 		,'	WHERE 1 = 1'
 		,'	AND !sp.Deleted'
 		,'	AND sp.DeletedFromUser = 0'
	);

 	-- filter customer id
	IF `CustomerId` != 0 AND `CustomerId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND sp.CustomerId  = ',`CustomerId`
	 		);
		SET @sql_command = CONCAT (
			@sql_command
 			,'	AND (ip.IsOnline = 1 OR ip.CustomerId = ', CustomerId, ')'
	 		);
	ELSE
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND ip.IsOnline = 1'
	 		);
 	END IF;

	-- filter state
	IF `StateProvinceId` != 0 AND `StateProvinceId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
			,'	AND (sp.StateProvinceId = ', `StateProvinceId`, ' OR sp.StateProvinceId IS NULL)'
	 		);
 	END IF;

 	-- filter service type
	IF `ServiceTypeId` != 0 AND `ServiceTypeId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND sp.ServiceTypeId  = ',`ServiceTypeId`
	 		);
 	END IF;
 	-- filter year of exp
	IF `YearExperience` != 0 AND `YearExperience` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND sp.YearExperience  = ',`YearExperience`
	 		);
 	END IF;

 	-- filter service model
	IF `ServiceModelId` != 0 AND `ServiceModelId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND sp.ServiceModelId  = ',`ServiceModelId`
	 		);
 	END IF;
 
 	-- filter category ids
 	IF EXISTS
 		(SELECT 1
 		FROM `tempCategoryIds`)
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND EXISTS'
			,'		(SELECT 1'
			,'		FROM tempCategoryIds c'
			,'		WHERE c.Id = e.JobServiceCategoryId)'
	 		);
 	END IF;
 	-- filter expertise ids
 	IF EXISTS
 		(SELECT 1
 		FROM `tempExpertiseIds`)
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND EXISTS'
			,'		(SELECT 1'
			,'		FROM tempExpertiseIds ex'
			,'		WHERE ex.Id = se.ExpertiseId)'
	 		);
 	END IF;
 	-- filter excl.serviceType ids
 	IF EXISTS
 		(SELECT 1
 		FROM `tempExclServiceTypeIds`)
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND NOT EXISTS'
			,'		(SELECT 1'
			,'		FROM tempExclServiceTypeIds est'
			,'		WHERE est.Id = sp.ServiceTypeId)'
	 		);
 	END IF;
 	-- filter ConsultationProfileId
	IF `ConsultationProfileId` != 0 AND `ConsultationProfileId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND NOT EXISTS'
			,'		(SELECT 1'
			,'		FROM ConsultationInvitation ci'
			,'		WHERE ci.ServiceProfileId = sp.Id'
			,'		AND ci.ConsultationProfileId = ',`ConsultationProfileId` 
			,'		AND !ci.Deleted' 
			,'		)'
	 		);
 	END IF; 
 
 	-- filter buyer customer id
	IF `BuyerCustomerId` != 0 AND `BuyerCustomerId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
			,'  AND sp.ServiceTypeId != 3'
	 		,'	AND NOT EXISTS'
			,'		(SELECT 1'
			,'		FROM ServiceApplication sa'
			,'		WHERE sa.ServiceProfileId = sp.Id'
			,'		AND sa.CustomerId = ',`BuyerCustomerId` 
			-- 4 :- ServiceApplicationStatus.Rejected
			-- 6 :- ServiceApplicationStatus.Completed
			,'		AND sa.Status != 4'
			,'		AND sa.Status != 6'
			,'		AND !sa.Deleted' 
			,'		)'
	 		);
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND sp.CustomerId != ',`BuyerCustomerId`
	 		);
 	END IF;
 
 	-- FULLTEXT search with keyword
 	IF `Keyword` != '' AND `Keyword` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND ('
	 		,'	   MATCH(sp.Company, sp.Position) AGAINST (''',`Keyword`,''' IN BOOLEAN MODE)'
	 		,'	OR MATCH(saq.AcademicQualificationName, saq.AcademicInstitution) AGAINST (''',`Keyword`,''' IN BOOLEAN MODE)'
	 		,'	OR MATCH(slc.ProfessionalAssociationName, slc.LicenseCertificateName) AGAINST (''',`Keyword`,''' IN BOOLEAN MODE)'
	 		,'	)'
	 		);
 	END IF;

	-- Order By
	SET @sql_command = CONCAT (
		@sql_command
	 	,'	ORDER BY (CASE '
		,'  WHEN sp.UpdatedOnUTC IS NULL THEN sp.CreatedOnUTC'
		,'  ELSE                           	  sp.UpdatedOnUTC'
		,'  END) DESC'
	 	);
	    
    SET @sql_command = concat('INSERT INTO tempServiceProfileIds (ServiceProfileId)', @sql_command);

    #SELECT @sql_command; #debug

	-- execute dynamic query
	PREPARE sql_do_stmts FROM @sql_command;
	EXECUTE sql_do_stmts;
	DEALLOCATE PREPARE sql_do_stmts;    
    
    SELECT COUNT(Id) from `tempServiceProfileIds` into `TotalRecords`;
    
    #return serviceProfiles
	SELECT sp.*
	FROM `tempServiceProfileIds` tsp
		INNER JOIN ServiceProfile sp on sp.Id = tsp.ServiceProfileId
	WHERE tsp.Id > `PageSize` * `PageIndex`
	ORDER BY tsp.Id
    LIMIT `PageSize`;

	-- create temp table
	DROP TEMPORARY TABLE IF EXISTS `tempServiceProfileIds`;
	DROP TEMPORARY TABLE IF EXISTS `tempCategoryIds`;
	DROP TEMPORARY TABLE IF EXISTS `tempExpertiseIds`;
	DROP TEMPORARY TABLE IF EXISTS `tempExclServiceTypeIds`;
END// 
DELIMITER ;

set @total:=0;
CALL usp_SearchServiceProfile("Fusion",NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,10,@total)


-- CALL `Create_FullText_Index`('ServiceLicenseCertificate', 'ProfessionalAssociationName, LicenseCertificateName', 'FT_IX_ProfessionalAssociationName_LicenseCertificateName',  @result);
-- CALL `Create_FullText_Index`('ServiceAcademicQualification', 'AcademicQualificationName, AcademicInstitution', 'FT_IX_JobProfile_AcademicQualificationName_AcademicInstitution',  @result);
-- CALL `Create_FullText_Index`('ServiceProfile', 'Company, Position', 'FT_IX_Company_Position',  @result);
