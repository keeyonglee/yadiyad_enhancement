
DROP TEMPORARY TABLE IF EXISTS `tmpLocaleStringResources`;

CREATE TEMPORARY TABLE `tmpLocaleStringResources`(
  `ResourceName` varchar(200) NOT NULL,
  `ResourceValue` longtext NOT NULL,
  `LanguageId` int(11) NOT NULL
);

INSERT INTO `tmpLocaleStringResources` 
(`LanguageId`, `ResourceName`, `ResourceValue`) VALUES 
(1, 'Pro.Account.EmailConfirmation.Message', '<i>Note: Please check your Spam folder if it doesn’t appear in your Inbox. If you do not receive any email within an hour, please Whatsapp us at +6018 576 3009.</i>'),
(1, 'Pro.Job.Projectbased.Milestone.Example', '<div class="form-group"><div>Example for Milestone Description (pleases describe the job deliverables at each milestone as concisely as possible):</div><div>Phase / Stage 1 � Development of xxx module</div><div>Phase / Stage 2 � Interim and testing</div><div>Phase / Stage 3 � Final development and reporting </div></div>'),
(1, 'Pro.Account.Setting.ProfileOnline.Explaination', '<div>* Yes - Your profile will appear in Search results</div><div>* No - Your profile will not appear in Search results</div>'),
(1, 'Pro.Service.UserAgreement', 'I acknowledge that I have read and agreed to be bound by the terms and conditions in the <a class="btn-link text-primary text-bold" target="_blank" href="/terms-of-service">"Terms of Service" </a> and <a class="btn-link text-primary text-bold" target="_blank" href="/consultation-guidelines-and-terms">"Consulting Guidelines and Terms" </a>.'),
(1, 'Pro.Service.RegisterUserAgreement', 'By registering I agree to the "Terms of Service".'),
(1, 'Pro.Service.PrivacyPolicy', 'I acknowledge that I have read and agreed to be bound by "Privacy Policy".'),
(1, 'Pro.Service.CreateServiceProfile', 'Create Service Profile'),
(1, 'Pro.Job.SearchJobs', 'Search Jobs'),
(1, 'Pro.Job.MatchJobsWithKeyword', 'These are the matched jobs with <b>{{keyword}}</b>.'),
(1, 'Pro.Job.TotalJobsFound', '{{totalJob} Jobs found'),
(1, 'Pro.Service.CreateServiceProfile', 'Create Service Profile'),
(1, 'Pro.ConsultationJob.MinSessionRateByYearsOfExperience', '<p>11 - 19 years : USD 200 (~RM800)</p><p>20 - 30 years : USD 300 (~RM1200)</p><p>More than 30 years : USD 400 (~RM1600)</p>'),
(1, 'Pro.Common.SelectJobOrServices', 'Select Services'),
(1, 'Pro.Common.Category', 'Category'),
(1, 'Pro.Common.JobType', 'Job Type'),
(1, 'Pro.Common.JobModel', 'Job Model'),
(1, 'Pro.Common.Location', 'Location'),
(1, 'Pro.Common.SearchKeyword', 'Search Keyword'),
(1, 'Pro.Common.SearchFor', 'I''m searching for:'),
(1, 'Pro.Consultation.CreateConsultationProfile', 'Create Consultation Request'),
(1, 'Pro.Job.Consultation.Advs.Review', 'Review Consultation Job Profile'),
(1, 'Pro.Job.Consultation.Reply.Review', 'Review Consultant Replies'),
(1, 'Pro.Job.Consultation.Facilitating', 'Facilitate Consultation Session'),
(1, 'admin.catalog.jobservicecategory',	'Job Service Category'),
(1, 'admin.catalog.expertise',	'Expertise'),
(1, 'admin.contentmanagement.news.yadiyadnews',	'Yadiyad News'),
(1, 'Plugins.Payments.IPay88.Fields.ProxyResponseURL', 'Proxy Response URL'),
(1, 'Plugins.Payments.IPay88.Fields.ProxyPaymentURL', 'Proxy Payment URL'),
(1, 'admin.customers.bankAccount', 'Bank Accounts'),
(1, 'admin.customers.bankAccount.fields.accountholdername', 'Account Holder Name'),
(1, 'admin.customers.bankAccount.fields.bankname', 'Bank Name'),
(1, 'admin.customers.bankAccount.fields.accountnumber', 'Account Number'),
(1, 'admin.customers.bankAccount.fields.isverified', 'Is Verified'),
(1, 'admin.customers.bankAccount.fields.createdDate', 'Created Date'),
(1, 'admin.customers.bankAccount.fields.status', 'Status'),
(1, 'admin.customers.bankAccount.editbankaccountdetails', 'Edit bank account details'),
(1, 'admin.customers.bankAccount.backtolist', 'Back to bank account list'),
(1, 'admin.customers.bankAccount.reject', 'Reject'),
(1, 'admin.customers.bankAccount.approve', 'Approve'),
(1, 'Admin.Common.Action', 'Action'),
(1, 'Admin.Customers.Payout', 'Payout'),
(1, 'Admin.Customers.BankAccount.Fields.BatchNo', 'Batch No'),
(1, 'Admin.Customers.BankAccount.Fields.DateGenerated', 'Date Generated'),
(1, 'Admin.Customers.BankAccount.Fields.PayoutRecords', 'Payout Records'),
(1, 'Admin.Customers.BankAccount.Fields.DownloadDate', 'Download Date'),
(1, 'Admin.Customers.BankAccount.Fields.ReconDate', 'Recon Date'),
(1, 'Admin.Customers.BankAccount.Fields.Amount', 'Amount'),
(1, 'Admin.Customers.Payout.Fields.BatchNo', 'Batch No'),
(1, 'Admin.Customers.Payout.Fields.DateGenerated', 'Date Generated'),
(1, 'Admin.Customers.Payout.Fields.PayoutRecords', 'Payout Records'),
(1, 'Admin.Customers.Payout.Fields.Status', 'Status'),
(1, 'Admin.Customers.Payout.Fields.DownloadDate', 'Download Date'),
(1, 'Admin.Customers.Payout.Fields.ReconDate', 'Recon Date'),
(1, 'Admin.Customers.Payout.Fields.Amount', 'Amount'),
(1, 'Admin.Customers.Payout.Fields.Name', 'Name'),
(1, 'Admin.Customers.Payout.Fields.Records', 'Records'),
(1, 'Admin.Customers.Payout.Fields.Remarks', 'Remarks'),
(1, 'Admin.Customers.Payout.Fields.BankName', 'Bank Name'),
(1, 'Admin.Customers.Payout.Fields.BankAccountNo', 'Bank Account No.'),
(1, 'Admin.Customers.Payout.Fields.OrderNo', 'Order No.'),
(1, 'Admin.Customers.Payout.Fields.Type', 'Type'),
(1, 'Admin.Customers.Payout.Fields.ListedProfessionalFee', 'Listed Professional Fee'),
(1, 'Admin.Customers.Payout.Fields.PayoutCharges', 'Payout Charges'),
(1, 'Admin.Customers.Payout.Fields.PayoutAmount', 'Payout Amount'),
(1, 'Admin.Customers.Payout.BackToBatch', 'back to batch'),
(1, 'Admin.Customers.Payout.BackToBatchGroup', 'back to batch group'),
(1, 'admin.charge', 'Charge'),
(1, 'admin.customers.cancellationrequest', 'Cancellation Request'),
(1, 'Plugins.StorageMedia.AmazonS3.Instructions', 'Amazon S3 Setting'),
(1, 'Plugins.StorageMedia.AmazonS3.Fields.BucketName', 'Bucket Name'),
(1, 'Plugins.StorageMedia.AmazonS3.Fields.AccessKey', 'Access Key'),
(1, 'Plugins.StorageMedia.AmazonS3.Fields.AccessSecret', 'Access Secret'),
(1, 'Plugins.StorageMedia.AmazonS3.Fields.S3BaseUrl', 'S3 Base Url'),
(1, 'Plugins.StorageMedia.AmazonS3.Fields.PreSignedMinutes', 'PreSigned Minutes'),
(1, 'Plugins.StorageMedia.AmazonS3.Fields.PrivateBucketName', 'Private Bucket Name'),
(1, 'admin.proorders',	'ProOrders'),
(1, 'PayoutRequest.AutoApproveRemarks',	'Auto approved by system'),
(1, 'PayoutRequest.AutoApproveActorName',	'System'),
(1, 'Admin.Customers.Payout.Recon',	'Payout Recon'),
(1, 'Admin.PayoutBatch.Status.InProcess',	'Payout generation is still in process.'),
(1, 'Admin.Common.DownloadUploadedReconFile',	'Uploaded Recon File'),
(1, 'Admin.PayoutBatch.Status.NoItemToProcess',	'No available payout request or refund request for payout.'),
(1, 'Admin.Customers.Payout.Fields.StatusRemarks',	'Remarks'),
(1, 'Admin.PayoutBatch.ProcessReconFile',	'Process Recon File'),
(1, 'Account.PayoutBatch.ReconFileDownloadId.Required',	'Recon file is required.'),
(1, 'Admin.ContentManagement.PictureThumbnail',	'Picture Thumbnail'),
(1, 'Blog.Viewall',	'View More'),
(1, 'Admin.Dashboard.Member.Signup',	'Member Signup'),
(1, 'Admin.Dashboard.Other.Signup',	'Other Signup'),
(1, 'Admin.Dashboard.TransactionValue',	'Transaction Value'),
(1, 'Admin.Dashboard.ServiceCharges',	'Service Charges'),
(1, 'Admin.Dashboard.KeyStatistics',	'Key Statistics'),
(1, 'Admin.Dashboard.Pro.Job.Post',	'Pro Job Post'),
(1, 'Admin.Dashboard.Pro.Job.Hired',	'Pro Job Hired'),
(1, 'Admin.Dashboard.Pro.Service.Hired',	'Pro Service Hired'),
(1, 'Admin.Dashboard.Shuq.Orders',	'Shuq Orders'),
(1, 'Admin.Dashboard.Pro',	'Pro Dashboard'),
(1, 'Admin.Dashboard.Shuq',	'Shuq Dashboard'),
(1, 'Admin.Dashboard.Main',	'Main Dashboard'),
(1, 'Admin.Dashboard.Label.MemberSignUp',	'Member Signup'),
(1, 'Admin.Dashboard.Label.OtherSignUp',	'Other Signup'),
(1, 'Admin.Dashboard.Label.TransactionValue',	'Transaction Value'),
(1, 'Admin.Dashboard.Label.ServiceCharges',	'Service Charge'),
(1, 'Admin.Orders.List.ShippingLastName',	'Shipping last name'),
(1, 'Admin.Orders.List.ShippingPhone',	'Shipping phone number'),
(1, 'Admin.Orders.List.ShippingEmail',	'Shipping email address'),
(1, 'Admin.Orders.List.ShippingCountry',	'Shipping country'),
(1, 'Admin.Dashboard.Pro.TopJobCVSegments',	'Job CV Segments'),
(1, 'Admin.Dashboard.Pro.TopServiceSegments',	'Service Segments'),
(1, 'Admin.Dashboard.Pro.ActiveOrganizations',	'Active Organizations'),
(1, 'Admin.Dashboard.Pro.ActiveIndividuals',	'Active Individuals'),
(1, 'Pro.Job.Apply.OptInEscrow.Help', 'Find out more about what is Escrow and its benefits <a  class="btn-link text-primary text-bold" href="/escrow-service-terms" target="_blank">here</a>.'),
(1, 'Account.Vendor.Centre',	'Vendor"s Centre'),
(1, 'Pro.JobCancellationStatus.FirstCancellationProceedRehire',	'The job seeker had cancelled the engagement, the original job post is now extended for another 15 days from the cancellation date for 1 rehiring attempt.'),
(1, 'Pro.JobCancellationStatus.FirstCancellationProceedRefund',	'The Job Seeker had cancelled the engagement and your organization had requested for refund. Please check the refund status for more information.'),
(1, 'Pro.JobCancellationStatus.SecondCancellationDepositCollected',	'The rehired job seeker had cancelled the engagement, the deposit will be arranged for refund. Please check the refund status below for more information.'),
(1, 'Pro.JobCancellationStatus.SecondCancellationDepositNotCollected',	'The rehired job seeker had cancelled the engagement.'),
(1, 'Pro.JobCancellationStatus.FirstCancellationAlreadyRehired',	'The Job Applicant had cancelled the engagement and you have rehired a new candidate.'),
(1, 'Yadiyad.Platform.Name',	'Yadiyad Sdn. Bhd.'),
(1, 'Admin.Dashboard.Shuq.BestSellers.ByVolume',	'Bestsellers by volume'),
(1, 'Admin.Dashboard.Shuq.BestSellers.ByRevenue',	'Bestsellers by revenue'),
(1, 'Shuq.Invoice.InvoiceNumber',	'Invoice Number: {0}'),
(1, 'Shuq.Invoice.OrderDate',	'Order Date: {0}'),
(1, 'Checkout.BillToSameAddress',	'Bill to the same address'),
(1, 'Account.YourBankAccount',	'Your Bank Account'),
(1, 'Admin.Catalog.Products.Fields.DeliveryMode',	'Delivery Mode'),
(1, 'Account.BankAccount',	'Bank Account'),
(1, 'admin.orders.fields.guide.one', '1. Click on "Add Shipment" when the order is ready to be delivered.'),
(1, 'admin.orders.fields.guide.two', '2. Inside shipment, show barcode at courier"s centre to print airway bill or click on "Print" to view and print airway bill.'),
(1, 'admin.orders.fields.guide.three', '3. Inside shipment, click on "Set as shipped" when the parcel has been picked up by or dropped off at courier.'),
(1, 'pro.individual.tour.profilearea', 'Awesome! You\'ve taken the first step to sell your services to organizations / individuals or buy services. Let\'s continue.'),
(1, 'pro.individual.tour.seekfreelancejob', 'Want to sell your service to organizations? Click here to create your Job CV.'),
(1, 'pro.individual.tour.sellservices', 'Want to sell your service to individuals? Click here to create your Service Profile(s).'),
(1, 'pro.individual.tour.buyservices', 'Want to buy service from individual service seller? Click here to start searching.'),
(1, 'pro.individual.tour.onlineoffline', '<b>Online</b> - Your profile will be visible in Search results. <br> <b>Offline</b> - Your profile will be hidden from Search results.'),
(1, 'pro.individual.tour.mainmenu', 'Click here to complete / update / change your details or change any settings.'),
(1, 'pro.individual.tour.facebookmessengerbutton', 'Need help? Click here to chat with us.'),
(1, 'pro.individual.tour.facebookmessengerbutton.position', 'top-right'),
(1, 'pro.individual.tour.profilearea.position', 'top-center'),
(1, 'pro.individual.tour.seekfreelancejob.position', 'bottom-left'),
(1, 'pro.individual.tour.sellservices.position', 'bottom-center'),
(1, 'pro.individual.tour.buyservices.position', 'bottom-right'),
(1, 'pro.individual.tour.onlineoffline.position', 'bottom-center'),
(1, 'pro.individual.tour.mainmenu.position', 'bottom-right'),
(1, 'pro.individual.tour.highlight.colour', '#7cd9b4'),
(1, 'Admin.PayoutBatch.Status.Exception', 'Payout generation error'),
(1, 'pro.meta.keywords', 'yadiyad, yadiyad pro, yadiyadpro, freelance, freelancers, marketplace, sharing economy, gig, project based, part time, retired, retrenched, professionals, consultation'),
(1, 'landing.meta.keywords', 'yadiyad, yadiyad pro, yadiyadpro, freelance, freelancers, marketplace, sharing economy, gig, project based, part time, retired, retrenched, professionals, consultation, yadiyad shuq, yadiyadshuq, shuq, yadiyad shuq, yadiyadshuq, homemade, home cooked, family recipe, handmade, handcrafts, handicrafts, cottage industry'),
(1, 'pro.meta.description', 'Freelance marketplace for senior and experienced professionals'),
(1, 'landing.meta.description', 'Marketplace for freelancers, eCommerce for homepreneurs')
;

INSERT INTO LocaleStringResource
(`LanguageId`, `ResourceName`, `ResourceValue`)
select `LanguageId`, `ResourceName`, `ResourceValue`z
from `tmpLocaleStringResources` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM LocaleStringResource lsr
	where tlsr.ResourceName = lsr.ResourceName 
	limit 1);


SELECT lsr.ResourceName, lsr.ResourceValue , tlsr .ResourceValue 
FROM LocaleStringResource lsr
INNER JOIN tmpLocaleStringResources tlsr
ON tlsr.ResourceName = lsr.ResourceName 
WHERE lsr.`ResourceValue` != tlsr.ResourceValue;
/*
UPDATE LocaleStringResource lsr
INNER JOIN tmpLocaleStringResources tlsr
ON tlsr.ResourceName = lsr.ResourceName 
SET lsr.`ResourceValue` = tlsr.ResourceValue;
*/

SELECT *
FROM `tmpLocaleStringResources`
ORDER BY `ResourceName`;

DROP TABLE IF EXISTS `tmpLocaleStringResources`;


