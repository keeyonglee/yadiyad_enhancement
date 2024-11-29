using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Services.Events;
using Nop.Services.Orders;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Core.Domain.DepositRequest;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Services.Services.Campaign;
using YadiYad.Pro.Services.Services.Engagement;
using YadiYad.Pro.Services.Individual;
using Nop.Core.Domain.Documents;

namespace YadiYad.Pro.Services.Order
{
    public class OrderProcessingService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly OrderService _orderService;
        private readonly InvoiceService _invoiceService;
        private readonly JobProfileService _jobProfileService;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly ServiceSubscriptionService _serviceSubscriptionService;
        private readonly ConsultationInvitationService _consultationInvitationService;
        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly DepositRequestService _depositRequestService;
        private readonly ChargeService _chargeService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly RefundRequestService _refundRequestService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IndividualProfileService _individualProfileService;
        
        List<List<ProductType>> _productTypeGrouppedByAcccount = new List<List<ProductType>> {
            new List<ProductType> {  ProductType.ApplyJobSubscription, ProductType.ViewJobCandidateFullProfileSubscription, ProductType.ConsultationEngagementMatchingFee, ProductType.ServiceEnagegementMatchingFee},
            //new List<ProductType> { ProductType.ConsultationEngagementFee, ProductType.ServiceEnagegementFee, ProductType.JobEnagegementFee },
        };

        #endregion

        #region Ctor

        public OrderProcessingService(
            IWorkContext workContext,
            OrderService orderService,
            InvoiceService invoiceService,
            JobProfileService jobProfileService,
            JobSeekerProfileService jobSeekerProfileService,
            JobApplicationService jobApplicationService,
            ServiceSubscriptionService serviceSubscriptionService,
            ConsultationInvitationService consultationInvitationService,
            ServiceApplicationService serviceApplicationService,
            ServiceProfileService serviceProfileService,
            ChargeService chargeService,
            DepositRequestService depositRequestService,
            ProWorkflowMessageService proWorkflowMessageService,
            RefundRequestService refundRequestService,
            IEventPublisher eventPublisher,
            IndividualProfileService individualProfileService)
        {
            _workContext = workContext;
            _orderService = orderService;
            _invoiceService = invoiceService;
            _jobProfileService = jobProfileService;
            _jobSeekerProfileService = jobSeekerProfileService;
            _jobApplicationService = jobApplicationService;
            _serviceSubscriptionService = serviceSubscriptionService;
            _consultationInvitationService = consultationInvitationService;
            _serviceApplicationService = serviceApplicationService;
            _serviceProfileService = serviceProfileService;
            _chargeService = chargeService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _depositRequestService = depositRequestService;
            _refundRequestService = refundRequestService;
            _eventPublisher = eventPublisher;
            _individualProfileService = individualProfileService;
        }


        #endregion

        #region Methods

