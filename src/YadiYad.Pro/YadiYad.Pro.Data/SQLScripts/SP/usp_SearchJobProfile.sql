
-- create store procedure 
DROP PROCEDURE IF EXISTS usp_SearchJobProfile;
DELIMITER //
CREATE DEFINER=`admin`@`%` PROCEDURE usp_SearchJobProfile(
	`Keyword` text,
	`CategoryIds` text,
	`ServiceTypeId`INT,
	`ServiceModelId` INT,
	`StateProvinanceId` INT,
	`CustomerId` INT,
    `IsFreelanceHourly` tinyint(1),
    `IsFreelanceDaily` tinyint(1),
    `IsProjectBased` tinyint(1),
    `IsOnSite` tinyint(1),
    `IsPartialOnSite` tinyint(1),
    `IsRemote` tinyint(1),
	`JobSeekerProfileId` INT,
	`PageIndex` INT,
	`PageSize` INT,
	out `TotalRecords` INT
) 
	READS SQL DATA 
	SQL SECURITY INVOKER
BEGIN
	/*********************************************************
	Updated Date		Updated By			Changes
	2021-06-01			Raymond				add end date filter
	*********************************************************/
	DECLARE strLen INT Default 0 ;
	DECLARE str VARCHAR(255);
	DECLARE _jobSeekerProfileId INT DEFAULT `JobSeekerProfileId`;
	SET @sql_command = '';
	SET @sql_command_job_model = '';
	SET @sql_command_job_type = '';

	-- create table table for all jobProfile 
	DROP TEMPORARY TABLE IF EXISTS `tempJobProfileIds`;
	CREATE TEMPORARY TABLE tempJobProfileIds (
		Id int NOT NULL auto_increment,
		JobProfileId int NOT NULL,
        PRIMARY KEY (id)
	);

	-- create table table for multiple category ids
	DROP TEMPORARY TABLE IF EXISTS `tempCategoryIds`;
	CREATE TEMPORARY TABLE tempCategoryIds (
		Id INT NOT NULL 
	);
	-- get category id in table
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

	-- create table table for applied jobProfile ids
	DROP TEMPORARY TABLE IF EXISTS `tempAppliedJobProfileIds`;
	CREATE TEMPORARY TABLE tempAppliedJobProfileIds (
		JobProfileId INT NOT NULL 
	);
	-- get applied jobProfile id in table
	IF `JobSeekerProfileId` != 0 AND `JobSeekerProfileId` IS NOT NULL
	THEN
		INSERT INTO tempAppliedJobProfileIds (JobProfileId) 
            SELECT JobProfileId FROM JobApplication ja 
				WHERE !ja.Deleted AND ja.JobSeekerProfileId = _jobSeekerProfileId
			UNION ALL
            SELECT JobProfileId FROM JobInvitation ji 
				WHERE !ji.Deleted AND ji.JobSeekerProfileId = _jobSeekerProfileId;
	END IF;

	SET @sql_command = CONCAT (
		@sql_command
		,'	SELECT jp.Id'
		,'	FROM JobProfile jp '
		,'	INNER JOIN JobServiceCategory jsc '
		,'	ON jsc.Id  = jp.CategoryId '
		,'	INNER JOIN OrganizationProfile op '
		,'	ON op.CustomerId = jp.CustomerId AND !op.Deleted'
		,'	LEFT JOIN ServiceSubscription ss'
		,'	ON ss.RefId  = jp.Id'
		,'	AND ss.Deleted  = 0'
		,'	AND ss.SubscriptionTypeId  = 2'
		,'	AND ss.StartDate <= UTC_TIMESTAMP()'
		,'	AND UTC_TIMESTAMP() <= IFNULL(ss.StopDate, ss.EndDate)'
 		,'	WHERE !jp.Deleted'
	);

	-- filter state
	IF `StateProvinanceId` != 0 AND `StateProvinanceId` IS NOT NULL 
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND (jp.StateProvinceId  = ',`StateProvinanceId`
	 		,'	OR jp.StateProvinceId  IS NULL)'
	 		);
 	END IF;

 	-- filter service type
	IF `ServiceTypeId` != 0 AND `ServiceTypeId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND jp.JobType  = ',`ServiceTypeId`
	 		);
 	END IF;

 	-- filter service model
	IF `ServiceModelId` != 0 AND `ServiceModelId` IS NOT NULL
	THEN
		IF `ServiceModelId` = 1
		THEN
			SET @sql_command = CONCAT (
				@sql_command
		 		,'	AND jp.IsOnSite  = 1'
		 		);
	 	END IF;
		IF `ServiceModelId` = 2
		THEN
			SET @sql_command = CONCAT (
				@sql_command
		 		,'	AND jp.IsPartialOnSite  = 1'
		 		);
	 	END IF;
		IF `ServiceModelId` = 3
		THEN
			SET @sql_command = CONCAT (
				@sql_command
		 		,'	AND jp.IsRemote  = 1'
		 		);
	 	END IF;
 	END IF;

 	-- filter job model
	IF `IsOnSite` = 1
	THEN
		SET @sql_command_job_model = CONCAT (
			@sql_command_job_model
		 	,'	OR jp.IsOnSite  = 1'
		 	);
	END IF;
	IF `IsPartialOnSite` = 1
	THEN
		SET @sql_command_job_model = CONCAT (
			@sql_command_job_model
		 	,'	OR jp.IsPartialOnSite  = 1'
		 	);
	END IF;
	IF `IsRemote` = 1
	THEN
		SET @sql_command_job_model = CONCAT (
			@sql_command_job_model
		 	,'	OR jp.IsRemote  = 1'
		 	);
	END IF;

	IF LENGTH(@sql_command_job_model) > 0
	THEN
		SET @sql_command = CONCAT (
			@sql_command
			,' AND (', SUBSTRING(@sql_command_job_model, 5), ')'
	 		);
	END IF;

 	-- filter job type
	IF `IsFreelanceHourly` = 1
	THEN
		SET @sql_command_job_type = CONCAT (
			@sql_command_job_type
		 	,'	OR jp.JobType  = 1'
		 	);
	END IF;
	IF `IsFreelanceDaily` = 1
	THEN
		SET @sql_command_job_type = CONCAT (
			@sql_command_job_type
		 	,'	OR jp.JobType  = 2'
		 	);
	END IF;
	IF `IsProjectBased` = 1
	THEN
		SET @sql_command_job_type = CONCAT (
			@sql_command_job_type
		 	,'	OR jp.JobType  = 3'
		 	);
	END IF;

	IF LENGTH(@sql_command_job_type) > 0
	THEN
		SET @sql_command = CONCAT (
			@sql_command
			,' AND (', SUBSTRING(@sql_command_job_type, 5), ')'
	 		);
	END IF;
     
 	-- filter customer id
	IF `CustomerId` != 0 AND `CustomerId` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND jp.CustomerId  = ',`CustomerId`
	 		);
	ELSE
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND ss.Id IS NOT NULL'
	 		);
 	END IF;
 
 	-- FULLTEXT search with keyword
 	IF `Keyword` != '' AND `Keyword` IS NOT NULL
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND MATCH(jp.JobTitle) AGAINST (''',`Keyword`,''' IN BOOLEAN MODE)'
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
			,'		WHERE c.Id = jp.CategoryId)'
	 		);
 	END IF;
 
 	-- filter applied job profile ids (from service profile id)
 	IF EXISTS
 		(SELECT 1
 		FROM `tempAppliedJobProfileIds`)
	THEN
		SET @sql_command = CONCAT (
			@sql_command
	 		,'	AND NOT EXISTS'
			,'		(SELECT 1'
			,'		FROM tempAppliedJobProfileIds ajp'
			,'		WHERE ajp.JobProfileId = jp.Id)'
	 		);
 	END IF;

	
	 SET @sql_command = CONCAT (
				@sql_command
	 			,'	ORDER BY CASE WHEN jp.UpdatedOnUTC IS NOT NULL THEN jp.UpdatedOnUTC ELSE jp.CreatedOnUTC END DESC'
	 			);
 	-- SELECT @sql_command;
	    
    SET @sql_command = concat('INSERT INTO tempJobProfileIds (JobProfileId)', @sql_command);

    #SELECT @sql_command; #debug

	-- execute dynamic query
	PREPARE sql_do_stmts FROM @sql_command;
	EXECUTE sql_do_stmts;
	DEALLOCATE PREPARE sql_do_stmts;    
    
    SELECT COUNT(Id) from `tempJobProfileIds` into `TotalRecords`;
    
    #return jobProfiles
	SELECT jp.*
	FROM `tempJobProfileIds` tjp
		INNER JOIN JobProfile jp on jp.Id = tjp.JobProfileId
	WHERE tjp.Id > `PageSize` * `PageIndex`
	ORDER BY tjp.Id
    LIMIT `PageSize`;

	-- create temp table
	DROP TEMPORARY TABLE IF EXISTS `tempJobProfileIds`;
	DROP TEMPORARY TABLE IF EXISTS `tempCategoryIds`;
	DROP TEMPORARY TABLE IF EXISTS `tempAppliedJobProfileIds`;
END// 
DELIMITER ;

set @total:=0;
-- CALL usp_SearchJobProfile("Software App","1,2,15", 3, 1, 59, NULL, NULL, 0, 10, @total);

-- CALL usp_SearchJobProfile("","", NULL, NULL, NULL, NULL, NULL, 0, 10, @total);

-- CALL `Create_FullText_Index`('JobProfile', 'JobTitle', 'FT_IX_JobProfile',  @result);

CALL usp_SearchJobProfile(
null
,"2,1"
,0
,0
,0
,0
,TRUE
,TRUE
,TRUE
,TRUE
,TRUE
,TRUE
,9
,0
,10
, @total
);




