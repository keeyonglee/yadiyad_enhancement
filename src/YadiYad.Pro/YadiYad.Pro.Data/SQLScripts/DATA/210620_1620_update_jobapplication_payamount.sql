
UPDATE JobApplication ja
INNER JOIN JobProfile jp 
ON jp.Id  = ja.JobProfileId 
SET ja.PayAmount  = jp.PayAmount 
WHERE ja.PayAmount  = 0;


UPDATE JobApplication ja
INNER JOIN JobProfile jp 
ON jp.Id  = ja.JobProfileId 
SET ja.JobType  = jp.JobType 
WHERE ja.JobType  = 0;


UPDATE JobApplication ja
INNER JOIN JobProfile jp 
ON jp.Id  = ja.JobProfileId 
SET ja.JobRequired  = jp.JobRequired; 