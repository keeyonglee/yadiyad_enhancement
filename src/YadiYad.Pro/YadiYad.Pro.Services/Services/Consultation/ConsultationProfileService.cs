using AutoMapper;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Caching;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Infrastructure.Cache;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Services.Attentions;
using YadiYad.Pro.Services.Services.Messages;
using TimeZone = YadiYad.Pro.Core.Domain.Common.TimeZone;

namespace YadiYad.Pro.Services.Consultation
{
    public class ConsultationProfileService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<ConsultationProfile> _ConsultationProfileRepository;
        private readonly IRepository<ConsultationInvitation> _ConsultationInvitationRepository;
        private readonly IRepository<BusinessSegment> _BusinessSegmentRepository;
        private readonly IRepository<TimeZone> _TimeZoneRepository;
        private readonly IRepository<ConsultationExpertise> _ConsultationExpertiseRepository;
        private readonly IRepository<Expertise> _ExpertiseRepository;
        private readonly IRepository<JobServiceCategory> _jobServiceCategoryRepository;
        private readonly IRepository<ServiceProfile> _serviceProfileRepository;
        private readonly IndividualAttentionService _individualAttentionService;


        #endregion

        #region Ctor

        public ConsultationProfileService
            (IMapper mapper,
            IRepository<ServiceProfile> serviceProfileRepository,
            IRepository<ConsultationProfile> ConsultationProfileRepository,
            IRepository<ConsultationInvitation> ConsultationInvitationRepository,
            IRepository<BusinessSegment> BusinessSegmentRepository,
            IRepository<TimeZone> TimeZoneRepository,
            IRepository<ConsultationExpertise> ConsultationExpertiseRepository,
            IRepository<Expertise> ExpertiseRepository,
            IRepository<JobServiceCategory> jobServiceCategoryRepository,
            IndividualAttentionService individualAttentionService)
        {
            _mapper = mapper;
            _serviceProfileRepository = serviceProfileRepository;
            _ConsultationProfileRepository = ConsultationProfileRepository;
            _ConsultationInvitationRepository = ConsultationInvitationRepository;
            _BusinessSegmentRepository = BusinessSegmentRepository;
            _TimeZoneRepository = TimeZoneRepository;
            _ConsultationExpertiseRepository = ConsultationExpertiseRepository;
            _ExpertiseRepository = ExpertiseRepository;
            _jobServiceCategoryRepository = jobServiceCategoryRepository;
            _individualAttentionService = individualAttentionService;
        }

        #endregion


        #region Methods

        public virtual void CreateConsultationProfile(int actorId, ConsultationProfileDTO dto)
        {
            var model = _mapper.Map<ConsultationProfile>(dto);
            //var model = dto.ToModel(_mapper);

            model.AvailableTimeSlot = JsonConvert.SerializeObject(dto.TimeSlots);
            model.Questionnaire = JsonConvert.SerializeObject(dto.Questions);

            model.CreateAudit(actorId);

            _ConsultationProfileRepository.Insert(model);

            foreach (var expertise in model.ConsultationExpertises)
            {
                expertise.ConsultationProfileId = model.Id;
                expertise.ExpertiseId = expertise.Id;
                
                expertise.CreateAudit(actorId);
            }

            _ConsultationExpertiseRepository.Insert(model.ConsultationExpertises);
            
        }

        public virtual void UpdateConsultationProfile(int actorId, ConsultationProfileDTO dto)
        {
            var model = _ConsultationProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == dto.Id)
                .FirstOrDefault();

            if(model == null)
            {
                throw new KeyNotFoundException();
            }
            model.UpdateAudit(actorId);