        public OrderDTO CreateOrder(SubmitOrderDTO dto)
        {
            var actorId = _workContext.CurrentCustomer.Id;
            decimal fee = 0m;
            var refId = dto.RefId;
            var productTypeId = dto.ProductTypeId;
            dynamic moreInfo = null;
            var newOrderItems = new List<OrderItemDTO>();
            var buyerCustomerId = actorId;
            DepositRequestDTO depositRequest = null;

            //if paid for deposit request
            if (productTypeId == 0 && dto.DepositRequestId > 0)
            {
                depositRequest = _depositRequestService.GetDepositRequestById(dto.DepositRequestId);

                if (depositRequest == null)
                {
                    throw new KeyNotFoundException("Deposit request not found.");
                }

                productTypeId = (int)ProductType.DepositRequest;
                refId = dto.DepositRequestId;
            }

            var orderItemForOffset = _orderService.GetOrderItemBForOffsetByProductTypeRefId(actorId, (ProductType)productTypeId, refId);

            switch (productTypeId)
            {
                case (int)ProductType.ApplyJobSubscription:
                    {
                        var record = _serviceProfileService.GetServiceProfileById(refId);
                        var charge = _chargeService.GetLatestCharge((int)ProductType.ApplyJobSubscription, 0);
                        fee = charge.Value;
                        refId = actorId;

                        moreInfo = new
                        {
                            ChargeId = charge.Id,
                            ValidityDays = charge.ValidityDays,
                        };

                        newOrderItems.Add(new OrderItemDTO
                        {
                            ProductTypeId = dto.ProductTypeId,
                            RefId = refId,
                            ItemName = $"{((ProductType)dto.ProductTypeId).GetDescription()}",
                            Quantity = 1,
                            UnitPrice = fee,
                            Tax = 0m,
                            Price = fee * 1,
                            Discount = 0m,
                        });
                    }
                    break;
                case (int)ProductType.ViewJobCandidateFullProfileSubscription:
                    {
                        var record = _jobProfileService.GetJobProfileById(refId);
                        var charge = _chargeService.GetLatestCharge((int)ProductType.ViewJobCandidateFullProfileSubscription, record.JobType, record.PayAmount);

                        fee = record.FeePerDepositRequest;

                        if (charge.ValueType == (int)ChargeValueType.Amount)
                        {
                            fee = charge.Value;
                        }
                        else
                        {
                            fee = fee * charge.Value;
                        }

                        moreInfo = new
                        {
                            ServiceProfileServiceTypeId = record.JobType,
                            ChargeId = charge.Id,
                            ValidityDays = charge.ValidityDays,
                            ChargeValueType = charge.ValueType,
                            Value = charge.Value,
                        };
                        newOrderItems.Add(new OrderItemDTO
                        {
                            ProductTypeId = dto.ProductTypeId,
                            RefId = refId,
                            ItemName = $"{((ProductType)dto.ProductTypeId).GetDescription()} - {record.JobTitle}",
                            Quantity = 1,
                            UnitPrice = fee,
                            Tax = 0m,
                            Price = fee * 1,
                            Discount = 0m,
                        });
                    }
                    break;
                case (int)ProductType.ConsultationEngagementFee:
                    {
                        var charge = _chargeService.GetLatestCharge((int)ProductType.ConsultationEngagementMatchingFee, 0);
                        var consultationInvitation = _consultationInvitationService.GetConsultationInvitationById(refId);
                        buyerCustomerId = consultationInvitation.OrganizationProfile.CustomerId;
                        if (charge.ValueType == (int)ChargeValueType.Amount)
                        {
                            fee = charge.Value;
                        }
                        else
                        {
                            fee = consultationInvitation.RatesPerSession.Value * charge.Value;
                        }
                        moreInfo = new
                        {
                            ChargeId = charge.Id,
                            ValidityDays = charge.ValidityDays,
                            ChargeValueType = charge.ValueType,
                            Value = charge.Value,
                        };

                        //if no item to be offset add matching fee
                        if (orderItemForOffset == null)
                        {
                            newOrderItems.Add(new OrderItemDTO
                            {
                                ProductTypeId = (int)ProductType.ConsultationEngagementMatchingFee,
                                RefId = refId,
                                ItemName = $"{ProductType.ConsultationEngagementMatchingFee.GetDescription()}",
                                Quantity = 1,
                                UnitPrice = fee,
                                Tax = 0m,
                                Price = fee * 1,
                                Discount = 0m,
                            });
                        }

                        var professionalFeeOrderItem = new OrderItemDTO
                        {
                            ProductTypeId = (int)ProductType.ConsultationEngagementFee,
                            RefId = refId,
                            ItemName = $"{ProductType.ConsultationEngagementFee.GetDescription()}",
                            Quantity = 1,
                            UnitPrice = consultationInvitation.RatesPerSession.Value,
                            Tax = 0m,
                            Price = consultationInvitation.RatesPerSession.Value * 1,
                            Discount = 0m,
                        };

                        newOrderItems.Add(professionalFeeOrderItem);

                        //add offsert item if any
                        if (orderItemForOffset != null)
                        {
                            AddOffsetItem(
                                professionalFeeOrderItem,
                                orderItemForOffset,
                                newOrderItems);
                        }
                    }
                    break;
                case (int)ProductType.ServiceEnagegementFee:
                    {
                        var record = _serviceApplicationService.GetServiceApplicationById(refId);
                        var charge = _chargeService.GetLatestCharge((int)ProductType.ServiceEnagegementMatchingFee, record.ServiceProfileServiceTypeId);
                        var monthlyPayment = 0m;
                        var escrowAmount = 0m;
                        var finalServiceFeeAmount = 0m;
                        buyerCustomerId = record.CustomerId;

                        monthlyPayment = record.FeePerDepositExclSST;

                        _serviceApplicationService.RehireServiceApplication(refId, record.CustomerId);
                        _serviceApplicationService.SetHiredTime(refId);
                        //check escrow
                        if (record.IsEscrow || depositRequest != null)
                        {
                            escrowAmount = monthlyPayment;
                        }

                        if (depositRequest == null)
                        {
                            //check charge type 
                            if (charge.ValueType == (int)ChargeValueType.Amount)
                            {
                                finalServiceFeeAmount = charge.Value;
                                moreInfo = new
                                {
                                    ServiceProfileServiceTypeId = record.ServiceProfileServiceTypeId,
                                    ChargeId = charge.Id,
                                    ChargeValueType = charge.ValueType,
                                    Value = charge.Value,
                                };
                            }
                            else
                            {
                                finalServiceFeeAmount = monthlyPayment * charge.Value;
                                moreInfo = new
                                {
                                    ServiceProfileServiceTypeId = record.ServiceProfileServiceTypeId,
                                    ChargeId = charge.Id,
                                    ChargeValueType = charge.ValueType,
                                    Value = charge.Value,
                                    MonthlyPayment = monthlyPayment,
                                };
                            }

                            if (orderItemForOffset == null)
                            {
                                newOrderItems.Add(new OrderItemDTO
                                {
                                    ProductTypeId = (int)ProductType.ServiceEnagegementMatchingFee,
                                    RefId = refId,
                                    ItemName = $"{ProductType.ServiceEnagegementMatchingFee.GetDescription()}",
                                    Quantity = 1,
                                    UnitPrice = finalServiceFeeAmount,
                                    Tax = 0m,
                                    Price = finalServiceFeeAmount * 1,
                                    Discount = 0m,
                                });
                            }
                        }
                        if (escrowAmount != 0)
                        {
                            //escrowProductTypeId = (int)ProductType.ServiceEscrow;
                            var professionalFeeOrderItem = new OrderItemDTO
                            {
                                ProductTypeId = (int)ProductType.ServiceEnagegementFee,
                                RefId = refId,
                                ItemName = $"{ProductType.ServiceEnagegementFee.GetDescription()}",
                                Quantity = 1,
                                UnitPrice = escrowAmount,
                                Tax = 0m,
                                Price = escrowAmount * 1,
                                Discount = 0m,
                            };

                            newOrderItems.Add(professionalFeeOrderItem);


                            if (depositRequest == null && orderItemForOffset != null)
                            {
                                AddOffsetItem(
                                    professionalFeeOrderItem,
                                    orderItemForOffset,
                                    newOrderItems);
                            }
                        }
                    }
                    break;
                case (int)ProductType.JobEnagegementFee:
                    {
                        var record = _jobApplicationService.GetJobApplicationById(refId);
                        var jobProfile = _jobProfileService.GetJobProfileById(record.JobProfileId);
                        buyerCustomerId = jobProfile.CustomerId;

                        if (record.JobProfile.Status != (int)JobProfileStatus.Publish)
                        {
                            throw new InvalidOperationException("The job ads is non long open for hiring.");
                        }

                        fee = record.FeePerDepositExclSST;

                        moreInfo = new
                        {
                            PayAmount = fee,
                        };

                        var professionalFeeOrderItem = new OrderItemDTO
                        {
                            ProductTypeId = (int)ProductType.JobEnagegementFee,
                            RefId = refId,
                            ItemName = $"{((ProductType)dto.ProductTypeId).GetDescription()} - {jobProfile.JobTitle} - {record.JobSeekerProfile.Name}",
                            Quantity = 1,
                            UnitPrice = fee,
                            Tax = 0m,
                            Price = fee * 1,
                            Discount = 0m,
                        };

                        newOrderItems.Add(professionalFeeOrderItem);

                        if (orderItemForOffset != null)
                        {
                            AddOffsetItem(
                                professionalFeeOrderItem,
                                orderItemForOffset,
                                newOrderItems);
                        }
                    }
                    break;
                case (int)ProductType.DepositRequest:
                    {
                        //as deposit request have calculate so no need calculate second time.
                        fee = depositRequest.Amount;

                        var depositRequestOrderItem = new OrderItemDTO
                        {
                            ProductTypeId = (int)ProductType.DepositRequest,
                            RefId = dto.DepositRequestId,
                            ItemName = $"{((ProductType)depositRequest.ProductTypeId).GetDescription()}",
                            Quantity = 1,
                            UnitPrice = fee,
                            Tax = 0m,
                            Price = fee * 1,
                            Discount = 0m,
                        };

                        newOrderItems.Add(depositRequestOrderItem);
                    }
                    break;
            }

            //construct order
            var orderTax = newOrderItems.Sum(x => x.Tax);
            var orderDiscount = newOrderItems.Sum(x => x.Discount);
            var orderTotal = newOrderItems.Sum(x => x.Price) + orderTax - orderDiscount;

            var newOrder = new OrderDTO
            {
                CustomerId = buyerCustomerId,
                OrderStatusId = (int)OrderStatus.Pending,
                PaymentStatusId = (int)PaymentStatus.Pending,
                CustomerCurrencyCode = "MYR",
                OrderTax = orderTax,
                OrderDiscount = orderDiscount,
                OrderTotal = orderTotal,
                CustomOrderNumber = _orderService.GenerateOrderCustomNumber(),
                MoreInfo = moreInfo != null ? JsonConvert.SerializeObject(moreInfo, Formatting.Indented) : null,
                OrderItems = newOrderItems,
            };
            var createdOrder = _orderService.CreateOrder(actorId, buyerCustomerId, newOrder);


            //set pro order item id to deposit request
            if (depositRequest != null)
            {
                var orderItem = newOrder.OrderItems
                    .Where(x => x.ProductTypeId == (int)ProductType.DepositRequest
                    && x.RefId == refId)
                    .First();

                _depositRequestService.SetDepositRequestOrderItem(actorId, dto.DepositRequestId, orderItem.Id);
            }

            return newOrder;
        }

