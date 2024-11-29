START TRANSACTION;

DROP TEMPORARY TABLE IF EXISTS `tmpMessageTemplate`;

CREATE TEMPORARY TABLE `tmpMessageTemplate`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Email` varchar(200) NOT NULL
  ,`Name` varchar(200) NOT NULL
  ,`Subject` varchar(1000) NOT NULL
  ,`Body` longtext NOT NULL
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpMessageTemplate` 
(`Email`, `Name`,`Subject`, `Body`) VALUES 
('feedbacks@yadiyad.com' ,'Home.ContactUs.Feedbacks'	,'%Store.Name%. Contact us'	,
'<p>
%ContactUs.Body%
</p>'),
('help@yadiyad.com' ,'Home.ContactUs.Issues'	,'%Store.Name%. Contact us'	,
'<p>
%ContactUs.Body%
</p>'),
('hello@yadiyad.com' ,'Home.ContactUs.Enquiry'	,'%Store.Name%. Contact us'	,
'<p>
%ContactUs.Body%
</p>'),
('noreply@yadiyad.com' ,'DepositRequest.LastReminder'	,'%Store.Name%. Last Reminder for Deposit Request #%DepositRequest.DepositNumber%'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This is last reminder for the deposit request  #%DepositRequest.DepositNumber%, we requires your immediate action.
<br />
Order number: %DepositRequest.OrderNumber%
<br />
Product name: %DepositRequest.OrderItem.ItemName%
<br />
Deposit amount: %DepositRequest.Amount%
<br />
Cycle: %DepositRequest.StartToEnd%
<br />
Due date: %DepositRequest.DueDate%
<br />
To go to link <a href="%DepositRequest.ConfirmedOrder.Url%">click here</a>.
<br />
<br />
%Store.Name%
'),
('noreply@yadiyad.com' ,'DepositRequest.Reminder'	,'%Store.Name%. %DepositRequest.ReminderCount% Reminder for Deposit Request #%DepositRequest.DepositNumber%'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This is %DepositRequest.ReminderCount% reminder for the deposit request  #%DepositRequest.DepositNumber%, we requires your immediate action.
<br />
Order number: %DepositRequest.OrderNumber%
<br />
Product name: %DepositRequest.OrderItem.ItemName%
<br />
Deposit amount: %DepositRequest.Amount%
<br />
Cycle: %DepositRequest.StartToEnd%
<br />
Due date: %DepositRequest.DueDate%
<br />
To go to link <a href="%DepositRequest.ConfirmedOrder.Url%">click here</a>.
<br />
<br />
%Store.Name%
'),
('noreply@yadiyad.com' ,'DepositRequest.Notification'	,'%Store.Name%. New Deposit Request #%DepositRequest.DepositNumber%'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
There is a new deposit request  #%DepositRequest.DepositNumber% requires your action.
<br />
Order number: %DepositRequest.OrderNumber%
<br />
Product name: %DepositRequest.OrderItem.ItemName%
<br />
Deposit amount: %DepositRequest.Amount%
<br />
Cycle: %DepositRequest.StartToEnd%
<br />
Due date: %DepositRequest.DueDate%
<br />
To go to link <a href="%DepositRequest.ConfirmedOrder.Url%">click here</a>.
<br />
<br />
%Store.Name%
'),
('noreply@yadiyad.com' ,'Customer.BlockSeller'	,'%Store.Name%. Access blocked'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
You have been block with immediate effect till %Block.EndDate% because of:
<br />
Reason: %Block.Reason%
Remarks: %Block.Remarks%
<br />
Number of times you have been blocked: %Block.NumberOfTimes%.
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Seller.Cancellation'	,'%Store.Name%. Consultation Job Cancelled'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This Consultation Job has been cancelled by %ConsultationInvitation.CancelledBy%.
Reason: %ConsultationInvite.Reason%
Remarks: %ConsultationInvite.Remarks%
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Seller.DeclinedByOrganization'	,'%Store.Name%. Consultation Job is No Longer Needed by Organization'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This Consultation Job is no longer needed by %Buyer.Name%.
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Buyer.Cancellation'	,'%Store.Name%. Consultation Profile Cancelled'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This Consultation Job has been cancelled by %ConsultationInvitation.CancelledBy%.
Reason: %ConsultationInvite.Reason%
Remarks: %ConsultationInvite.Remarks%
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Seller.Reschedule'	,'%Store.Name%. Consultation Job Rescheduled'	,
'%Store.Name%. Consultation Profile Rescheduled
<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This Consultation Job has been rescheduled to: 
Start Time: %ConsultationInvite.StartTime%
End Time: %ConsultationInvite.EndTime%
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Buyer.Reschedule'	,'%Store.Name%. Consultation Profile Rescheduled'	,
'%Store.Name%. Consultation Profile Rescheduled
<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This Consultation Job has been rescheduled to: 
Start Time: %ConsultationInvite.StartTime%
End Time: %ConsultationInvite.EndTime%
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Seller.Completed'	,'%Store.Name%. Consultation Job Completed'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This Consultation Job has been successfully completed.
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Buyer.Completed'	,'%Store.Name%. Consultation Profile Completed'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This Consultation Job has been successfully completed.
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Candidate.Paid'	,'%Store.Name%. Consultation Job Paid'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
You have been paid for this Consultation Job: 
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Paid.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Organization.Declined'	,'%Store.Name%. Consultation Job Declined'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
%Individual.Fullname% has declined your invite for this Consultation Job: 
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Declined.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Organization.Accepted'	,'%Store.Name%. Consultation Job Accepted'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
%Individual.Fullname% has accepted your invite for this Consultation Job: 
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Accepted.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Consultation.Candidate.Invited'	,'%Store.Name%. Consultation Job invitation'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
You have been invited to apply for this Consultation Job: 
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Invite.Url%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Service.Seller.Confirm'	,'%Store.Name%. Sell Service Application Confirm'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Sell Service Application for %JobServiceCategory% is confirm
<br />
To go to link <a href="%ServiceApplication.HiresUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Service.Buyer.Confirm'	,'%Store.Name%. Buy Service Application Confirm'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Buy Service Application for %JobServiceCategory% is confirm
<br />
To go to link <a href="%ServiceApplication.ConfirmsUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Service.Buyer.Reproposed'	,'%Store.Name%. Buy Service Application Reproposed'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Buy Service Application for %JobServiceCategory% is reproposed
To go to link <a href="%ServiceApplication.RequestsUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Service.Buyer.Declined'	,'%Store.Name%. Buy Service Application Declined'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Buy Service Application for %JobServiceCategory% is declined
To go to link <a href="%ServiceApplication.RequestsUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Service.Buyer.Accepted'	,'%Store.Name%. Buy Service Application Accepted'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Buy Service Application for %JobServiceCategory% is accepted
To go to link <a href="%ServiceApplication.RequestsUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'Service.Seller.Request'	,'%Store.Name%. Sell Service Application Request'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Sell Service Application for %JobServiceCategory% is requested
<br />
To go to link <a href="%ServiceApplication.ReceivesUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'JobApplication.Individual.Hire'	,'%Store.Name%. Job Application Status Changed'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Job Application status for %JobInvitation.JobTitle% is hired
<br />
To go to link <a href="%JobApplication.AppliedUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'JobApplication.Individual.FutureReference'	,'%Store.Name%. Job Application Status Changed'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Job Application status for %JobInvitation.JobTitle% is keep for future reference
<br />
To go to link <a href="%JobApplication.AppliedUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'JobApplication.Individual.Shortlist'	,'%Store.Name%. Job Application Status Changed'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
Your Job Application status for %JobInvitation.JobTitle% is shortlisted
<br />
To go to link <a href="%JobApplication.AppliedUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'JobInvitation.Organisation.Rejected'	,'%Store.Name%. Job Invitation Rejected'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
%Individual.Fullname% has rejected your invite for %JobInvitation.JobTitle%
<br />
To go to link <a href="%JobInvitation.RejectUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'JobInvitation.Organisation.Accepted'	,'%Store.Name%. Job Invitation Accepted'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
%Individual.Fullname% has accepted your invite for %JobInvitation.JobTitle%
<br />
To go to link <a href="%JobInvitation.AcceptUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'JobInvitation.Individual.Invite'	,'%Store.Name%. Job Invitation Invite'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
You have been invited to apply for %JobInvitation.JobTitle%
<br />
To go to invite <a href="%JobInvitation.InviteUrl%">click here</a>.
<br />
<br />
%Store.Name%'),
('noreply@yadiyad.com' ,'PayoutRequest.NonProject.New'	,'%PayoutRequest.EngagementNo% - Payout Request Required Approval'	
,'Dear Sir/Madam,

Payout request on %PayoutRequest.EngagementNo% required your approval.'),
('noreply@yadiyad.com' ,'PayoutRequest.NonProject.Approved'	,'%PayoutRequest.EngagementNo% - Payout Request Approved'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% has been approved.'),
('noreply@yadiyad.com' ,'PayoutRequest.NonProject.AutoApproved'	,'%PayoutRequest.EngagementNo% - Payout Request Auto Approved'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% has been auto approved.'),
('noreply@yadiyad.com' ,'PayoutRequest.NonProject.RequiredMoreInfo'	,'%PayoutRequest.EngagementNo% - Payout Request Required More Info'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% required more info to proceed.'),
('noreply@yadiyad.com' ,'PayoutRequest.NonProject.Error'	,'%PayoutRequest.EngagementNo% - Payout Request Error'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% having issue during payout.
Please proceed to verify your bank account.'),
('noreply@yadiyad.com' ,'PayoutRequest.NonProject.Paid'	,'%PayoutRequest.EngagementNo% - Payout Request Paid'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% has been paid.'),
('noreply@yadiyad.com' ,'PayoutRequest.Project.New'	,'%PayoutRequest.EngagementNo% - Payout Request Required Approval'	
,'Dear Sir/Madam,

Payout request on %PayoutRequest.EngagementNo% required your approval.'),
('noreply@yadiyad.com' ,'PayoutRequest.Project.Approved'	,'%PayoutRequest.EngagementNo% - Payout Request Approved'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% has been approved.'),
('noreply@yadiyad.com' ,'PayoutRequest.Project.AutoApproved'	,'%PayoutRequest.EngagementNo% - Payout Request Auto Approved'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% has been auto approved.'),
('noreply@yadiyad.com' ,'PayoutRequest.Project.RequiredMoreInfo'	,'%PayoutRequest.EngagementNo% - Payout Request Required More Info'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% required more info to proceed.'),
('noreply@yadiyad.com' ,'PayoutRequest.Project.Error'	,'%PayoutRequest.EngagementNo% - Payout Request Error'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% having issue during payout.
Please proceed to verify your bank account.'),
('noreply@yadiyad.com' ,'PayoutRequest.Project.Paid'	,'%PayoutRequest.EngagementNo% - Payout Request Paid'	
,'Dear Sir/Madam,

Your payout request on %PayoutRequest.EngagementNo% has been paid.'),
('noreply@yadiyad.com' ,'DepositRequest.PaymentVerificationNotification'	,'%PayoutRequest.EngagementNo% - Deposit Request Payment Verification'	
,'Dear Sir/Madam,

Deposit request, %DepositRequest.DepositNumber% required your approval to proceed.'),
('noreply@yadiyad.com' ,'DepositRequest.TerminatingApplication.Buyer'	,'%Store.Name%. %ProductType% Terminating Due to Deposit Request #%DepositRequest.DepositNumber% Not Fulfilled'	
,'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This is to inform that the %ProductType% is terminating due to the deposit request (#%DepositRequest.DepositNumber%) is not fulfilled.
<br />
Order number: %DepositRequest.OrderNumber%
<br />
Product name: %DepositRequest.OrderItem.ItemName%
<br />
End date: %DepositRequest.EndDate%
<br />
To go to link <a href="%DepositRequest.ConfirmedOrder.Url%">click here</a>.
<br />
<br />
%Store.Name%
'),
('noreply@yadiyad.com' ,'DepositRequest.TerminatingApplication.Seller'	,'%Store.Name%. %ProductType% Terminating Due to Buyers Insufficient Deposit Balance'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This is to inform that the %ProductType% is terminating due to buyers insufficient deposit balance.
<br />
Order number: %DepositRequest.OrderNumber%
<br />
Product name: %DepositRequest.OrderItem.ItemName%
<br />
End date: %DepositRequest.EndDate%
<br />
To go to link <a href="%DepositRequest.ConfirmedOrder.Url%">click here</a>.
<br />
<br />
%Store.Name%
'),
('noreply@yadiyad.com' ,'PayoutRequest.FailInvalidBankAccount'	,'%Store.Name%. Payout Fail to Process'	,
'<a href="%Store.URL%">%Store.Name%</a>
<br />
<br />
This is to inform that recent payout has fail to process due to invalid bank account info. Please update bank account info so that payout can re-process again.
<br />
<br />
%Store.Name%
'),
('noreply@yadiyad.com' ,'Consultation.Consultant.AutoDeclined'	,'%Store.Name% - Consultation Job Auto Declined after %ConsultationInvite.InvitationAutoRejectAfterWorkingDays% working days'	,
'
Consultation job invitation from %Buyer.Name%  has been auto declined by the system after %ConsultationInvite.InvitationAutoRejectAfterWorkingDays% working days.
<br />
The consultation job info as below:
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Declined.Url%">click here</a>.
<br />
<br />
'),
('noreply@yadiyad.com' ,'Consultation.Organization.AutoDeclined'	,'%Store.Name% - Consultation Job Auto Declined after %ConsultationInvite.InvitationAutoRejectAfterWorkingDays% working days'	,
'
Consultation job invitation to %Seller.Name% has been auto declined by the system after %ConsultationInvite.InvitationAutoRejectAfterWorkingDays% working days.
<br />
The consultation job info as below:
<br />
Segment: %ConsultationProfile.SegmentName%
<br />
Topic: %ConsultationProfile.Topic%
<br />
Objective: %ConsultationProfile.Objective%
<br />
To go to link <a href="%Consultation.Declined.Url%">click here</a>.
<br />
<br />
'),
('noreply@yadiyad.com' ,'Project.Deposit.Confirmed' ,'%Project.EngagementNo% - Deposit Confirmed'	
,'Dear Sir/Madam,
<br />
Your bank in deposit for Project No. %Project.EngagementNo% is confirmed'),
('noreply@yadiyad.com' ,'Project.Deposit.NotConfirmed' ,'%Project.EngagementNo% - Deposit Not Confirmed'	
,'Dear Sir/Madam,
<br />
Your bank in deposit for Project No. %Project.EngagementNo% is not confirmed
<br />
Please refer to remarks.
<br />
Remarks: %Project.Deposit.Remarks%'),
('noreply@yadiyad.com' ,'Project.Deposit.Buyer.Paid' ,'%Project.EngagementNo% - Deposit Paid by Organization'	
,'Dear Admin,
<br />
Please check Project No. %Project.EngagementNo% for deposit approval.')
;

INSERT INTO MessageTemplate
(Name, BccEmailAddresses, Subject, EmailAccountId, Body, IsActive, DelayBeforeSend, DelayPeriodId, AttachedDownloadId, LimitedToStores)
select tmt.Name, NULL,tmt. `Subject`, ea.Id, tmt.`Body`, 1, NULL, 0, 0, 0 
from `tmpMessageTemplate` tmt 
INNER JOIN EmailAccount ea 
ON ea.Email  = tmt.`Email`
where NOT EXISTS
	(SELECT 0
	FROM `MessageTemplate` mt
	where tmt.Name = mt.Name 
	limit 1);

-- UPDATE MessageTemplate mt
-- INNER JOIN `tmpMessageTemplate` tmt 
-- ON mt.Name  = tmt.Name
-- SET mt.Subject = tmt.Subject
-- , mt.Body  = tmt.Body;


DROP TABLE IF EXISTS `tmpMessageTemplate`;

COMMIT;


