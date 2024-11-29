using System.Security;
using System.Security.Authentication;
using System;

using Nop.Core;

using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.Engagement;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Services.Engagement;
using YadiYad.Pro.Services.Services.Moderator;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.DTO.Payout;
using Nop.Services.Security;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Services.Attentions;

namespace YadiYad.Pro.Services.Engagement
{
    public class EngagementCancellationManager
    {
        private readonly IPermissionService _permissionService;
        private readonly BlockCustomerService _blockCustomerService;
        private readonly CancellationRequestService _cancellationRequestService;
        private readonly EngagementResolver _engagementResolver;
        private readonly RefundRequestService _refundRequestService;
        private readonly FeeCalculationService _feeCalculationService;
        private readonly OrderService _orderService;
        private readonly ServiceSubscriptionService _serviceSubscriptionService;
        private const string ENGAGEMENT_ACTION = "Cancellation";
        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly ProEngagementSettings _proEngagementSettings;
        private readonly JobApplicationService _jobApplicationService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly ChargeService _chargeService;
        private readonly OrganizationAttentionService _organizationAttentionService;
        private readonly IndividualAttentionService _individualAttentionService;


        public EngagementCancellationManager(
            IPermissionService permissionService,
            BlockCustomerService blockCustomerService,
            EngagementResolver engagementResolver,
            CancellationRequestService cancellationRequestService,
            RefundRequestService refundRequestService,
            FeeCalculationService feeCalculationService,
            OrderService orderService,
            ServiceApplicationService serviceApplicationService,
            ServiceSubscriptionService serviceSubscriptionService,
            ProEngagementSettings proEngagementSettings,
            JobApplicationService jobApplicationService,
            IndividualProfileService individualProfileService,
            ServiceProfileService serviceProfileService,
            ChargeService chargeService,
            OrganizationAttentionService organizationAttentionService,
            IndividualAttentionService individualAttentionService)
        {
            _permissionService = permissionService;
            _blockCustomerService = blockCustomerService;
            _engagementResolver = engagementResolver;
            _cancellationRequestService = cancellationRequestService;
            _refundRequestService = refundRequestService;
            _feeCalculationService = feeCalculationService;
            _orderService = orderService;
            _serviceApplicationService = serviceApplicationService;
            _serviceSubscriptionService = serviceSubscriptionService;
            _proEngagementSettings = proEngagementSettings;
            _jobApplicationService = jobApplicationService;
            _individualProfileService = individualProfileService;
            _serviceProfileService = serviceProfileService;
            _chargeService = chargeService;
            _organizationAttentionService = organizationAttentionService;
            _individualAttentionService = individualAttentionService;
        }