        public void ProcessOrder(ICustomOrderEntity dto)
        {
            // update order
            var orderDTO = _orderService.GetOrderById(dto.Id);
            var actorId = orderDTO.CustomerId;

            if (orderDTO.PaymentStatusId != (int)PaymentStatus.Pending
                || orderDTO.OrderStatusId != (int)OrderStatus.Pending)
            {
                return;
            }

            if (dto.OrderStatusId != 0)
            {
                orderDTO.OrderStatusId = dto.OrderStatusId;
            }

            if (dto.PaymentStatusId != 0)
            {
                orderDTO.PaymentStatusId = dto.PaymentStatusId;
                if (dto.PaymentStatusId == (int)PaymentStatus.Paid)
                {
                    orderDTO.PaidOnUTC = DateTime.Now;
                }
            }

            if (_orderService.HasMatchedOrder(orderDTO.Id, orderDTO.PaymentStatusId, orderDTO.OrderStatusId) == false)
            {
                _orderService.UpdateOrder(actorId, orderDTO.CustomerId, orderDTO);
            }
            else
            {
                return;
            }

            // create invoice
            // split invoice
            var orderItemsWithNoInvoice = orderDTO.OrderItems.Where(x => x.InvoiceId == null).ToList();
            // split invoice
            _productTypeGrouppedByAcccount.Select(x => x.Intersect(orderItemsWithNoInvoice.Select(x => (ProductType)x.ProductTypeId).ToList()))
                .ToList()
                .ForEach(x =>
                {
                    var orderItemIds = orderDTO.OrderItems.Where(y => x.Contains((ProductType)y.ProductTypeId)).Select(x => x.Id).ToList();
                    var newInvoice = new InvoiceDTO
                    {
                        OrderId = orderDTO.Id,
                        InvoiceTo = orderDTO.CustomerId,
                        InvoiceRefType = InvoiceRefType.Order
                    };
                    var createdInvoice = _invoiceService.CreateInvoice(actorId, newInvoice, RunningNumberType.ProInvoice);
                    _orderService.UpdateOrderItem(actorId, orderDTO.CustomerId, orderItemIds, createdInvoice.Id);
                });

            // activate product
            if (dto.PaymentStatusId != 0
                && orderDTO.OrderStatusId == (int)OrderStatus.Complete)
            {
                var paidedOrderItems = orderDTO.OrderItems.Where(x => x.Price >= 0).ToList();

                foreach (var orderItem in paidedOrderItems)
                {
                    switch (orderItem.ProductTypeId)
                    {
                        case (int)ProductType.ApplyJobSubscription:
                            {
                                var moreInfo = JsonConvert.DeserializeObject<MoreInfoDTO>((string)orderDTO.MoreInfo);
                                _serviceSubscriptionService.CreateServiceSubscription(actorId, orderDTO.CustomerId, SubscriptionType.ApplyJob, moreInfo.ValidityDays);
                            }
                            break;
                        case (int)ProductType.ViewJobCandidateFullProfileSubscription:
                            {
                                var moreInfo = JsonConvert.DeserializeObject<MoreInfoDTO>((string)orderDTO.MoreInfo);
                                var jobProfile = _jobProfileService.GetJobProfileById(orderItem.RefId);

                                if (jobProfile.Status == (int)JobProfileStatus.Draft)
                                {
                                    _jobProfileService.PublishJobProfile(actorId, orderItem.RefId);
                                }
                                var subs = _serviceSubscriptionService.CreateServiceSubscription(actorId, orderDTO.CustomerId, SubscriptionType.ViewJobCandidateFulleProfile, moreInfo.ValidityDays, orderItem.RefId);
                                _eventPublisher.Publish(new ExtendPviEvent(orderDTO.CustomerId, subs.Id));
                            }
                            break;
                        case (int)ProductType.ConsultationEngagementFee:
                            {
                                var consultationInvitation = _consultationInvitationService.GetConsultationInvitationById(orderItem.RefId);
                                consultationInvitation.ConsultantAvailableTimeSlots = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(consultationInvitation.ConsultantAvailableTimeSlot ?? "null");

                                //update engagement status
                                consultationInvitation.ConsultationApplicationStatus = (int)ConsultationInvitationStatus.Paid;
                                _consultationInvitationService.UpdateConsultationInvitation(actorId, consultationInvitation);

                                //create deposit for 1 engagement fee
                                _depositRequestService.CreatePaidDepositRequest(actorId, new DepositRequest
                                {
                                    OrderItemId = orderItem.Id,
                                    Amount = orderItem.Price,
                                    DepositTo = consultationInvitation.IndividualCustomerId,
                                    DepositFrom = orderDTO.CustomerId,
                                    CycleStart = consultationInvitation.ConsultantAvailableTimeSlots.Select(x => x.StartDate).Min(),
                                    CycleEnd = consultationInvitation.ConsultantAvailableTimeSlots.Select(x => x.EndDate).Max(),
                                    ProductTypeId = orderItem.ProductTypeId,
                                    RefId = orderItem.RefId
                                });

                                //process rematching order item if any
                                var orderItemForOffset = ProcessRematchingOrderItemIfAny(actorId, ProductType.ConsultationEngagementFee, orderItem.RefId);
                                ProcessRefundIfAny(actorId, orderItem, orderItemForOffset);

                                //send email
                                _proWorkflowMessageService.SendConsultationCandidatePaid(_workContext.WorkingLanguage.Id, consultationInvitation);
                            }
                            break;
                        case (int)ProductType.ServiceEnagegementFee:
                            {
                                var serviceApplication = _serviceApplicationService.GetServiceApplicationById(orderItem.RefId);
                                var serviceProfile = _serviceProfileService.GetServiceProfileById(serviceApplication.ServiceProfileId);

                                //update engagement status
                                _serviceApplicationService.UpdateServiceApplicationStatus(orderItem.RefId, actorId, ServiceApplicationStatus.Paid);

                                //create deposit for 1 engagement fee
                                _depositRequestService.CreatePaidDepositRequest(actorId, new DepositRequest
                                {
                                    OrderItemId = orderItem.Id,
                                    Amount = orderItem.Price,
                                    DepositTo = serviceProfile.CustomerId,
                                    DepositFrom = orderDTO.CustomerId,
                                    CycleStart = serviceApplication.StartDate,
                                    CycleEnd = serviceApplication.StartDate.AddMonths(1).AddDays(-1),
                                    ProductTypeId = orderItem.ProductTypeId,
                                    RefId = orderItem.RefId
                                });

                                //process rematching order item if any
                                var orderItemForOffset = ProcessRematchingOrderItemIfAny(actorId, ProductType.ServiceEnagegementFee, orderItem.RefId);
                                ProcessRefundIfAny(actorId, orderItem, orderItemForOffset);

                                //send email
                                _proWorkflowMessageService.SendServiceBuyerConfirm(orderItem.RefId, _workContext.WorkingLanguage.Id);
                                _proWorkflowMessageService.SendServiceSellerConfirm(orderItem.RefId, _workContext.WorkingLanguage.Id);
                            }
                            break;
                        case (int)ProductType.JobEnagegementFee:
                            {
                                var jobApplication = _jobApplicationService.GetJobApplicationById(orderItem.RefId);
                                var jobSeekerProfile = _jobSeekerProfileService.GetJobSeekerProfileById(jobApplication.JobSeekerProfileId);

                                if (jobApplication.JobProfile.Status != (int)JobProfileStatus.Publish)
                                {
                                    throw new InvalidOperationException("The job ads is non long open for hiring.");
                                }

                                //update engagement status to hired
                                var updatedJobApplication = _jobApplicationService.UpdateJobApplicationStatus
                                    (actorId,
                                    orderItem.RefId,
                                    JobApplicationStatus.Hired);

                                if (updatedJobApplication.NumberOfHiring == 2)
                                {
                                    //stop PVI subscription if it is 2nd hiring for same job profile within the subscription
                                    _serviceSubscriptionService.StopAllActiveServiceSubscription(
                                        actorId,
                                        SubscriptionType.ViewJobCandidateFulleProfile,
                                        jobApplication.JobProfileId);
                                }

                                _jobProfileService.UpdateJobProfileToHiredStatus(actorId, jobApplication.JobProfileId);

                                var depositRequestDTO = _depositRequestService.GetDepositRequestByProductTypeRefId(0, orderItem.RefId, orderItem.ProductTypeId);

                                if (depositRequestDTO == null)
                                {
                                    //create deposit for 1 engagement fee
                                    _depositRequestService.CreatePaidDepositRequest(actorId, new DepositRequest
                                    {
                                        OrderItemId = orderItem.Id,
                                        Amount = orderItem.Price,
                                        DepositTo = jobSeekerProfile.CustomerId,
                                        DepositFrom = orderDTO.CustomerId,
                                        CycleStart = jobApplication.JobType == (int)JobType.ProjectBased ? null : (DateTime?)jobApplication.StartDate.Value,
                                        CycleEnd = jobApplication.JobType == (int)JobType.ProjectBased ? null : (DateTime?)jobApplication.StartDate.Value.AddMonths(1).AddDays(-1),
                                        ProductTypeId = orderItem.ProductTypeId,
                                        RefId = orderItem.RefId
                                    });
                                }
                                else
                                {
                                    _depositRequestService.UpdateDepositRequest(actorId, depositRequestDTO.Id, orderItem.Id, (int)DepositRequestStatus.Paid);
                                }

                                //process rematching order item if any
                                var orderItemForOffset = ProcessRematchingOrderItemIfAny(actorId, ProductType.JobEnagegementFee, orderItem.RefId);

                                ProcessRefundIfAny(actorId, orderItem, orderItemForOffset);

                                //send email
                                _proWorkflowMessageService.SendIndividualJobApplicationHire(orderItem.RefId, _workContext.WorkingLanguage.Id);
                            }
                            break;
                        case (int)ProductType.DepositRequest:
                            {
                                _depositRequestService.UpdateDepositRequest(actorId, orderItem.RefId, orderItem.Id, (int)DepositRequestStatus.Paid);
                            }
                            break;
                    }
                }
            }
        }

