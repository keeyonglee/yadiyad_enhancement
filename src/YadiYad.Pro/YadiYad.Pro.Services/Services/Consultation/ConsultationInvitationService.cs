using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Infrastructure.Cache;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Moderator;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Extension;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Attentions;
using YadiYad.Pro.Services.Services.Base;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Services.Moderator;
using TimeZone = YadiYad.Pro.Core.Domain.Common.TimeZone;

namespace YadiYad.Pro.Services.Consultation
{
    public class ConsultationInvitationService : BaseService, IEngagementService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<ConsultationProfile> _ConsultationProfileRepository;
        private readonly IRepository<ConsultationInvitation> _consultationInvitationRepository;
        private readonly IRepository<ServiceProfile> _ServiceProfileRepository;
        private readonly IRepository<OrganizationProfile> _OrganizationProfileRepository;
        private readonly IRepository<BusinessSegment> _BusinessSegmentRepository;
        private readonly IRepository<TimeZone> _TimeZoneRepository;
        private readonly IRepository<ServiceExpertise> _serviceExpertiseRepository;
        private readonly IRepository<Expertise> _expertiseRepository;
        private readonly IRepository<JobServiceCategory> _jobServiceCategoryRepository;
        private readonly IRepository<IndividualProfile> _individualeRepository;
        private readonly OrderService _orderService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly ChargeService _chargeService;
        private readonly ModeratorCancellationRequestService _moderatorCancellationRequestService;
        private readonly BlockCustomerService _blockCustomerService;
        private readonly ConsultationJobSettings _consultationJobSettings;
        private readonly IndividualAttentionService _individualAttentionService;
        private readonly OrganizationAttentionService _organizationAttentionService;

        public EngagementType EngagementType => EngagementType.Consultation;
        public EngagementPartyTypeInfo EngagementPartyTypeInfo => new EngagementPartyTypeInfo
        {
            Buyer = "Organization",
            Seller = "Consultant",
            Moderator = "Moderator"
        };
        #endregion

        #region Ctor

        public ConsultationInvitationService
            (IMapper mapper,
            ConsultationJobSettings consultationJobSettings,
            IDateTimeHelper dateTimeHelper,
            IRepository<IndividualProfile> individualeRepository,
            IRepository<ServiceExpertise> serviceExpertiseRepository,
            IRepository<Expertise> expertiseRepository,
            IRepository<JobServiceCategory> jobServiceCategoryRepository,
            IRepository<ConsultationProfile> ConsultationProfileRepository,
            IRepository<ConsultationInvitation> ConsultationInvitationRepository,
            IRepository<ServiceProfile> ServiceProfileRepository,
            IRepository<OrganizationProfile> OrganizationProfileRepository,
            IRepository<BusinessSegment> BusinessSegmentRepository,
            IRepository<TimeZone> TimeZoneRepository,
            OrderService orderService,
            ConsultationProfileService consultationProfileService,
            IWorkContext workContext,
            IndividualProfileService individualProfileService,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
            OrganizationProfileService organizationProfileService,
            ServiceProfileService serviceProfileService,
            ChargeService chargeService,
            ModeratorCancellationRequestService moderatorCancellationRequestService,
            IRepository<RefundRequest> refundRequestRepo,
            BlockCustomerService blockCustomerService,
            IndividualAttentionService individualAttentionService,
            OrganizationAttentionService organizationAttentionService)
        {
            _mapper = mapper;
            _consultationJobSettings = consultationJobSettings;
            _dateTimeHelper = dateTimeHelper;
            _individualeRepository = individualeRepository;
            _ConsultationProfileRepository = ConsultationProfileRepository;
            _consultationInvitationRepository = ConsultationInvitationRepository;
            _ServiceProfileRepository = ServiceProfileRepository;
            _OrganizationProfileRepository = OrganizationProfileRepository;
            _BusinessSegmentRepository = BusinessSegmentRepository;
            _TimeZoneRepository = TimeZoneRepository;
            _serviceExpertiseRepository = serviceExpertiseRepository;
            _expertiseRepository = expertiseRepository;
            _jobServiceCategoryRepository = jobServiceCategoryRepository;
            _orderService = orderService;
            _customerRepository = customerRepository;
            _customerRoleRepository = customerRoleRepository;
            _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
            _organizationProfileService = organizationProfileService;
            _serviceProfileService = serviceProfileService;
            _chargeService = chargeService;
            _moderatorCancellationRequestService = moderatorCancellationRequestService;
            _blockCustomerService = blockCustomerService;
            _individualAttentionService = individualAttentionService;
            _organizationAttentionService = organizationAttentionService;
        }

        #endregion

        #region Methods
        public List<int> GetModeratorWithLeastAssign(int size = 1)
        {
            var moderatorIds = (from c in _customerRepository.Table
                .Where(c => !c.Deleted)
                    from ccrm in _customerCustomerRoleMappingRepository.Table
                    .Where(ccrm => c.Id == ccrm.CustomerId)
                    from cr in _customerRoleRepository.Table
                    .Where(cr => cr.Active && ccrm.CustomerRoleId == cr.Id)
                    from ci in _consultationInvitationRepository.Table
                    .Where(ci => !ci.Deleted && c.Id == ci.ModeratorCustomerId).DefaultIfEmpty()
                    where cr.Name == "Moderator"
                    group ci by c.Id into g
                    select new
                    {
                        Id = g.Key,
                        Count = g.Count(x => x != null)
                    }
                )
                .OrderBy(x => x.Count)
                .Select(x => x.Id)
                .Take(size)
                .ToList();

            return moderatorIds;
        }
        public virtual void CreateConsultationInvitation(int actorId, ConsultationInvitationDTO dto)
        {
            var request = _mapper.Map<ConsultationInvitation>(dto);
            request.CreateAudit(actorId);
            var serviceProfiles = _ServiceProfileRepository.Table.Where(x => dto.ServiceProfileIds.Contains(x.Id)).ToList();
            var consultationProfile = _ConsultationProfileRepository.Table
                .Where(x => x.Deleted == false && x.Id == dto.ConsultationProfileId && !x.DeletedFromUser)
                .First();
            // assign to mederator with least ci
            var moderatorIds = GetModeratorWithLeastAssign(dto.ServiceProfileIds.Count());

            var flattenRequest = dto.ServiceProfileIds.Select((serviceProfileId, idx) =>
            {
                var serviceProfile = serviceProfiles.FirstOrDefault(x => x.Id == serviceProfileId);
                var newRequest = new ConsultationInvitation
                {
                    ServiceProfileId = serviceProfileId,
                    IndividualCustomerId = serviceProfile.CustomerId,
                    ConsultationProfileId = request.ConsultationProfileId,
                    OrganizationProfileId = consultationProfile.OrganizationProfileId,
                    ConsultationApplicationStatus = (int)ConsultationInvitationStatus.New,
                    Questionnaire = consultationProfile.Questionnaire,
                    CreatedById = request.CreatedById,
                    CreatedOnUTC = request.CreatedOnUTC,
                    UpdatedById = request.UpdatedById,
                    UpdatedOnUTC = request.UpdatedOnUTC,
                    ModeratorCustomerId = moderatorIds.ElementAt(idx % moderatorIds.Count()),
                };
                return newRequest;
            });

            _consultationInvitationRepository.Insert(flattenRequest);

            if (consultationProfile.IsApproved == true)
            {
                foreach (var serviceProfile in serviceProfiles)
                {
                    _individualAttentionService.ClearIndividualAttentionCache(serviceProfile.CustomerId);
                }
            }
        }

