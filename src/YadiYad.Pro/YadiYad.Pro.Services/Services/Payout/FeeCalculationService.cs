using Nop.Core;
using System;
using System.Linq;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Campaign;
using YadiYad.Pro.Services.Services.Messages;

namespace YadiYad.Pro.Services.Payout
{
    public class FeeCalculationService
    {
        private readonly PayoutRequestService _payoutRequestService;
        private readonly ChargeService _chargeService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly JobProfileService _jobProfileService;
        private readonly ConsultationInvitationService _consultationInvitationService;
        private readonly OrderService _orderService;
        private readonly RefundRequestService _refundRequestService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly CampaignProcessingService _campaign;

        public FeeCalculationService(
            IWorkContext workContext,
            PayoutRequestService payoutRequestService,
            ChargeService chargeService,
            ConsultationInvitationService consultationInvitationService,
            ServiceApplicationService serviceApplicationService,
            JobApplicationService jobApplicationService,
            JobProfileService jobProfileService,
            OrderService orderService,
            RefundRequestService refundRequestService,
            ProWorkflowMessageService proWorkflowMessageService,
            CampaignProcessingService campaign
            )
        {
            _workContext = workContext;
            _payoutRequestService = payoutRequestService;
            _chargeService = chargeService;
            _consultationInvitationService = consultationInvitationService;
            _serviceApplicationService = serviceApplicationService;
            _jobApplicationService = jobApplicationService;
            _jobProfileService = jobProfileService;
            _orderService = orderService;
            _refundRequestService = refundRequestService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _campaign = campaign;
        }


        public PayoutRequestDTO ProcessPayoutRequest(int actorId, string actorName, PayoutRequestDTO dto, bool disabledCheckOwnship = true)
        {
            //calculate fee
            var engagementFee = CalculatePayout(dto);

            if(engagementFee.Fee <= 0)
            {
                return null;
            }
            else
            {
                dto.Fee = engagementFee.Fee;
                dto.ServiceCharge = 0;
                dto.ServiceChargeRate = 0;
                dto.ServiceChargeType = (int)ChargeValueType.Rate;
            }

            var escrowProductTypeId = 0;

            switch(engagementFee.ProductTypeId)
            {
                case (int)ProductType.JobEnagegementFee:
                    escrowProductTypeId = (int)ProductType.JobEscrowFee;
                    break;
                case (int)ProductType.ServiceEnagegementFee:
                    escrowProductTypeId = (int)ProductType.ServiceEscrowFee;
                    break;
                case (int)ProductType.ConsultationEngagementFee:
                    escrowProductTypeId = (int)ProductType.ConsultationEscrowFee;
                    break;
            }

            //calculate payout platform charges
            if (escrowProductTypeId != 0)
            {
                var payoutCharge = _chargeService.GetLatestCharge(escrowProductTypeId, 0);

                if (payoutCharge != null)
                {
                    if (payoutCharge.ValueType == (int)ChargeValueType.Amount)
                    {
                        dto.ServiceCharge = payoutCharge.Value;
                        dto.ServiceChargeRate = payoutCharge.Value;
                        dto.ServiceChargeType = payoutCharge.ValueType;
                    }
                    else if (payoutCharge.ValueType == (int)ChargeValueType.Rate)
                    {
                        dto.ServiceCharge = Math.Round(engagementFee.Fee * payoutCharge.Value, 2);
                        dto.ServiceChargeRate = payoutCharge.Value;
                        dto.ServiceChargeType = payoutCharge.ValueType;
                    }

                    dto.Fee = engagementFee.Fee - dto.ServiceCharge;
                }
            }

            //perform db updates
            if (dto.Id == 0)
            {
                var updatedPayoutRequest = _payoutRequestService.CreatePayoutRequest(actorId, actorName, dto, disabledCheckOwnship);
                updatedPayoutRequest.RefId = dto.RefId;
                updatedPayoutRequest.ProductTypeId = dto.ProductTypeId;

                if (dto.ProductTypeId != (int)ProductType.ModeratorFacilitateConsultationFee
                    && dto.ProductTypeId != (int)ProductType.ConsultationEngagementFee)
                {
                    _proWorkflowMessageService.SendSubmittedPayoutRequestMessage(_workContext.WorkingLanguage.Id, updatedPayoutRequest);
                }
            }
            else
            {
                if (dto.Status == (int)PayoutRequestStatus.Approved
                    || dto.Status == (int)PayoutRequestStatus.RequiredMoreInfo)
                {
                    var updatedPayoutRequest = _payoutRequestService.UpdatePayoutRequestStatus(actorId, actorName, dto.Id, dto.Status, dto.EnteredRemark);
                    updatedPayoutRequest.RefId = dto.RefId;
                    updatedPayoutRequest.ProductTypeId = dto.ProductTypeId;

                    if (dto.Status == (int)PayoutRequestStatus.Approved)
                    {
                        _proWorkflowMessageService.SendApprovedPayoutRequestMessage(_workContext.WorkingLanguage.Id, updatedPayoutRequest);
                    }
                    else if (dto.Status == (int)PayoutRequestStatus.RequiredMoreInfo)
                    {
                        _proWorkflowMessageService.SendRequiredMoreInfoPayoutRequestMessage(_workContext.WorkingLanguage.Id, updatedPayoutRequest);
                    }
                }
                else if (dto.Status == (int)PayoutRequestStatus.New)
                {
                    _campaign.Apply(actorId, dto);
                    var updatedPayoutRequest = _payoutRequestService.UpdatePayoutRequestDetail(actorId, actorName, dto);
                    updatedPayoutRequest.RefId = dto.RefId;
                    updatedPayoutRequest.ProductTypeId = dto.ProductTypeId;
                    _proWorkflowMessageService.SendSubmittedPayoutRequestMessage(_workContext.WorkingLanguage.Id, updatedPayoutRequest);
                }
            }

            return dto;
        }