        public void CancelEngagement(int engagementId, EngagementType engagementType, int actorId, string actorName, int reasonId, string userRemarks, PostCancellationAction postCancellationAction = PostCancellationAction.Rehire, DateTime? submissionDate = null, int? attachmentId = null)
        {
            // check whether cancellation already present for engagement
            var cancellationRequest = _cancellationRequestService.GetCancellationRequestId(engagementId, engagementType);
            if(cancellationRequest != null)
                return;

            var reason = _cancellationRequestService.GetReasonById(reasonId);
            var cancellingParty = GetCancellingParty(reason);

            var engagementService = _engagementResolver.Resolve(engagementType);

            // Update Order and Order Item / Payout
            var productTypeId = _engagementResolver.GetEngagementProductType(engagementType);

            // authorize cancellation for actor
            var engagementPartyInfo = engagementService.GetEngagingParties(engagementId);

            if(!CanActorCancel(engagementPartyInfo, actorId))
                throw new SecurityException("Not Authorized to perform Cancellation.");

            // only moderator allowed to use submissionDate
            if(actorId != (engagementPartyInfo.ModeratorId ?? 0))
                submissionDate = DateTime.UtcNow;

            // Cancel Engagement and return Success or Fail

            var cancelResult = engagementService.Cancel(engagementId, cancellingParty, actorId);

            var actorParty = EngagementParty.Moderator;

            if(actorId == engagementPartyInfo.BuyerId)
            {
                actorParty = EngagementParty.Buyer;
            }
            else if(actorId == engagementPartyInfo.SellerId)
            {
                actorParty = EngagementParty.Seller;
            }

            engagementService.UpdateCancel(engagementId, submissionDate ?? DateTime.UtcNow, userRemarks, reasonId, attachmentId, actorParty);

            if (cancellingParty == EngagementParty.Seller
                && engagementType == EngagementType.Service)
            {
                var serviceApplication = _serviceApplicationService.GetServiceApplicationById(engagementId);
                var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(serviceApplication.CustomerId);

                individualProfile.NumberOfCancellation++;
                _individualProfileService.UpdateIndividualProfile(actorId, individualProfile);

                // refund engagement if seller cancel second time
                if (individualProfile.NumberOfCancellation == 2)
                {
                    _refundRequestService.RefundProOrderItem(actorId, productTypeId, engagementId);
                    _serviceApplicationService.HasCancelledTwice(engagementId, actorId);
                }
            }

            int blockSellerForDays = 0;
            if (cancelResult && cancellingParty == EngagementParty.Seller)
                blockSellerForDays = _blockCustomerService.GetCustomerNextBlockDuration(engagementPartyInfo.SellerId);

            // Add Record to Cancellation Request
            _cancellationRequestService.CreateRequest(engagementId, engagementType, actorId, cancellingParty, reasonId, userRemarks, blockSellerForDays, postCancellationAction, attachmentId);

            if (engagementPartyInfo.IsEscrow)
            {
                if (cancellingParty == EngagementParty.Seller)
                {
                    if (engagementType == EngagementType.Job)
                    {
                        var jobApplication = _jobApplicationService.GetJobApplicationById(engagementId);

                        if (jobApplication != null
                            && jobApplication.JobProfileId > 0
                            && jobApplication.NumberOfHiring == 1)
                        {
                            if (jobApplication.IsEscrow == true)
                            {
                                //set order item open for rematch
                                var rematchingOrderItem = _orderService.SetOrderItemToOpenForRematch(productTypeId, engagementId);

                                //if no order item open for rematch mean full refund is required
                                if (rematchingOrderItem == null)
                                {
                                    var refundRequest = _refundRequestService.RefundProOrderItem(actorId, productTypeId, engagementId);
                                }
                            }

                            //extend subscription if it is a job engagement
                            var serviceSubcription = _serviceSubscriptionService.EnsureServiceSubscriptionActiveNextNoDays(
                                       actorId,
                                       engagementPartyInfo.BuyerId,
                                       SubscriptionType.ViewJobCandidateFulleProfile,
                                       _proEngagementSettings.DaysMinActivePayToViewInviteOnCancellation,
                                       jobApplication.JobProfileId
                                       );
                        }
                        else if (jobApplication.NumberOfHiring == 2
                            && jobApplication.IsEscrow == true)
                        {
                            var refundRequest = _refundRequestService.RefundProOrderItem(actorId, productTypeId, engagementId);
                        }
                    }
                    else
                    {
                        //set order item open for rematch
                        var rematchingOrderItem = _orderService.SetOrderItemToOpenForRematch(productTypeId, engagementId);

                        //if no order item open for rematch mean full refund is required
                        if (rematchingOrderItem == null)
                        {
                            var refundRequest = _refundRequestService.RefundProOrderItem(actorId, productTypeId, engagementId);

                            if(engagementType == EngagementType.Consultation)
                            {
                                //refund consultation engagement fee
                                var refundCharge = _chargeService.GetLatestCharge((int)ProductType.ConsultationEngagementMatchingFeeRefundAdminCharges, 0);
                                var consultationMatchingFeeOrderItem = _orderService.GetOrderItem((int)ProductType.ConsultationEngagementMatchingFee, engagementId);

                                var adminCharges = refundCharge.ValueType == (int)ChargeValueType.Amount ? refundCharge.Value : consultationMatchingFeeOrderItem.Price * refundCharge.Value;
                                var refundAmount = consultationMatchingFeeOrderItem.Price - adminCharges;

                                _refundRequestService.CreateRefundRequest(actorId, consultationMatchingFeeOrderItem.Id, refundAmount, adminCharges, true);

                                //create moderator payout
                                var payoutRequestDTO = new PayoutRequestDTO
                                {
                                    ProductTypeId = (int)ProductType.ModeratorFacilitateConsultationFee,
                                    RefId = engagementId
                                };
                                _feeCalculationService.ProcessPayoutRequest(actorId, actorName, payoutRequestDTO, false);
                            }
                        }
                    }
                }
                else if (cancellingParty == EngagementParty.Buyer)
                {
                    switch (engagementType)
                    {
                        case EngagementType.Consultation:
                            {
                                //create moderator payout if any
                                var payoutRequestDTO = new PayoutRequestDTO
                                {
                                    ProductTypeId = (int)ProductType.ModeratorFacilitateConsultationFee,
                                    RefId = engagementId
                                };
                                var moderatorFee = _feeCalculationService.ProcessPayoutRequest(actorId, actorName, payoutRequestDTO);

                                //create consultant payout if any
                                var consultationEngagementFeePayoutRequestDTO = new PayoutRequestDTO
                                {
                                    ProductTypeId = (int)ProductType.ConsultationEngagementFee,
                                    RefId = engagementId
                                };
                                var engagementFeePayoutRequest = _feeCalculationService.ProcessPayoutRequest(actorId, actorName, consultationEngagementFeePayoutRequestDTO);

                                //create admin charges if any
                                var adminCharges = _feeCalculationService.CalculateAdminCharges(ProductType.ConsultationBuyerCancellationAdminCharges, engagementId);

                                //create refund request if any
                                var refundRequest = _refundRequestService.RefundProOrderItem(actorId, productTypeId, engagementId, adminCharges.Fee, engagementFeePayoutRequest?.Fee??0);
                            }
                            break;
                        case EngagementType.Job:
                            {
                                //create admin charges if any
                                var adminCharges = _feeCalculationService.CalculateAdminCharges(ProductType.JobBuyerCancellationAdminCharges, engagementId);

                                //create refund request if any
                                var refundRequest = _refundRequestService.RefundProOrderItem(actorId, productTypeId, engagementId, adminCharges.Fee);

                                var jobApplication = _jobApplicationService.GetJobApplicationById(engagementId);

                                _serviceSubscriptionService.StopAllActiveServiceSubscription(actorId, SubscriptionType.ViewJobCandidateFulleProfile, jobApplication.JobProfileId);
                            }
                            break;
                        case EngagementType.Service:
                            {
                                //create admin charges if any
                                var adminCharges = _feeCalculationService.CalculateAdminCharges(ProductType.ServiceBuyerCancellationAdminCharges, engagementId);

                                //create refund request if any
                                var refundRequest = _refundRequestService.RefundProOrderItem(actorId, productTypeId, engagementId, adminCharges.Fee);
                            }
                            break;
                    }
                }
            }

            if(engagementType == EngagementType.Job
                && actorId != engagementPartyInfo.BuyerId)
            {
                _organizationAttentionService.ClearOrganizationAttentionCache(engagementPartyInfo.BuyerId);
            }

            if (engagementType == EngagementType.Job
                && actorId != engagementPartyInfo.SellerId)
            {
                _individualAttentionService.ClearIndividualAttentionCache(engagementPartyInfo.SellerId);
            }
        }