        public virtual void UpdateConsultationInvitation(int actorId, ConsultationInvitationDTO dto)
        {
            var model = _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == dto.Id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }
            model.UpdateAudit(actorId);

            if (dto.ConsultationApplicationStatus != 0)
            {
                model.ConsultationApplicationStatus = dto.ConsultationApplicationStatus;
            }

            if (dto.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted)
            {
                dto.IsApproved = null;
                model.IsApproved = null;
                model.QuestionnaireAnswer = dto.QuestionnaireAnswer;
                model.ConsultantAvailableTimeSlot = JsonConvert.SerializeObject(dto.ConsultantAvailableTimeSlots);
                model.RatesPerSession = dto.RatesPerSession;

            }
            else if (dto.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual)
            {
                model.DeclineReasons = dto.DeclineReasons;
            }

            if (dto.IsApproved != null)
            {
                model.IsApproved = dto.IsApproved;
            }

            _consultationInvitationRepository.Update(model);

            var consultantCustId = (from sp in _ServiceProfileRepository.Table
                                    where sp.Deleted == false
                                    && sp.Id == model.ServiceProfileId
                                    select sp.CustomerId)
                                   .FirstOrDefault();

            if (consultantCustId == 0)
            {
                throw new KeyNotFoundException("Consultant customer id not found.");
            }

            _individualAttentionService.ClearIndividualAttentionCache(consultantCustId);

            //get organization customer id
            var orgCustId = (from cp in _ConsultationProfileRepository.Table
                             where cp.Deleted == false
                             && cp.Id == model.ConsultationProfileId
                             from o in _OrganizationProfileRepository.Table
                             where o.Deleted == false
                             && o.Id == cp.OrganizationProfileId
                             select o.CustomerId)
                            .FirstOrDefault();

            if (orgCustId == 0)
            {
                throw new KeyNotFoundException("Organization customer id not found.");
            }