        /// <summary>
        /// use to calculate payout fee
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public CalculatedFeeDTO CalculatePayout(
            PayoutRequestDTO dto)
        {
            var engagementFeeDTO = new CalculatedFeeDTO();
            engagementFeeDTO.RefId = dto.RefId;
            engagementFeeDTO.ProductTypeId = dto.ProductTypeId;

            switch ((ProductType)dto.ProductTypeId)
            {
                case ProductType.ModeratorFacilitateConsultationFee:
                    {
                        var consultationMatchingFeeOderItem = _orderService.GetOrderItem((int)ProductType.ConsultationEngagementMatchingFee, dto.RefId);
                        var engagement = _consultationInvitationService.GetConsultationInvitationById(dto.RefId);
                        var consultationMatchingFee = consultationMatchingFeeOderItem.Price;

                        //check if cancel by seller/consultant
                        if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual)
                        {
                            //var refundRequest = _refundRequestService.GetByOrderItemId(consultationMatchingFeeOderItem.Id);

                            //if(refundRequest != null)
                            //{
                            //    consultationMatchingFee -= refundRequest.Amount;
                            //}
                        }

                        //identify moderator charges.
                        var moderatorFacilitateConsultationFeeType = ModeratorFacilitateConsultationFeeType.CompleteConsultation;

                        if(engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual)
                        {
                            moderatorFacilitateConsultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledBySeller;
                        }
                        else if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization)
                        {
                            var cancellationHourBeforeAppointment =
                                engagement.AppointmentStartDate.HasValue
                                ?(engagement.AppointmentStartDate.Value - engagement.CancellationDateTime.Value).TotalHours
                                :999999;

                            if(cancellationHourBeforeAppointment < 24)
                            {
                                moderatorFacilitateConsultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerLessThan24Hours ;
                            }
                            else if (cancellationHourBeforeAppointment < 72)
                            {
                                moderatorFacilitateConsultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerMoreThan24HoursLessThan72Hours;
                            }
                            else
                            {
                                moderatorFacilitateConsultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerMoreThan72Hours;
                            }
                        }

                        var moderatorFacilitateConsultationFeeCharge = _chargeService.GetLatestCharge(
                            (int)ProductType.ModeratorFacilitateConsultationFee,
                            (int)moderatorFacilitateConsultationFeeType);

                        if(moderatorFacilitateConsultationFeeCharge != null)
                        {
                            engagementFeeDTO.SubProductTypeId = (int)moderatorFacilitateConsultationFeeType;
                            engagementFeeDTO.Fee = 
                                moderatorFacilitateConsultationFeeCharge.ValueType == (int)ChargeValueType.Amount
                                ? moderatorFacilitateConsultationFeeCharge.Value
                                : consultationMatchingFee * moderatorFacilitateConsultationFeeCharge.Value;
                        }
                    }
                    break;
                case ProductType.ServiceEnagegementFee:
                    {
                        //get engagement
                        var engagement = _serviceApplicationService.GetServiceApplicationById(dto.RefId);
                        engagementFeeDTO.SubProductTypeId = engagement.ServiceProfileServiceTypeId;
                        var monthlyPayment = 0m;

                        engagementFeeDTO.StartDate = engagement.StartDate;
                        engagementFeeDTO.EndDate = engagement.EndDate;

                        //check type
                        if (engagement.ServiceProfileServiceTypeId == (int)ServiceType.Freelancing)
                        {
                            //by week * 4 = month
                            engagementFeeDTO.MaxMonthlyWorkDuration = engagement.Required.Value * 4 / 2;
                            engagementFeeDTO.ProratedPercentage = dto.IsProrated
                                ? decimal.Parse(dto.ProratedWorkDuration.ToString()) / decimal.Parse(engagementFeeDTO.MaxMonthlyWorkDuration.ToString())
                                : 1;

                            //fee per hour
                            var feePerHour = engagement.ServiceProfileServiceFee;

                            //check onsite
                            if (engagement.ServiceProfileServiceModelId == (int)ServiceModel.Onsite
                                || engagement.ServiceProfileServiceModelId == (int)ServiceModel.PartialOnsite)
                            {
                                feePerHour += engagement.ServiceProfileOnsiteFee;
                            }

                            monthlyPayment = feePerHour * engagementFeeDTO.MaxMonthlyWorkDuration * engagementFeeDTO.ProratedPercentage;
                        }
                        else if (engagement.ServiceProfileServiceTypeId == (int)ServiceType.PartTime)
                        {
                            //by month
                            engagementFeeDTO.MaxMonthlyWorkDuration = engagement.Required.Value / 2;
                            engagementFeeDTO.ProratedPercentage = dto.IsProrated
                                ? decimal.Parse(dto.ProratedWorkDuration.ToString()) / decimal.Parse(engagementFeeDTO.MaxMonthlyWorkDuration.ToString())
                                : 1;

                            //fee per hour
                            var feePerDay = engagement.ServiceProfileServiceFee;

                            //check onsite
                            if (engagement.ServiceProfileServiceModelId == (int)ServiceModel.Onsite
                                || engagement.ServiceProfileServiceModelId == (int)ServiceModel.PartialOnsite)
                            {
                                feePerDay += engagement.ServiceProfileOnsiteFee;
                            }

                            monthlyPayment = feePerDay * engagementFeeDTO.MaxMonthlyWorkDuration * engagementFeeDTO.ProratedPercentage;
                        }
                        else if (engagement.ServiceProfileServiceTypeId == (int)ServiceType.Consultation)
                        {
                            //by session or by project
                            monthlyPayment = engagement.ServiceProfileServiceFee;
                        }

                        //check escrow
                        engagementFeeDTO.Fee = monthlyPayment;
                    }
                    break;
                case ProductType.JobEnagegementFee:
                    {
                        var engagement = _jobApplicationService.GetJobApplicationById(dto.RefId);
                        engagementFeeDTO.SubProductTypeId = engagement.JobType;

                        if (engagement.JobType == (int)JobType.PartTime)
                        {
                            // $/day * day/month
                            engagementFeeDTO.MaxMonthlyWorkDuration = engagement.JobRequired.Value;
                            engagementFeeDTO.ProratedPercentage = dto.IsProrated
                                ? decimal.Parse(dto.ProratedWorkDuration.ToString()) / decimal.Parse(engagementFeeDTO.MaxMonthlyWorkDuration.ToString())
                                : 1;
                            engagementFeeDTO.Fee = engagement.PayAmount * engagementFeeDTO.MaxMonthlyWorkDuration * engagementFeeDTO.ProratedPercentage;
                        }
                        else if (engagement.JobType == (int)JobType.Freelancing)
                        {
                            //  $/hour * hour/week * 4 weeks/month
                            engagementFeeDTO.MaxMonthlyWorkDuration = engagement.JobRequired.Value * 4;
                            engagementFeeDTO.ProratedPercentage = dto.IsProrated
                                ? decimal.Parse(dto.ProratedWorkDuration.ToString()) / decimal.Parse(engagementFeeDTO.MaxMonthlyWorkDuration.ToString())
                                : 1;
                            engagementFeeDTO.Fee = engagement.PayAmount * engagementFeeDTO.MaxMonthlyWorkDuration * engagementFeeDTO.ProratedPercentage;
                        }
                        else if (engagement.JobType == (int)JobType.ProjectBased)
                        {
                            var jobMilestone = engagement.JobMilestones
                                .Where(x => x.Id == dto.JobMilestoneId)
                                .First();

                            engagementFeeDTO.Fee = jobMilestone.Amount;
                        }
                    }
                    break;
                case ProductType.ConsultationEngagementFee:
                    {
                        var consultationFeeOderItem = _orderService.GetOrderItem((int)ProductType.ConsultationEngagementFee, dto.RefId);

                        var engagement = _consultationInvitationService.GetConsultationInvitationById(dto.RefId);
                        var consultationFee = consultationFeeOderItem.Price;
                        engagementFeeDTO.SubProductTypeId = 0;

                        //check if cancel by seller/consultant
                        if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual)
                        {
                            engagementFeeDTO.Fee = consultationFeeOderItem.Price;
                        }
                        else
                        {
                            //consultation engagemment status
                            var consultationFeeType = ModeratorFacilitateConsultationFeeType.CompleteConsultation;

                            if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual)
                            {
                                consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledBySeller;
                            }
                            else if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization)
                            {
                                var cancellationHourBeforeAppointment = engagement.AppointmentStartDate.HasValue
                                    ?(engagement.AppointmentStartDate.Value - engagement.CancellationDateTime.Value).TotalHours
                                    :99999999;

                                if (cancellationHourBeforeAppointment < 24)
                                {
                                    consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerLessThan24Hours;
                                }
                                else if (cancellationHourBeforeAppointment < 72)
                                {
                                    consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerMoreThan24HoursLessThan72Hours;
                                }
                                else
                                {
                                    consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerMoreThan72Hours;
                                }
                            }