            model.SegmentId = dto.SegmentId;
            model.Topic = dto.Topic;
            model.Objective = dto.Objective;
            model.TimeZoneId = dto.TimeZoneId;
            model.AvailableTimeSlot = JsonConvert.SerializeObject(dto.TimeSlots);
            model.Questionnaire = JsonConvert.SerializeObject(dto.Questions);
            model.IsApproved = null;
            _ConsultationProfileRepository.Update(model);
        }

        public virtual void UpdateConsultationProfileApproval(int actorId, ConsultationProfileDTO dto)
        {
            var model = _ConsultationProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == actorId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            model.Remarks = dto.Remarks;
            model.IsApproved = dto.IsApproved;
            model.UpdateAudit(actorId);
            model.UpdatedOnUTC = DateTime.UtcNow;
            _ConsultationProfileRepository.Update(model);
        }


        public virtual void ReviewConsultationResponses(int actorId, int consultationProfileId, List<QuestionDTO> responses)
        {
            var profile = _ConsultationProfileRepository.Table.FirstOrDefault(x => x.Id == consultationProfileId && !x.Deleted);
            if (profile == null) throw new KeyNotFoundException();
            profile.Responses = JsonConvert.SerializeObject(responses);
            profile.UpdateAudit(actorId);
            _ConsultationProfileRepository.Update(profile);
        }

        public virtual void HireConsultant(int actorId, int consultationProfileId, int consultantId, decimal fee)
        {
            var profile = _ConsultationProfileRepository.Table.FirstOrDefault(x => x.Id == consultationProfileId && !x.Deleted);
            if (profile == null) throw new KeyNotFoundException();
            profile.OrganizationProfileId = consultantId;
            profile.IsApproved = true;
            profile.UpdateAudit(actorId);
            _ConsultationProfileRepository.Update(profile);
        }

        public virtual void RehireConsultant(int actorId, int consultationProfileId, int newConsultantId)
        {
            var profile = _ConsultationProfileRepository.Table.FirstOrDefault(x => x.Id == consultationProfileId && !x.Deleted);
            if (profile == null) throw new KeyNotFoundException();

            profile.OrganizationProfileId = newConsultantId;
            profile.IsApproved = true;
            profile.UpdateAudit(actorId);
            _ConsultationProfileRepository.Update(profile);
        }

        public virtual void SubmitConsultationReview(int actorId, int consultationProfileId, ConsultationInvitationReviewDTO review)
        {
            var profile = _ConsultationProfileRepository.Table.FirstOrDefault(x => x.Id == consultationProfileId && !x.Deleted);
            if (profile == null) throw new KeyNotFoundException();
            profile.Review = JsonConvert.SerializeObject(review);
            profile.Status = (int)ConsultationHiringStatus.Completed;
            profile.UpdateAudit(actorId);
            _ConsultationProfileRepository.Update(profile);
        }


        public ConsultationProfileDTO GetConsultationProfileById(int id)
        {
            if (id == 0)
                return null;

            var query = _ConsultationProfileRepository.Table;

            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var expertises = _ConsultationExpertiseRepository.Table
                .Where(x => x.Deleted == false
                && x.ConsultationProfileId == id)
                .Join(_ExpertiseRepository.Table,
                x => x.ExpertiseId,
                y => y.Id,
                (x, y) => new
                {
                    x.ConsultationProfileId
                    ,
                    Id = x.ExpertiseId
                    ,
                    x.OtherExpertise
                    ,
                    y.JobServiceCategoryId
                    ,
                    y.Name
                })
                .Select(x => new ConsultationExpertiseDTO
                {
                    Id = x.Id,
                    OtherExpertise = x.OtherExpertise,
                    CategoryId = x.JobServiceCategoryId,
                    Name = x.Name
                })
                .ToList();

            var segmentRecord = _BusinessSegmentRepository.Table.Where(x => x.Id == record.SegmentId).FirstOrDefault();
            var timeZoneRecord = _TimeZoneRepository.Table.Where(x => x.Id == record.TimeZoneId).FirstOrDefault();

            var response = _mapper.Map<ConsultationProfileDTO>(record);

            response.SegmentName = segmentRecord.Name;
            response.TimeZoneName = timeZoneRecord.Name;
            response.CategoryId = expertises.Select(x => x.CategoryId).Distinct().FirstOrDefault();
            var category = _jobServiceCategoryRepository.Table
                .Where(x => x.Id == response.CategoryId)
                .FirstOrDefault();
            response.CategoryName = category?.Name;
            response.ExperienceYearName = ((ExperienceYear)response.YearExperience).GetDescription();
            response.ConsultationExpertises = expertises;

            response.Questions = JsonConvert.DeserializeObject<List<QuestionDTO>>(record.Questionnaire);
            response.TimeSlots = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(record.AvailableTimeSlot);

            return response;
        }

        public IPagedList<ConsultationProfileDTO> GetConsultationProfilesByOrganizationProfileId(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            ConsultationListSearchFilterDTO filterDTO = null)
        {
            var record = from cp in _ConsultationProfileRepository.Table
                         where cp.OrganizationProfileId == filterDTO.OrganizationProfileId
                         && cp.Deleted == false
                         && !cp.DeletedFromUser
                         from sg in _BusinessSegmentRepository.Table
                         where sg.Id == cp.SegmentId
                         from tz in _TimeZoneRepository.Table
                         where tz.Id == cp.TimeZoneId
                         select new ConsultationProfileDTO
                         {
                             Id = cp.Id,
                             Topic = cp.Topic,
                             SegmentId = cp.SegmentId,
                             SegmentName = sg.Name,
                             Objective = cp.Objective,
                             TimeZoneId = cp.TimeZoneId,
                             TimeZoneName = tz.Name,
                             Questionnaire = cp.Questionnaire,
                             AvailableTimeSlot = cp.AvailableTimeSlot,
                             Duration = cp.Duration,
                             UpdatedOnUTC = cp.UpdatedOnUTC,
                             IsApproved = cp.IsApproved,
                             Remarks = cp.Remarks,
                             YearExperience = cp.YearExperience,
                             CreatedOnUTC = cp.CreatedOnUTC
                         };

            record = record.OrderByDescending(x => x.CreatedOnUTC);
            var consultationProfileIds = record.Select(x => x.Id).ToList();
            var response = new PagedList<ConsultationProfileDTO>(record, pageIndex, pageSize);

            var expertises = _ConsultationExpertiseRepository.Table
                .Where(x => !x.Deleted && consultationProfileIds.Contains(x.ConsultationProfileId))
                .Join(_ExpertiseRepository.Table,
                x => x.ExpertiseId,
                y => y.Id,
                (x, y) => new
                {
                    x.ConsultationProfileId,
                    Id = x.ExpertiseId,
                    x.OtherExpertise,
                    y.JobServiceCategoryId,
                    y.Name
                })
                .ToList();
            var expertiseCategoryIds = expertises.Select(x => x.JobServiceCategoryId).Distinct().ToList();

            var category = _jobServiceCategoryRepository.Table
                            .Where(x => expertiseCategoryIds.Contains(x.Id))
                            .ToList();


            foreach (var responseDTO in response)
            {
                responseDTO.Questions = JsonConvert.DeserializeObject<List<QuestionDTO>>(responseDTO.Questionnaire);
                responseDTO.TimeSlots = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(responseDTO.AvailableTimeSlot);
                responseDTO.ConsultationExpertises = expertises.Where(y => y.ConsultationProfileId == responseDTO.Id).Select(y => new ConsultationExpertiseDTO
                {
                    Id = y.Id,
                    OtherExpertise = y.OtherExpertise,
                    CategoryId = y.JobServiceCategoryId,
                    Name = y.Name
                }).ToList();

                responseDTO.CategoryId = category.Where(y => responseDTO.ConsultationExpertises.Select(z => z.CategoryId).FirstOrDefault() == y.Id).Select(y => y.Id).FirstOrDefault();
                responseDTO.CategoryName = category.Where(y => responseDTO.ConsultationExpertises.Select(z => z.CategoryId).FirstOrDefault() == y.Id).Select(y => y.Name).FirstOrDefault();

            }

            return response;
        }

        public IQueryable<ConsultationProfileDTO> GetConsultationProfilesDto()
        {
            var query = from cp in _ConsultationProfileRepository.Table
                        where cp.Deleted == false
                        from sg in _BusinessSegmentRepository.Table
                        where sg.Id == cp.SegmentId
                        from tz in _TimeZoneRepository.Table
                        where tz.Id == cp.TimeZoneId
                        select new ConsultationProfileDTO
                        {
                            Id = cp.Id,
                            Topic = cp.Topic,
                            SegmentId = cp.SegmentId,
                            SegmentName = sg.Name,
                            Objective = cp.Objective,
                            TimeZoneId = cp.TimeZoneId,
                            TimeZoneName = tz.Name,
                            Questionnaire = cp.Questionnaire,
                            AvailableTimeSlot = cp.AvailableTimeSlot,
                            IsApproved = cp.IsApproved,
                            Duration = cp.Duration
                        };
            return query;
        }

        public IPagedList<ConsultationProfileDTO> GetConsultationProfilesChecking(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            ConsultationAdvsReviewSearchFilterDTO filterDTO = null)
        {
            var record = GetConsultationProfilesDto();
            record = record.Where(x => x.IsApproved == null).OrderByDescending(x => x.Id);

            var response = new PagedList<ConsultationProfileDTO>(record, pageIndex, pageSize);
            foreach (var responseDTO in response)
            {
                responseDTO.Questions = JsonConvert.DeserializeObject<List<QuestionDTO>>(responseDTO.Questionnaire);
                responseDTO.TimeSlots = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(responseDTO.AvailableTimeSlot);
            }

            return response;
        }

        public virtual List<int> DeleteConsultationProfile(int actorId, int id)
        {
            var model = _ConsultationProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }
            model.UpdateAudit(actorId);
            model.DeletedFromUser = true;

            // get new & accepted invitation & update status
            var ciToUpdate = _ConsultationInvitationRepository.Table
                .Where(x => !x.Deleted && x.ConsultationProfileId == id && (x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New || x.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted))
                .ToList()
                .Select(x =>
                {
                    x.ConsultationApplicationStatus = (int)ConsultationInvitationStatus.DeclinedByOrganization;
                    x.IsIndividualRead = false;

                    return x;
                })
                .ToList();

            var serviceProfileIds = ciToUpdate.Select(y => y.ServiceProfileId).ToList();

            var consultantSustIds = _serviceProfileRepository.Table
                .Where(x => x.Deleted == false
                && serviceProfileIds.Contains(x.Id))
                .Select(x => x.CustomerId)
                .ToList();

            consultantSustIds.ForEach(custId =>
            {
                _individualAttentionService.ClearIndividualAttentionCache(custId);
            });

            _ConsultationProfileRepository.Update(model);
            _ConsultationInvitationRepository.Update(ciToUpdate);

            return ciToUpdate.Select(x => x.Id).ToList();
        }

        #endregion
    }
}
