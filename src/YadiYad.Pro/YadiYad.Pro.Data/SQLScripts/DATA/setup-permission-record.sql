
DROP TEMPORARY TABLE IF EXISTS `tmpPermissionRecord`;

CREATE TEMPORARY TABLE `tmpPermissionRecord`(
    `Name` longtext NOT NULL,
  `SystemName` varchar(255) NOT NULL,
  `Category` varchar(255) NOT NULL
);

INSERT INTO `tmpPermissionRecord` 
(`Name`, `SystemName`, `Category`) VALUES 
('Admin area. Manage Expertise'	,'ManageExpertise'	,'Catalog'),
('Admin area. Manage YadiyadNews'	,'ManageYadiyadNews'	,'Content Management'),
('Admin area. Manage ProOrders'	,'ManageProOrders'	,'Orders'),
('Organisation. Jobs Summary'	,'JobsSummary'	,'Organisation'),
('Pro Area. Jobs'	,'Jobs'	,'Jobs'),
('Organisation Job. Jobs Summary'	,'JobsSummary'	,'OrganisationJob'),
('Organisation Job. Organisation Profile'	,'OrganisationProfile'	,'Organisation'),
('Organisation Job. Organisation Edi tProfile'	,'OrganisationEditProfile'	,'Organisation'),
('Organisation Job. Create Job Profile'	,'CreateJobProfile'	,'OrganisationJob'),
('Organisation Job. Job Posting'	,'JobPosting'	,'OrganisationJob'),
('Organisation Job. Job Invited'	,'JobInvited'	,'OrganisationJob'),
('Organisation Job. Job Applicants'	,'JobApplicants'	,'OrganisationJob'),
('Organisation Consultation. Consultation Posting'	,'ConsultationPosting'	,'OrganisationConsultation'),
('Organisation Consultation. Consultation Invited'	,'ConsultationInvited'	,'OrganisationConsultation'),
('Organisation Consultation. Consultation Applicants'	,'ConsultationApplicants'	,'OrganisationConsultation'),
('Organisation Consultation. Consultation Confirmed'	,'ConsultationConfirmed'	,'OrganisationConsultation'),
('Organisation Consultation. Consultation New'	,'ConsultationNew'	,'OrganisationConsultation'),
('Organisation Consultation. Consultation Search Candidates'	,'ConsultationSearchCandidates'	,'OrganisationConsultation'),
('Individual. Profile'	,'IndividualProfile'	,'Individual'),
('Individual. Individual Profile Edit'	,'IndividualProfileEdit'	,'Individual'),
('Individual Job. Job Profile Edit'	,'JobProfileEdit'	,'IndividualJob'),
('Individual Job. Job Invites'	,'JobInvites'	,'IndividualJob'),
('Individual Job. Job Applied'	,'JobApplied'	,'IndividualJob'),
('Individual Job. Job Search'	,'JobSearch'	,'IndividualJob'),
('Individual Service. Service Profiles'	,'ServiceProfiles'	,'IndividualService'),
('Individual Service. Received Requests'	,'ReceivedRequests'	,'IndividualService'),
('Individual Service. Hire Requests'	,'HireRequests'	,'IndividualService'),
('Individual Service. Create Service Profiles'	,'CreateServiceProfiles'	,'IndividualService'),
('Individual Service. Edit Service Profiles'	,'EditServiceProfiles'	,'IndividualService'),
('Individual Service. Search Sellers'	,'SearchSellers'	,'IndividualService'),
('Individual Service. Requested Orders'	,'RequestedOrders'	,'IndividualService'),
('Individual Service. Confirmed Orders'	,'ConfirmedOrders'	,'IndividualService'),
('Moderator. Advs Review'	,'AdvsReview'	,'Moderator'),
('Moderator. Reply Review'	,'ReplyReview'	,'Moderator'),
('Moderator. Facilitating'	,'Facilitating'	,'Moderator'),
('Moderator. Facilitate All Session'	,'ConsultationFacilitateAllSession'	,'Moderator'),
('Organization. Job'	,'OrganizationJob'	,'Organization'),
('Organization. Consultation'	,'OrganizationConsultation'	,'Organization'),
('Individual. Job'	,'IndividualJob'	,'Individual'),
('Individual. Service'	,'IndividualService'	,'Individual'),
('Moderator. Review'	,'ModeratorReview'	,'Moderator'),
('Organization. Profile'	,'OrganizationProfile'	,'Organization'),
('Admin area. Manage Shop'	,'ManageShop'	,'Vendor'),
('Admin area. Manage Products Operator Mode'	,'ManageProductsOperatorMode'	,'Catalog'),
('Admin area. Manage Products Advanced Mode'	,'ManageProductsAdvancedMode'	,'Catalog')
;

INSERT INTO PermissionRecord 
(`Name`, `SystemName`, `Category`)
SELECT `Name`, `SystemName`, `Category`
FROM `tmpPermissionRecord` tce
where NOT EXISTS
	(SELECT 0
	FROM PermissionRecord jsc
	where tce.SystemName = jsc.SystemName 
	limit 1)
ORDER BY `SystemName`;




/*

SELECT *
FROM `tmpPermissionRecord`;
*/

DROP TABLE IF EXISTS `tmpPermissionRecord`;