                            var consultantFeeRate = _chargeService.GetLatestCharge(
                                (int)ProductType.ConsultationEngagementFee,
                                (int)consultationFeeType);

                            if (consultantFeeRate != null
                                && consultantFeeRate.ValueType == (int)ChargeValueType.Rate)
                            {
                                engagementFeeDTO.Fee = consultationFee * consultantFeeRate.Value;
                            }
                            else if(consultationFeeType == ModeratorFacilitateConsultationFeeType.CompleteConsultation)
                            {
                                engagementFeeDTO.Fee = consultationFee;
                            }
                            else
                            {
                                engagementFeeDTO.Fee = 0;
                            }
                        }
                    }
                    break;
            }

            return engagementFeeDTO;
        }

        public CalculatedFeeDTO CalculateAdminCharges(ProductType productType, int refId)
        {
            var adminChargeDTO = new CalculatedFeeDTO();
            adminChargeDTO.RefId = refId;
            adminChargeDTO.ProductTypeId = (int)productType;
            adminChargeDTO.Fee = 0;

            switch (productType)
            {
                case ProductType.JobBuyerCancellationAdminCharges:
                    {
                        var engagement = _jobApplicationService.GetJobApplicationById(adminChargeDTO.RefId);

                        if (engagement.JobApplicationStatus == (int)JobApplicationStatus.CancelledByOrganization)
                        {
                            var cancellationHourBeforeAppointment = (engagement.StartDate.Value - engagement.CancellationDateTime.Value).TotalHours;

                            if (cancellationHourBeforeAppointment <= 5 * 24)
                            {
                                adminChargeDTO.SubProductTypeId = 0;
                                var chargeRate = _chargeService.GetLatestCharge((int)ProductType.JobBuyerCancellationAdminCharges, adminChargeDTO.SubProductTypeId);

                                if (chargeRate != null)
                                {
                                    if (engagement.JobType == (int)JobType.PartTime)
                                    {
                                        // $/day * day/month
                                        adminChargeDTO.MaxMonthlyWorkDuration = engagement.JobRequired.Value;
                                        adminChargeDTO.Fee = chargeRate.ValueType == (int)ChargeValueType.Amount
                                            ? chargeRate.Value
                                            : engagement.PayAmount * adminChargeDTO.MaxMonthlyWorkDuration * chargeRate.Value;
                                    }
                                    else if (engagement.JobType == (int)JobType.Freelancing)
                                    {
                                        //  $/hour * hour/week * 4 weeks/month
                                        adminChargeDTO.MaxMonthlyWorkDuration = engagement.JobRequired.Value * 4;
                                        adminChargeDTO.Fee = chargeRate.ValueType == (int)ChargeValueType.Amount
                                            ? chargeRate.Value
                                            : engagement.PayAmount * adminChargeDTO.MaxMonthlyWorkDuration * chargeRate.Value;
                                    }
                                    else if (engagement.JobType == (int)JobType.ProjectBased)
                                    {
                                        var totalAmount = engagement.JobMilestones
                                            .Sum(x => x.Amount);

                                        adminChargeDTO.Fee = chargeRate.ValueType == (int)ChargeValueType.Amount
                                            ? chargeRate.Value
                                            : totalAmount * chargeRate.Value;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ProductType.ServiceBuyerCancellationAdminCharges:
                    {
                        //get engagement
                        var engagement = _serviceApplicationService.GetServiceApplicationById(adminChargeDTO.RefId);
                        if (engagement.Status == (int)ServiceApplicationStatus.CancelledByBuyer)
                        {
                            adminChargeDTO.SubProductTypeId = 0;
                            var cancellationHourBeforeAppointment = (engagement.StartDate - engagement.CancellationDateTime.Value).TotalHours;

                            if (cancellationHourBeforeAppointment <= 5 * 24)
                            {
                                var monthlyPayment = 0m;

                                adminChargeDTO.StartDate = engagement.StartDate;
                                adminChargeDTO.EndDate = engagement.EndDate;

                                //check type
                                if (engagement.ServiceProfileServiceTypeId == (int)ServiceType.Freelancing)
                                {
                                    //by week * 4 = month
                                    adminChargeDTO.MaxMonthlyWorkDuration = engagement.Required.Value * 4 / 2;
                                    adminChargeDTO.ProratedPercentage = 1;

                                    //fee per hour
                                    var feePerHour = engagement.ServiceProfileServiceFee;

                                    //check onsite
                                    if (engagement.ServiceProfileServiceModelId == (int)ServiceModel.Onsite
                                        || engagement.ServiceProfileServiceModelId == (int)ServiceModel.PartialOnsite)
                                    {
                                        feePerHour += engagement.ServiceProfileOnsiteFee;
                                    }

                                    monthlyPayment = feePerHour * adminChargeDTO.MaxMonthlyWorkDuration * adminChargeDTO.ProratedPercentage;
                                }
                                else if (engagement.ServiceProfileServiceTypeId == (int)ServiceType.PartTime)
                                {
                                    //by month
                                    adminChargeDTO.MaxMonthlyWorkDuration = engagement.Required.Value / 2;
                                    adminChargeDTO.ProratedPercentage = 1;

                                    //fee per hour
                                    var feePerDay = engagement.ServiceProfileServiceFee;

                                    //check onsite
                                    if (engagement.ServiceProfileServiceModelId == (int)ServiceModel.Onsite
                                        || engagement.ServiceProfileServiceModelId == (int)ServiceModel.PartialOnsite)
                                    {
                                        feePerDay += engagement.ServiceProfileOnsiteFee;
                                    }

                                    monthlyPayment = feePerDay * adminChargeDTO.MaxMonthlyWorkDuration * adminChargeDTO.ProratedPercentage;
                                }
                                else if (engagement.ServiceProfileServiceTypeId == (int)ServiceType.Consultation)
                                {
                                    //by session or by project
                                    monthlyPayment = engagement.ServiceProfileServiceFee;
                                }

                                var chargeRate = _chargeService.GetLatestCharge((int)ProductType.JobBuyerCancellationAdminCharges, adminChargeDTO.SubProductTypeId);

                                if (chargeRate != null)
                                {
                                    adminChargeDTO.Fee = chargeRate.ValueType == (int)ChargeValueType.Amount ? chargeRate.Value : monthlyPayment * chargeRate.Value;
                                }
                            }
                        }
                    }
                    break;

                case ProductType.ConsultationBuyerCancellationAdminCharges:
                    {
                        var consultationFeeOderItem = _orderService.GetOrderItem((int)ProductType.ConsultationEngagementFee, refId);

                        var engagement = _consultationInvitationService.GetConsultationInvitationById(refId);
                        var consultationFee = consultationFeeOderItem.Price;
                        adminChargeDTO.SubProductTypeId = 0;

                        //check if cancel by seller/consultant
                        if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual)
                        {
                            adminChargeDTO.Fee = consultationFeeOderItem.Price;
                        }
                        else
                        {
                            //consultation engagemment status
                            var consultationFeeType = ModeratorFacilitateConsultationFeeType.CompleteConsultation;

                            if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual)
                            {
                                consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledBySeller;
                            }
                            else if (engagement.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization)
                            {
                                var cancellationHourBeforeAppointment =
                                    engagement.AppointmentStartDate.HasValue
                                    ? (engagement.AppointmentStartDate.Value - engagement.CancellationDateTime.Value).TotalHours
                                    : 999999;

                                if (cancellationHourBeforeAppointment < 24)
                                {
                                    consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerLessThan24Hours;
                                }
                                else if (cancellationHourBeforeAppointment < 72)
                                {
                                    consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerMoreThan24HoursLessThan72Hours;
                                }
                                else
                                {
                                    consultationFeeType = ModeratorFacilitateConsultationFeeType.CancelledByBuyerMoreThan72Hours;
                                }
                            }

                            var consultantFeeRate = _chargeService.GetLatestCharge(
                                (int)ProductType.ConsultationBuyerCancellationAdminCharges,
                                (int)consultationFeeType);

                            if (consultantFeeRate != null)
                            {
                                adminChargeDTO.Fee =
                                    consultantFeeRate.ValueType == (int)ChargeValueType.Amount
                                    ? consultantFeeRate.Value
                                    : consultationFee * consultantFeeRate.Value;
                            }
                        }
                    }
                    break;
            }

            return adminChargeDTO;
        }
    }
}
