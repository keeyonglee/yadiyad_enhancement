
UPDATE JobApplication ja
SET ja.HiredTime = ja.UpdatedOnUTC
WHERE ja.JobApplicationStatus IN (6,9,12,13,14,15,16)
AND  ja.HiredTime IS NULL