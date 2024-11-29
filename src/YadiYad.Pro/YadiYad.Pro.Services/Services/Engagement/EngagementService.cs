using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Engagement;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Service;

namespace YadiYad.Pro.Services.Services.Engagement
{
    public class EngagementService
    {
        #region Fields

        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly ConsultationInvitationService _consultationInvitationService;

        #endregion

        #region Ctor

        public EngagementService(
            ServiceApplicationService serviceApplicationService,
            JobApplicationService jobApplicationService,
            ConsultationInvitationService consultationInvitationService
            )
        {
            _serviceApplicationService = serviceApplicationService;
            _jobApplicationService = jobApplicationService;
            _consultationInvitationService = consultationInvitationService;
        }

        #endregion

        #region Method

        public EngagementDTO GetEngagement(int productTypeId, int refId)
        {
            EngagementDTO enagagementDTO = null;

            switch ((ProductType)productTypeId)
            {
                case ProductType.JobEnagegementFee:
                case ProductType.JobEscrowFee:
                case ProductType.JobBuyerCancellationAdminCharges:
                    {
                        var engagement = _jobApplicationService.GetJobApplicationById(refId);
                        enagagementDTO = new EngagementDTO
                        {
                            EngagementNo = engagement.Code,
                            ProductType = ProductType.JobEnagegementFee,
                            BuyerCustomerId = engagement.JobProfile.CustomerId,
                            SellerCustomerId = engagement.JobSeekerProfile.CustomerId,
                            IsProjectPayout = engagement.JobType == (int)JobType.ProjectBased,
                            EndMilestoneId = engagement.EndMilestoneId,
                            LastMilestoneId = engagement.JobMilestones.Count > 0 ? engagement.JobMilestones[engagement.JobMilestones.Count - 1].Id : (int?)null,
                            EndDate = engagement.EndDate

                        };
                    }
                    break;
                case ProductType.ServiceEnagegementMatchingFee:
                case ProductType.ServiceEnagegementFee:
                case ProductType.ServiceEscrowFee:
                case ProductType.ServiceBuyerCancellationAdminCharges:
                    {
                        var engagement = _serviceApplicationService.GetServiceApplicationById(refId);
                        enagagementDTO = new EngagementDTO
                        {
                            EngagementNo = engagement.Code,
                            ProductType = ProductType.ServiceEnagegementFee,
                            BuyerCustomerId = engagement.CustomerId,
                            SellerCustomerId = engagement.ServiceProfile.CustomerId,
                            IsProjectPayout = false,
                            EndDate = engagement.EndDate
                        };
                    }
                    break;
                case ProductType.ConsultationEngagementMatchingFee:
                case ProductType.ConsultationEngagementFee:
                case ProductType.ConsultationEscrowFee:
                case ProductType.ConsultationBuyerCancellationAdminCharges:
                case ProductType.ModeratorFacilitateConsultationFee:
                case ProductType.ConsultationEngagementMatchingFeeRefundAdminCharges:
                    {
                        var engagement = _consultationInvitationService.GetConsultationInvitationById(refId);

                        enagagementDTO = new EngagementDTO
                        {
                            EngagementNo = engagement.Code,
                            ProductType = ProductType.ConsultationEngagementFee,
                            BuyerCustomerId = engagement.OrganizationProfile.CustomerId,
                            SellerCustomerId = engagement.ServiceProfile.CustomerId,
                            ModeratorCustomerId  = engagement.ModeratorCustomerId,
                            IsProjectPayout = false,
                            EndDate = engagement.AppointmentEndDate
                        };
                    }
                    break;
            }

            if (enagagementDTO == null)
            {
                throw new KeyNotFoundException("Engagement not found.");
            }

            return enagagementDTO;
        }

        public void CompleteEngagement(int actorId, int productTypeId, int refId)
        {
            switch ((ProductType)productTypeId)
            {
                case ProductType.JobEnagegementFee:
                    {
                        _jobApplicationService.UpdateJobApplicationStatus(actorId, refId, JobApplicationStatus.Completed);
                    }
                    break;
                case ProductType.ServiceEnagegementFee:
                    {
                        _serviceApplicationService.UpdateServiceApplicationStatus(refId, actorId, ServiceApplicationStatus.Completed);
                    }
                    break;
                case ProductType.ConsultationEngagementFee:
                    {
                        //not need update as updated during moderator update.
                    }
                    break;
            }
        }


        #endregion

    }
}