        public OrderItemDTO ProcessRematchingOrderItemIfAny(int actorId, ProductType productType, int refId)
        {
            var orderItemForOffset = _orderService.GetOrderItemBForOffsetByProductTypeRefId(actorId, productType, refId);

            if (orderItemForOffset != null)
            {
                _orderService.UpdateOrderItem(actorId, orderItemForOffset.Id, ProOrderItemStatus.Remtached);
            }

            return orderItemForOffset;
        }

        public void ProcessRefundIfAny(int actorId,
            OrderItemDTO professionalFeeOrderItem,
            OrderItemDTO orderItemForOffset)
        {
            if (orderItemForOffset != null)
            {
                decimal valueToOffset = GetOffsetAmount(
                        professionalFeeOrderItem,
                        orderItemForOffset);

                var refundingAmount = orderItemForOffset.Price - valueToOffset;

                if (refundingAmount > 0)
                {
                    _refundRequestService.CreateRefundRequest(
                        actorId,
                        orderItemForOffset.Id,
                        refundingAmount);
                }
            }
        }

        public RefundRequestDTO ProcessRefundIfAny(int actorId, ProductType productType, int refId)
        {
            var orderItemForOffset = _orderService.GetOrderItemBForOffsetByProductTypeRefId(actorId, productType, refId);

            if (orderItemForOffset != null)
            {
                _orderService.SetOpenForRematchedOrderItemToPaid(actorId, orderItemForOffset.Id);
                var refundingAmount = orderItemForOffset.Price;

                if (refundingAmount > 0)
                {
                    var dto = _refundRequestService.CreateRefundRequest(
                        actorId,
                        orderItemForOffset.Id,
                        refundingAmount);

                    return dto;
                }
            }

            return null;
        }