        public void CloseCancellationRequest(int cancellationRequestId, int actorId, string adminRemarks = null, int blockUserDays = 0, int? attachmentId = null)
        {
            var result = _cancellationRequestService.Close(cancellationRequestId, actorId, adminRemarks, blockUserDays, attachmentId);

            if (result && blockUserDays > 0)
            {
                var cancellationRequest = _cancellationRequestService.GetCancellationRequestEntityById(cancellationRequestId);
                _blockCustomerService.CreateBlockCustomer(
                    cancellationRequest.EngagementPartyInfo.SellerId,
                    cancellationRequest.Id,
                    ENGAGEMENT_ACTION,
                    blockUserDays
                );
            }
        }

        public void CloseCancellationRequest(int engagementId, EngagementType engagementType, int actorId, string adminRemarks = null, int blockUserDays = 0, int? attachmentId = null)
        {
            var cancellationId = _cancellationRequestService.GetCancellationRequestId(engagementId, engagementType);
            if(cancellationId == default)
                return;

            CloseCancellationRequest(cancellationId.Value, actorId, adminRemarks, blockUserDays, attachmentId);
        }

        public void UpdateCancellationRequest(int engagementId, EngagementType engagementType, int actorId, string adminRemarks = null, int blockUserDays = 0, int? attachmentId = null)
        {
            var cancellationId = _cancellationRequestService.GetCancellationRequestId(engagementId, engagementType);
            if (cancellationId == default)
                return;

            UpdateCancellationRequest(cancellationId.Value, actorId, adminRemarks, blockUserDays, attachmentId);
        }

        public void UpdateCancellationRequest(int cancellationRequestId, int actorId, string adminRemarks = null, int blockUserDays = 0, int? attachmentId = null)
        {
            _cancellationRequestService.Update(cancellationRequestId, actorId, adminRemarks, blockUserDays, attachmentId);
        }

        public void AddAttachmentAfterClose(int cancellationRequestId, int actorId, int? attachmentId)
        {
            _cancellationRequestService.AddAttachmentAfterClose(cancellationRequestId, actorId, attachmentId);
        }

        public PagedList<CancellationRequestDTO> GetCancellationRequests(
            DateTime? searchDate, int searchType, string searchBuyer, int searchCancelledBy,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null)
        {
            return _cancellationRequestService.GetCancellationRequest(searchDate, searchType, searchBuyer, searchCancelledBy, pageIndex, pageSize, keyword);
        }

        public EngagementParty GetCancellingParty(Reason reason)
        {
            if(reason == null)
                throw new ArgumentNullException($"Unable to determine Cancelling Party from {nameof(reason)}");

            if(reason.EngagementParty == EngagementParty.Seller || reason.BlameSeller)
                return EngagementParty.Seller;

            return EngagementParty.Buyer;
        }

        public bool CanActorCancel(EngagementPartyInfo partyInfo, int actorId)
        {
            var authorizedFacilitateAllSession = _permissionService.Authorize(StandardPermissionProvider.FacilitateAllSession);

            return
                authorizedFacilitateAllSession
                || actorId == partyInfo.BuyerId
                || actorId == partyInfo.SellerId
                || actorId == (partyInfo.ModeratorId ?? 0);
        }
    }
}