            _organizationAttentionService.ClearOrganizationAttentionCache(orgCustId);
        }

        public virtual void ReviewConsultationInvitation(int actorId, int id, ConsultationInvitationReviewDTO dto)
        {
            var model = _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }
            model.UpdateAudit(actorId);

            model.ReviewDateTime = DateTime.UtcNow;
            model.ReviewText = dto.ReviewText;
            model.Rating = (dto.KnowledgenessRating
                + dto.ProfessionalismRating
                + dto.RelevanceRating
                + dto.RespondingRating
                + dto.ClearnessRating) / 5.0m;
            model.KnowledgenessRating = dto.KnowledgenessRating;
            model.ProfessionalismRating = dto.ProfessionalismRating;
            model.RelevanceRating = dto.RelevanceRating;
            model.RespondingRating = dto.RespondingRating;
            model.ClearnessRating = dto.ClearnessRating;

            _consultationInvitationRepository.Update(model);
        }

        public ConsultationInvitationDTO GetConsultationInvitationById(int id)
        {
            if (id == 0)
                return null;

            var query = _consultationInvitationRepository.Table;

            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var consultationProfile = _ConsultationProfileRepository.Table.Where(x => x.Id == record.ConsultationProfileId).FirstOrDefault();
            var serviceProfile = _ServiceProfileRepository.Table.Where(x => x.Id == record.ServiceProfileId).FirstOrDefault();
            var organizationProfile = _OrganizationProfileRepository.Table.Where(x => x.Id == record.OrganizationProfileId).FirstOrDefault();
            var segmentRecord = _BusinessSegmentRepository.Table.Where(x => x.Id == consultationProfile.SegmentId).FirstOrDefault();
            var timeZoneRecord = _TimeZoneRepository.Table.Where(x => x.Id == consultationProfile.TimeZoneId).FirstOrDefault();

            var response = _mapper.Map<ConsultationInvitationDTO>(record);

            response.ConsultationProfile = _mapper.Map<ConsultationProfileDTO>(consultationProfile);
            response.ServiceProfile = _mapper.Map<ServiceProfileDTO>(serviceProfile);
            response.OrganizationProfile = _mapper.Map<OrganizationProfileDTO>(organizationProfile);
            response.SegmentName = segmentRecord.Name;
            response.TimeZoneName = timeZoneRecord.Name;

            return response;
        }

        public PagedListDTO<ConsultationInvitationDTO> GetConsultationInvitations(
            int customerId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            ConsultationInvitationListingFilterDTO filterDTO = null)
        {
            var query = from ci in _consultationInvitationRepository.Table
                        where ci.Deleted == false

                        from cp in _ConsultationProfileRepository.Table
                        .Where(cp => cp.Id == ci.ConsultationProfileId
                        && cp.Deleted == false)

                        from sp in _ServiceProfileRepository.Table
                        .Where(sp => sp.Id == ci.ServiceProfileId
                        && sp.Deleted == false)

                        from ip in _individualeRepository.Table
                        .Where(ip => ip.CustomerId == sp.CustomerId
                        && ip.Deleted == false)

                        from op in _OrganizationProfileRepository.Table
                        .Where(op => op.Id == ci.OrganizationProfileId
                        && op.Deleted == false)

                        from sg in _BusinessSegmentRepository.Table
                        .Where(sg => sg.Id == cp.SegmentId)

                        from tz in _TimeZoneRepository.Table
                        .Where(tz => tz.Id == cp.TimeZoneId)

                        from mc in _customerRepository.Table
                        .Where(mc => mc.Deleted == false
                        && mc.Id == ci.ModeratorCustomerId)
                        .DefaultIfEmpty()

                        select new ConsultationInvitationDTO
                        {
                            Id = ci.Id,
                            ServiceProfileId = ci.ServiceProfileId,
                            IndividualCustomerId = ci.IndividualCustomerId,
                            ConsultationProfileId = ci.ConsultationProfileId,
                            OrganizationProfileId = ci.OrganizationProfileId,
                            IsIndividualRead = ci.IsIndividualRead,
                            isOrganizationRead = ci.IsOrganizationRead,
                            //if response havent reviewed by moderator it will consider new/pending from org point of view.
                            ConsultationApplicationStatus = (ci.IsApproved != true && ci.ConsultationApplicationStatus != 3 && ci.ConsultationApplicationStatus != 8) ? (int)ConsultationInvitationStatus.New : ci.ConsultationApplicationStatus,
                            ConsultantAvailableTimeSlot = ci.ConsultantAvailableTimeSlot,
                            QuestionnaireAnswer = ci.QuestionnaireAnswer,
                            Questionnaire = ci.Questionnaire,
                            CreatedById = ci.CreatedById,
                            CreatedOnUTC = ci.CreatedOnUTC,
                            UpdatedById = ci.UpdatedById,
                            UpdatedOnUTC = ci.UpdatedOnUTC,
                            ModeratorCustomerId = mc.Id,
                            ModeratorEmail = mc.Email,
                            SegmentName = sg.Name,
                            TimeZoneName = tz.Name,
                            RatesPerSession = ci.RatesPerSession,
                            Objective = cp.Objective,
                            Rating = ci.Rating,
                            ClearnessRating = ci.ClearnessRating,
                            RespondingRating = ci.RespondingRating,
                            RelevanceRating = ci.RelevanceRating,
                            ProfessionalismRating = ci.ProfessionalismRating,
                            KnowledgenessRating = ci.KnowledgenessRating,
                            ReviewText = ci.ReviewText,
                            DeclineReasons = ci.DeclineReasons,
                            Topic = cp.Topic,
                            ServiceProfile = new ServiceProfileDTO
                            {
                                Id = sp.Id,
                                GenderName = ((Gender)ip.Gender).GetDescription(),
                                YearExperience = sp.YearExperience,
                                DOB = ip.DateOfBirth,
                                ConsultationCharges = sp.ServiceFee
                            },
                            OrganizationProfile = new OrganizationProfileDTO
                            {
                                Id = op.Id
                            },
                            ConsultationProfile = new ConsultationProfileDTO
                            {
                                Id = cp.Id
                            },
                            AppointmentStartDate = ci.AppointmentStartDate,
                            AppointmentEndDate = ci.AppointmentEndDate,
                            RescheduleRemarks = ci.RescheduleRemarks,
                            ConsultantName =
                                ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization
                                ? ip.FullName
                                : null
                        };

            if (filterDTO != null)
            {
                if (filterDTO.IndividualCustomerId != 0)
                {
                    query = query.Where(x => filterDTO.IndividualCustomerId == x.IndividualCustomerId);
                }
                if (filterDTO.OrganizationProfileId != 0)
                {
                    query = query.Where(x => filterDTO.OrganizationProfileId == x.OrganizationProfileId);
                }
                if (filterDTO.ServiceProfileId != 0)
                {
                    query = query.Where(x => filterDTO.ServiceProfileId == x.ServiceProfileId);
                }
                if (filterDTO.ConsultationInvitationStatuses != null && filterDTO.ConsultationInvitationStatuses.Count > 0)
                {
                    query = query.Where(x => filterDTO.ConsultationInvitationStatuses.Contains(x.ConsultationApplicationStatus));
                }
            }

            var totalCount = query.Count();

            query = query.OrderByDescending(x => x.CreatedOnUTC);

            var records = query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            //get service profile id in list
            var serviceProfileId = records.Select(x => x.ServiceProfileId).ToList();

            //get required expertise
            var consultationInvitationExpectises = _serviceExpertiseRepository.Table
                    .Where(x => x.Deleted == false
                    && serviceProfileId.Contains(x.ServiceProfileId))
                    .Join(_expertiseRepository.Table,
                    x => x.ExpertiseId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId
                        ,
                        Id = x.ExpertiseId
                        ,
                        x.OtherExpertise
                        ,
                        y.JobServiceCategoryId
                        ,
                        y.Name
                    })
                    .Select(x => new
                    {
                        ServiceProfileId = x.ServiceProfileId,
                        Id = x.Id,
                        OtherExpertise = x.OtherExpertise,
                        CategoryId = x.JobServiceCategoryId,
                        Name = x.Name
                    })
                    .ToList();

            var cancelledEngagementIds = records
                .Where(x => x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual)
                .Select(x => x.Id)
                .ToList();

            var refundableOrderItems = _orderService.GetRefundableOrderItems(ProductType.ConsultationEngagementFee, cancelledEngagementIds);

            foreach (var engagement in records)
            {
                //set category
                engagement.ServiceProfile.CategoryId = consultationInvitationExpectises
                    .Where(x=>x.ServiceProfileId == engagement.ServiceProfileId)
                    .Select(x => x.CategoryId)
                    .Distinct().FirstOrDefault();

                var category = _jobServiceCategoryRepository.Table
                                .Where(x => x.Id == engagement.ServiceProfile.CategoryId)
                                .FirstOrDefault();
                engagement.ServiceProfile.CategoryName = category?.Name;

                //set expertise
                engagement.ServiceProfile.ServiceExpertises = consultationInvitationExpectises
                    .Where(x => x.ServiceProfileId == engagement.ServiceProfileId)
                    .Select(x=>new ServiceExpertiseDTO
                    {
                        Id = x.Id,
                        OtherExpertise = x.OtherExpertise,
                        CategoryId = x.CategoryId,
                        Name = x.Name
                    })
                    .ToList();

                //translate property id to name
                engagement.ServiceProfile.ExperienceYearName = ((ExperienceYear)engagement.ServiceProfile.YearExperience).GetDescription();

                //set available time slots and replys
                engagement.ConsultantAvailableTimeSlots
                    = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(
                        engagement.ConsultantAvailableTimeSlot ?? "null");
                engagement.ConsultantReplys
                    = JsonConvert.DeserializeObject<List<QuestionDTO>>(
                        engagement.Questionnaire ?? "null");

                var answers = JsonConvert.DeserializeObject<JObject>(engagement.QuestionnaireAnswer ?? "null");

                if (answers != null)
                {
                    foreach (var reply in engagement.ConsultantReplys)
                    {
                        var answer = answers.GetValue(reply.Name);

                        try
                        {
                            reply.Answers = answer.ToObject<string>();
                        }
                        catch (ArgumentException)
                        {
                            reply.Answers = answer.ToObject<List<string>>();
                        }
                    }
                }

                engagement.CanRefund = refundableOrderItems.Any(x=>x.RefId == engagement.Id);
            }

            var responseDTOs = new PagedListDTO<ConsultationInvitationDTO>(records, pageIndex, pageSize, totalCount);

            var unreadItemIds 
                = responseDTOs.Data
                .Where(x => x.isOrganizationRead == false)
                .Select(x => x.Id)
                .ToList();

            if(unreadItemIds != null
                && unreadItemIds.Count > 0)
            {
                var consultations = _consultationInvitationRepository.Table
                    .Where(x => unreadItemIds.Contains(x.Id))
                    .ToList();

                foreach(var consultation in consultations)
                {
                    consultation.IsOrganizationRead = true;
                    _consultationInvitationRepository.Update(consultation);
                }

                _organizationAttentionService.ClearOrganizationAttentionCache(customerId);
            }

            return responseDTOs;
        }

        public IQueryable<ConsultationInvitationDTO> GetAllConsultationInvitations(bool includeModeratorEmail = false)
        {
            var query = from ci in _consultationInvitationRepository.Table
                        where ci.Deleted == false

                        from cp in _ConsultationProfileRepository.Table
                        .Where(cp => cp.Id == ci.ConsultationProfileId
                        && cp.Deleted == false)

                        from sp in _ServiceProfileRepository.Table
                        .Where(sp => sp.Id == ci.ServiceProfileId
                        && sp.Deleted == false)

                        from ip in _individualeRepository.Table
                        .Where(ip => ip.CustomerId == sp.CustomerId
                        && ip.Deleted == false)

                        from op in _OrganizationProfileRepository.Table
                        .Where(op => op.Id == ci.OrganizationProfileId
                        && op.Deleted == false)

                        from sg in _BusinessSegmentRepository.Table
                        .Where(sg => sg.Id == cp.SegmentId)

                        from tz in _TimeZoneRepository.Table
                        .Where(tz => tz.Id == cp.TimeZoneId)

                        from mc in _customerRepository.Table
                        .Where(mc=>mc.Deleted == false
                        && mc.Id == ci.ModeratorCustomerId)
                        .DefaultIfEmpty()

                        select new ConsultationInvitationDTO
                        {
                            Id = ci.Id,
                            ServiceProfileId = ci.ServiceProfileId,
                            IndividualCustomerId = ci.IndividualCustomerId,
                            ConsultationProfileId = ci.ConsultationProfileId,
                            OrganizationProfileId = ci.OrganizationProfileId,
                            IsIndividualRead = ci.IsIndividualRead,
                            isOrganizationRead = ci.IsOrganizationRead,
                            ConsultationApplicationStatus = ci.ConsultationApplicationStatus,
                            ConsultantAvailableTimeSlot = ci.ConsultantAvailableTimeSlot,
                            QuestionnaireAnswer = ci.QuestionnaireAnswer,
                            Questionnaire = ci.Questionnaire,
                            CreatedById = ci.CreatedById,
                            CreatedOnUTC = ci.CreatedOnUTC,
                            UpdatedById = ci.UpdatedById,
                            UpdatedOnUTC = ci.UpdatedOnUTC,
                            SegmentName = sg.Name,
                            TimeZoneName = tz.Name,
                            RatesPerSession = ci.RatesPerSession,
                            Objective = cp.Objective,
                            Topic = cp.Topic,
                            ModeratorCustomerId = ci.ModeratorCustomerId,
                            IsApproved = ci.IsApproved,
                            StatusRemarks = ci.StatusRemarks,
                            RescheduleRemarks = ci.RescheduleRemarks,
                            AppointmentStartDate = ci.AppointmentStartDate,
                            AppointmentEndDate = ci.AppointmentEndDate,
                            
                            ServiceProfile = new ServiceProfileDTO
                            {
                                Id = sp.Id,
                                GenderName = ((Gender)ip.Gender).GetDescription(),
                                YearExperience = sp.YearExperience,
                                DOB = ip.DateOfBirth
                            },
                            OrganizationProfile = new OrganizationProfileDTO
                            {
                                Id = op.Id,
                                Name = op.Name,
                                ContactPersonContact = op.ContactPersonContact,
                                ContactPersonEmail = op.ContactPersonEmail,
                                ContactPersonName = op.ContactPersonName,
                                ContactPersonPosition = op.ContactPersonPosition,
                                ContactPersonTitleName = ((IndividualTitle)op.ContactPersonTitle).GetDescription()
                            },
                            ConsultationProfile = new ConsultationProfileDTO
                            {
                                Id = cp.Id,
                                Duration = cp.Duration
                            },
                            IndividualProfile = new DTO.Individual.IndividualProfileDTO
                            {
                                TitleName = ((IndividualTitle)ip.Title).GetDescription(),
                                FullName = ip.FullName,
                                ContactNo = ip.ContactNo,
                                Email = ip.Email
                            },
                            ModeratorEmail = mc.Email
                        };

            return query;
        }

        public PagedListDTO<ConsultationInvitationDTO> GetAllConsultationInvitationsDetails(IQueryable<ConsultationInvitationDTO> query)
        {
            int pageIndex = 0;
            int pageSize = int.MaxValue;

            var totalCount = query.Count();
            var records = query.ToList();

            foreach (var consultationInvitation in records)
            {
                var expertises = _serviceExpertiseRepository.Table
                    .Where(x => x.Deleted == false
                    && x.ServiceProfileId == consultationInvitation.ServiceProfileId)
                    .Join(_expertiseRepository.Table,
                    x => x.ExpertiseId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId
                        ,
                        Id = x.ExpertiseId
                        ,
                        x.OtherExpertise
                        ,
                        y.JobServiceCategoryId
                        ,
                        y.Name
                    })
                    .Select(x => new ServiceExpertiseDTO
                    {
                        Id = x.Id,
                        OtherExpertise = x.OtherExpertise,
                        CategoryId = x.JobServiceCategoryId,
                        Name = x.Name
                    })
                    .ToList();

                consultationInvitation.ServiceProfile.CategoryId = expertises.Select(x => x.CategoryId).Distinct().FirstOrDefault();

                var category = _jobServiceCategoryRepository.Table
                                .Where(x => x.Id == consultationInvitation.ServiceProfile.CategoryId)
                                .FirstOrDefault();

                consultationInvitation.ServiceProfile.ExperienceYearName = ((ExperienceYear)consultationInvitation.ServiceProfile.YearExperience).GetDescription();
                consultationInvitation.ServiceProfile.CategoryName = category?.Name;
                consultationInvitation.ServiceProfile.ServiceExpertises = expertises;
                consultationInvitation.ConsultantAvailableTimeSlots
                    = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(
                        consultationInvitation.ConsultantAvailableTimeSlot ?? "null");
                consultationInvitation.ConsultantReplys
                    = JsonConvert.DeserializeObject<List<QuestionDTO>>(
                        consultationInvitation.Questionnaire ?? "null");

                var answers = JsonConvert.DeserializeObject<JObject>(consultationInvitation.QuestionnaireAnswer ?? "null");

                if (answers != null)
                {
                    foreach (var reply in consultationInvitation.ConsultantReplys)
                    {
                        var answer = answers.GetValue(reply.Name);

                        try
                        {
                            reply.Answers = answer.ToObject<string>();
                        }
                        catch (ArgumentException)
                        {
                            reply.Answers = answer.ToObject<List<string>>();
                        }
                    }
                }
            }

            var responseDTOs = new PagedListDTO<ConsultationInvitationDTO>(records, pageIndex, pageSize, totalCount);

            return responseDTOs;
        }

        public PagedListDTO<ConsultationInvitationDTO> GetServiceProfilePastConsultationInvitations(
            int serviceProfileId,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var query = from ci in _consultationInvitationRepository.Table
                        where ci.Deleted == false
                        from sp in _ServiceProfileRepository.Table
                        where sp.Id == ci.ServiceProfileId
                        && sp.Deleted == false
                        from op in _OrganizationProfileRepository.Table
                        .Where(op => op.Deleted != true
                        && op.Id == ci.OrganizationProfileId)
                        select new ConsultationInvitationDTO
                        {
                            Id = ci.Id,
                            Rating = ci.Rating,
                            ReviewText = ci.ReviewText,
                            ReviewDateTime = ci.ReviewDateTime,
                            ServiceProfileId = ci.ServiceProfileId,
                            ConsultationApplicationStatus = ci.ConsultationApplicationStatus,
                            OrganizationName = op.Name,
                            ServiceProfile = new ServiceProfileDTO
                            {
                                Id = sp.Id,
                                ServiceTypeName =
                                    sp.ServiceTypeId == (int)ServiceType.Freelancing
                                    ? ServiceType.Freelancing.GetDescription()
                                    : sp.ServiceTypeId == (int)ServiceType.PartTime
                                    ? ServiceType.PartTime.GetDescription()
                                    : sp.ServiceTypeId == (int)ServiceType.Consultation
                                    ? ServiceType.Consultation.GetDescription()
                                    : sp.ServiceTypeId == (int)ServiceType.ProjectBased
                                    ? ServiceType.ProjectBased.GetDescription()
                                    : null
                            },
                        };


            query = query.Where(x =>
                x.ServiceProfileId == serviceProfileId
                && x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                && x.Rating != null)
                .OrderByDescending(x => x.ReviewDateTime);

            var totalCount = query.Count();
            var records = query.ToList();

            var responseDTOs = new PagedListDTO<ConsultationInvitationDTO>(records, pageIndex, pageSize, totalCount);

            return responseDTOs;
        }

        public IPagedList<ConsultationInvitationDTO> GetAllConsultationInvitationsChecking(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            ConsultationReplyReviewSearchFilterDTO filterDTO = null)
        {
            var record = GetAllConsultationInvitations();

            //both accepted or declined require approval
            record = record.Where(x =>
                (x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted)
                && x.IsApproved == null).OrderByDescending(x => x.Id);
            var response = new PagedList<ConsultationInvitationDTO>(record, pageIndex, pageSize);

            foreach (var consultationInvitation in response)
            {
                var expertises = _serviceExpertiseRepository.Table
                    .Where(x => x.Deleted == false
                    && x.ServiceProfileId == consultationInvitation.ServiceProfileId)
                    .Join(_expertiseRepository.Table,
                    x => x.ExpertiseId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId
                        ,
                        Id = x.ExpertiseId
                        ,
                        x.OtherExpertise
                        ,
                        y.JobServiceCategoryId
                        ,
                        y.Name
                    })
                    .Select(x => new ServiceExpertiseDTO
                    {
                        Id = x.Id,
                        OtherExpertise = x.OtherExpertise,
                        CategoryId = x.JobServiceCategoryId,
                        Name = x.Name
                    })
                    .ToList();

                consultationInvitation.ServiceProfile.CategoryId = expertises.Select(x => x.CategoryId).Distinct().FirstOrDefault();

                var category = _jobServiceCategoryRepository.Table
                                .Where(x => x.Id == consultationInvitation.ServiceProfile.CategoryId)
                                .FirstOrDefault();

                consultationInvitation.ServiceProfile.ExperienceYearName = ((ExperienceYear)consultationInvitation.ServiceProfile.YearExperience).GetDescription();
                consultationInvitation.ServiceProfile.CategoryName = category?.Name;
                consultationInvitation.ServiceProfile.ServiceExpertises = expertises;
                consultationInvitation.ConsultantAvailableTimeSlots
                    = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(
                        consultationInvitation.ConsultantAvailableTimeSlot ?? "null");
                consultationInvitation.ConsultantReplys
                    = JsonConvert.DeserializeObject<List<QuestionDTO>>(
                        consultationInvitation.Questionnaire ?? "null");

                var answers = JsonConvert.DeserializeObject<JObject>(consultationInvitation.QuestionnaireAnswer ?? "null");

                if (answers != null)
                {
                    foreach (var reply in consultationInvitation.ConsultantReplys)
                    {
                        var answer = answers.GetValue(reply.Name);

                        try
                        {
                            reply.Answers = answer.ToObject<string>();
                        }
                        catch (ArgumentException)
                        {
                            reply.Answers = answer.ToObject<List<string>>();
                        }
                    }
                }
            }

            return response;
        }

        public IPagedList<ConsultationInvitationDTO> GetAllConsultationInvitationsComplete(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            ConsultationFacilitatingSearchFilterDTO filterDTO = null)
        {
            var record = GetAllConsultationInvitations(filterDTO.IncludeModeratorEmail);
            record = record.Where(x => 
            (x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                || x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                || x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                || x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization))
                .OrderByDescending(x => x.Id);

            if (filterDTO != null)
            {
                if (filterDTO.StatusId != 0)
                {
                    record = record.Where(x => x.ConsultationApplicationStatus == filterDTO.StatusId);

                }
                if (filterDTO.Date != null)
                {
                    record = record.Where(x => x.CreatedOnUTC <= filterDTO.Date);
                }
                if (filterDTO.ModeratorId != 0)
                {
                    record = record.Where(x => x.ModeratorCustomerId == filterDTO.ModeratorId);
                }
                if (!string.IsNullOrEmpty(filterDTO.Name))
                {
                    record = record.Where(x => x.OrganizationProfile.Name.ToLower().Contains(filterDTO.Name.ToLower()));
                }
            }

            var response = new PagedList<ConsultationInvitationDTO>(record, pageIndex, pageSize);

            foreach (var consultationInvitation in response)
            {
                var expertises = _serviceExpertiseRepository.Table
                    .Where(x => x.Deleted == false
                    && x.ServiceProfileId == consultationInvitation.ServiceProfileId)
                    .Join(_expertiseRepository.Table,
                    x => x.ExpertiseId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId
                        ,
                        Id = x.ExpertiseId
                        ,
                        x.OtherExpertise
                        ,
                        y.JobServiceCategoryId
                        ,
                        y.Name
                    })
                    .Select(x => new ServiceExpertiseDTO
                    {
                        Id = x.Id,
                        OtherExpertise = x.OtherExpertise,
                        CategoryId = x.JobServiceCategoryId,
                        Name = x.Name
                    })
                    .ToList();

                consultationInvitation.ServiceProfile.CategoryId = expertises.Select(x => x.CategoryId).Distinct().FirstOrDefault();

                var category = _jobServiceCategoryRepository.Table
                                .Where(x => x.Id == consultationInvitation.ServiceProfile.CategoryId)
                                .FirstOrDefault();

                consultationInvitation.ServiceProfile.ExperienceYearName = ((ExperienceYear)consultationInvitation.ServiceProfile.YearExperience).GetDescription();
                consultationInvitation.ServiceProfile.CategoryName = category?.Name;
                consultationInvitation.ServiceProfile.ServiceExpertises = expertises;
                consultationInvitation.ConsultantAvailableTimeSlots
                    = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(
                        consultationInvitation.ConsultantAvailableTimeSlot ?? "null");
                consultationInvitation.ConsultantReplys
                    = JsonConvert.DeserializeObject<List<QuestionDTO>>(
                        consultationInvitation.Questionnaire ?? "null");

                var answers = JsonConvert.DeserializeObject<JObject>(consultationInvitation.QuestionnaireAnswer ?? "null");

                if (answers != null)
                {
                    foreach (var reply in consultationInvitation.ConsultantReplys)
                    {
                        var answer = answers.GetValue(reply.Name);

                        try
                        {
                            reply.Answers = answer.ToObject<string>();
                        }
                        catch (ArgumentException)
                        {
                            reply.Answers = answer.ToObject<List<string>>();
                        }
                    }
                }
            }

            return response;
        }

        public virtual IPagedList<ModeratorCancellationRequestDTO> GetAllConsultationInvitationsCancelled(
            DateTime? searchDate, int searchType, string searchBuyer, int searchCancelledBy,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null)
        {
            var query = _moderatorCancellationRequestService.GetAllCancelledEngagements();
            if (searchDate != null)
            {
                query = query.Where(x => x.SubmissionDate >= searchDate);
            }
            if (searchType != 0)
            {
                query = query.Where(x => x.EngagementType == Enum.GetName(typeof(CancellationRequestType), searchType));
            }
            if (!String.IsNullOrWhiteSpace(searchBuyer))
            {
                query = query.Where(x => x.BuyerName.ToLower().Contains(searchBuyer.ToLower()));

            }
            if (searchCancelledBy != 0)
            {
                query = query.Where(x => x.CancelledBy == Enum.GetName(typeof(CancellationRequestBy), searchCancelledBy));
            }
            var data = new PagedList<ModeratorCancellationRequestDTO>(query, pageIndex, pageSize);

            foreach (var d in data)
            {
                d.BlockStatusSeller = _blockCustomerService.GetBlockStatus(d.SellerId);
                d.Status = "Active";

                if (d.BlockStatusSeller.IsCurrentlyBlock)
                {
                    d.Status = "Block";
                }
            }

            return data;
        }

        public virtual void UpdateConsultationInvitationStatus(int id, int actorId, ConsultationInvitationStatus status)
        {
            var model = _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            model.ConsultationApplicationStatus = (int)status;
            model.UpdateAudit(actorId);

            _consultationInvitationRepository.Update(model);
        }

        public virtual void UpdateConsultationInvitationApproval(int actorId, int id, ConsultationProfileDTO dto)
        {
            var model = 
                (from ci in _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                from sp in _ServiceProfileRepository.Table
                where sp.Deleted == false
                && sp.Id == ci.ServiceProfileId
                from cp in _ConsultationProfileRepository.Table
                where cp.Deleted == false
                && cp.Id == ci.ConsultationProfileId
                from og in _OrganizationProfileRepository.Table
                where og.Deleted == false
                && og.Id == cp.OrganizationProfileId
                select new
                {
                    ci,
                    sp.CustomerId,
                    OrgCustomerId = og.CustomerId
                })
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            model.ci.ApprovalRemarks = dto.Remarks;
            model.ci.IsApproved = dto.IsApproved;
            model.ci.IsOrganizationRead = false;

            model.ci.UpdateAudit(actorId);
            model.ci.UpdatedOnUTC = DateTime.UtcNow;

            _consultationInvitationRepository.Update(model.ci);

            _individualAttentionService.ClearIndividualAttentionCache(model.CustomerId);
            _organizationAttentionService.ClearOrganizationAttentionCache(model.OrgCustomerId);
        }
        
        public virtual void UpdateConsultationInvitationComplete(
            int actorId, 
            int consultationInvitationId, 
            Dictionary<string, int> dict, string remarks, Dictionary<string, int> orgRatings, string orgRemarks)
        {    
            var model = _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == consultationInvitationId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }
            if (orgRatings.Count > 0)
            {
                if (orgRemarks != "")
                {
                    model.ReviewText = orgRemarks;
                }
                var val = 0;
                if (dict.TryGetValue("knowledgenessRating", out val))
                {
                    model.KnowledgenessRating = dict["knowledgenessRating"];
                }
                if (dict.TryGetValue("clearnessRating", out val))
                {
                    model.ClearnessRating = dict["clearnessRating"];
                }
                if (dict.TryGetValue("professionalismRating", out val))
                {
                    model.ProfessionalismRating = dict["professionalismRating"];
                }
                if (dict.TryGetValue("relevanceRating", out val))
                {
                    model.RelevanceRating = dict["relevanceRating"];
                }
                if (dict.TryGetValue("respondingRating", out val))
                {
                    model.RespondingRating = dict["respondingRating"];
                }
            }
            if (remarks != "")
            {
                model.ModeratorReviewText = remarks;
            }
            var value = 0;
            if (dict.TryGetValue("knowledgenessRating", out value))
            {
                model.ModeratorKnowledgenessRating = dict["knowledgenessRating"];
            }
            if (dict.TryGetValue("clearnessRating", out value))
            {
                model.ModeratorClearnessRating = dict["clearnessRating"];
            }
            if (dict.TryGetValue("professionalismRating", out value))
            {
                model.ModeratorProfessionalismRating = dict["professionalismRating"];
            }
            if (dict.TryGetValue("relevanceRating", out value))
            {
                model.ModeratorRelevanceRating = dict["relevanceRating"];
            }
            if (dict.TryGetValue("respondingRating", out value))
            {
                model.ModeratorRespondingRating = dict["respondingRating"];
            }
            model.ConsultationApplicationStatus = (int)ConsultationInvitationStatus.Completed;
            model.UpdateAudit(actorId);
            model.UpdatedOnUTC = DateTime.UtcNow;
            _consultationInvitationRepository.Update(model);
        }

        public virtual void UpdateConsultationInvitationReschedule(int actorId, string rescheduleRemarks, DateTime? start, DateTime? end)
        {
            var model = _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == actorId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            if (rescheduleRemarks != "")
            {
                var timezone = _dateTimeHelper.DefaultStoreTimeZone;
                var hoursDiff = timezone.BaseUtcOffset.TotalHours;
                var localDateTime = DateTime.UtcNow.AddHours(hoursDiff);
                var newRescheduleRemarks = $"<p>[{localDateTime.ToString("dd MMM yyyy HH:mm")}] - {rescheduleRemarks}</p>";

                model.RescheduleRemarks = newRescheduleRemarks + model.RescheduleRemarks;
            }
            if (start != null)
            {
                model.AppointmentStartDate = start;

                model.IsIndividualRead = false;
                model.IsOrganizationRead = false;

                _individualAttentionService.ClearIndividualAttentionCache(model.IndividualCustomerId);

            }
            if (end != null)
            {
                model.AppointmentEndDate = end;
            }
            model.UpdateAudit(actorId);
            _consultationInvitationRepository.Update(model);
        }

        public List<int> GetAllIndividualIdFromConsultationProfile(int consultationProfileId)
        {
            var model = _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false && x.ConsultationProfileId == consultationProfileId)
                .Select(x => x.IndividualCustomerId)
                .ToList();

            return model;
        }

        public void UpdateIndividualConsultationInvitationRead(int consultationInvitationId, int indvActorId)
        {
            var consultationInvitation =
                    (from ci in _consultationInvitationRepository.Table
                    .Where(ci => ci.Deleted == false
                    && (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                    || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                    || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization
                    || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed)
                    && ci.Id == consultationInvitationId)
                     from sp in _ServiceProfileRepository.Table
                     .Where(sp => sp.Deleted == false
                     && sp.Id == ci.ServiceProfileId
                     && sp.CustomerId == indvActorId)
                     select ci)
                    .FirstOrDefault();

            if (consultationInvitation != null)
            {
                //consultationInvitation.UpdateAudit(indvActorId);
                consultationInvitation.IsIndividualRead = true;

                _individualAttentionService.ClearIndividualAttentionCache(indvActorId);

                _consultationInvitationRepository.Update(consultationInvitation);
            }
        }

        public void UpdateOrganizationConsultationInvitationRead(int consultationInvitationId, int orgActorId)
        {
            var consultationInvitation =
                    (from ci in _consultationInvitationRepository.Table
                    .Where(ci => ci.Deleted == false
                    && (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                    || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                    || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization
                    || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed)
                    && ci.Id == consultationInvitationId)
                     from op in _OrganizationProfileRepository.Table
                     .Where(op => op.Deleted == false
                     && op.Id == ci.OrganizationProfileId
                     && op.CustomerId == orgActorId)
                     select ci)
                    .FirstOrDefault();

            if (consultationInvitation != null)
            {
                consultationInvitation.UpdateAudit(orgActorId);
                consultationInvitation.IsOrganizationRead = true;

                _consultationInvitationRepository.Update(consultationInvitation);

                _organizationAttentionService.ClearOrganizationAttentionCache(orgActorId);
            }
        }

        public ConsultationJobOrgCounterDTO GetConsultationJobOrgCounter(int customerId)
        {
            var recordQuery = (from ji in _consultationInvitationRepository.Table
                           .Where(ji => ji.Deleted == false)
                               from og in _OrganizationProfileRepository.Table
                               .Where(og => og.Deleted == false
                               && og.Id == ji.OrganizationProfileId
                               && og.CustomerId == customerId)
                               select ji)
                          .GroupBy(x => 1)
                          .Select(x => new
                          {
                              NoInvitation = x.Count(y =>
                                y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New
                                || (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted
                                && y.IsApproved != true)
                                || (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual
                                    && y.IsOrganizationRead == false)),
                              NoApplicant = x.Count(y =>
                                y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted
                                && y.IsApproved == true),
                              NoConfirmedOrder = x.Count(y =>
                                y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                                || (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                                    && y.IsOrganizationRead == false)
                                || (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization
                                    && y.IsOrganizationRead == false)
                                || (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                                    && y.IsOrganizationRead == false))
                          });
                          
            var record = recordQuery.FirstOrDefault();

            var dto = new ConsultationJobOrgCounterDTO();
            dto.NoInvitation = (record?.NoInvitation )?? 0;
            dto.NoApplicant = (record?.NoApplicant) ?? 0;
            dto.NoConfirmedOrder = (record?.NoConfirmedOrder) ?? 0;

            return dto;
        }

        public EngagementPartyInfo GetEngagingParties(int consultationInvitationId)
        {
            return QueryableEngagementParties().Where(s => s.EngagementId == consultationInvitationId).FirstOrDefault();
        }

        public bool Cancel(int consultationInvitationId, EngagementParty cancellingParty, int actorId)
        {
            var consultation = _consultationInvitationRepository.Table
                                .Where(q => q.Id == consultationInvitationId)
                                .FirstOrDefault();

            // Validate cancellation
            if(consultation.ConsultationApplicationStatus != (int)ConsultationInvitationStatus.Paid)
                return false;
            //

            if(cancellingParty == EngagementParty.Buyer)
                consultation.ConsultationApplicationStatus = (int)ConsultationInvitationStatus.CancelledByOrganization;
            else
                consultation.ConsultationApplicationStatus = (int)ConsultationInvitationStatus.CancelledByIndividual;

            consultation.UpdateAudit(actorId);

            _consultationInvitationRepository.Update(consultation);

            return true;
        }

        public void UpdateCancel(
            int consultationInvitationId,
            DateTime submissionTime, 
            string userRemarks, 
            int reasonId, 
            int? attachmentId,
            EngagementParty cancellationParty)
        {
            var consultation = _consultationInvitationRepository.Table
                .First(q => q.Id == consultationInvitationId);

            consultation.CancellationDateTime = submissionTime;
            consultation.CancellationReasonId = reasonId;
            consultation.CancellationRemarks = userRemarks;
            consultation.CancellationDownloadId = attachmentId;

            if (cancellationParty != EngagementParty.Buyer)
            {
                consultation.IsOrganizationRead = false;
            }
            if (cancellationParty != EngagementParty.Seller)
            {
                consultation.IsIndividualRead = false;
            }

            _consultationInvitationRepository.Update(consultation);
        }

        public IQueryable<EngagementPartyInfo> QueryableEngagementParties()
        {
            return from ci in _consultationInvitationRepository.Table
                   join oc in _OrganizationProfileRepository.Table on ci.OrganizationProfileId equals oc.Id
                   join s in _ServiceProfileRepository.Table on ci.ServiceProfileId equals s.Id
                   join c in _individualeRepository.Table on s.CustomerId equals c.CustomerId
                   select new EngagementPartyInfo
                   {
                       EngagementId = ci.Id,
                       EngagementType = EngagementType,
                       IsEscrow = true,
                       BuyerId = oc.CustomerId,
                       BuyerName = oc.Name,
                       SellerId = c.CustomerId,
                       SellerName = c.FullName,
                       ModeratorId = ci.ModeratorCustomerId.Value,
                       ModeratorName = null // TODO : Get Moderator name from Generic Attributes
                   };
        }

        public DateTime? GetStartDate(int consultationInvitaionId)
        {
            var timeSlots = _consultationInvitationRepository.Table
            .Where(q => q.Id == consultationInvitaionId)
            .Select(q => q.ConsultantAvailableTimeSlot)
            .FirstOrDefault();

            if(string.IsNullOrWhiteSpace(timeSlots))
                return null;

            return JsonConvert.DeserializeObject<List<TimeSlotDTO>>(timeSlots ?? "null").FirstOrDefault()?.StartDate;
        }

        public List<ConsultationInvitationDTO> AutoRejectConsultationInvitation(int actorId)
        {
            int maxWaitingWorkingDay = _consultationJobSettings.InvitationAutoRejectAfterWorkingDays;

            var timezone = _dateTimeHelper.DefaultStoreTimeZone;
            var hoursDiff = timezone.BaseUtcOffset.TotalHours;
            var localDateTime = DateTime.UtcNow.AddHours(hoursDiff);
            var scheduledExecuteTime = localDateTime.Date;


            var consultationInvitations = _consultationInvitationRepository.Table
                .Where(x => x.Deleted == false
                && x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New
                && x.CreatedOnUTC.AddDays(hoursDiff).AddDays(maxWaitingWorkingDay).Date < scheduledExecuteTime)
                .ToList();

            //further filter with working days
            var rejectingConsultationJob = consultationInvitations
                .Where(x => x.CreatedOnUTC.AddDays(hoursDiff).AddWorkdays(maxWaitingWorkingDay).Date < scheduledExecuteTime)
                .ToList();

            rejectingConsultationJob.ForEach(x =>
            {
                x.UpdateAudit(actorId);
                x.ConsultationApplicationStatus = (int)ConsultationInvitationStatus.DeclinedByIndividual;
            });

            _consultationInvitationRepository.Update(rejectingConsultationJob);

            var rejectedConsultationJobDTOs = _mapper.Map<List<ConsultationInvitationDTO>>(rejectingConsultationJob);

            return rejectedConsultationJobDTOs;
        }

        #endregion
    }
}