        /// <summary>
        /// add offset item
        /// </summary>
        /// <param name="professionalFeeOrderItem"></param>
        /// <param name="orderItemForOffset"></param>
        /// <param name="newOrderItems"></param>
        private void AddOffsetItem(
            OrderItemDTO professionalFeeOrderItem,
            OrderItemDTO orderItemForOffset,
            List<OrderItemDTO> newOrderItems)
        {
            decimal valueToOffset = GetOffsetAmount(
                    professionalFeeOrderItem,
                    orderItemForOffset);

            valueToOffset = -1 * valueToOffset;

            var offsetProOrderItem = new OrderItemDTO
            {
                ProductTypeId = orderItemForOffset.ProductTypeId,
                RefId = professionalFeeOrderItem.RefId,
                Quantity = 1,
                UnitPrice = valueToOffset,
                Tax = 0m,
                Price = valueToOffset,
                Discount = 0m,
                OffsetProOrderItemId = orderItemForOffset.Id,
                EngagementCode = orderItemForOffset.EngagementCode
            };

            switch ((ProductType)orderItemForOffset.ProductTypeId)
            {
                case ProductType.JobEnagegementFee:
                    offsetProOrderItem.ItemName = $"Offset profession fee from job engagement, {orderItemForOffset.EngagementCode}";
                    break;
                case ProductType.ServiceEnagegementFee:
                    offsetProOrderItem.ItemName = $"Offset profession fee from service engagement, {orderItemForOffset.EngagementCode}";
                    break;
                case ProductType.ConsultationEngagementFee:
                    offsetProOrderItem.ItemName = $"Offset profession fee from consultation engagement, {orderItemForOffset.EngagementCode}";
                    break;
            }

            newOrderItems.Add(offsetProOrderItem);
        }

        private decimal GetOffsetAmount(
            OrderItemDTO professionalFeeOrderItem,
            OrderItemDTO orderItemForOffset)
        {
            var valueEligiableToOffset = professionalFeeOrderItem.Price;
            var valueAvailableToOffset = orderItemForOffset.Price;
            decimal valueToOffset = 0;

            if (valueAvailableToOffset >= valueEligiableToOffset)
            {
                valueToOffset = valueEligiableToOffset;
            }
            else
            {
                valueToOffset = valueAvailableToOffset;
            }

            return valueToOffset;
        }

        #endregion

    }
}
