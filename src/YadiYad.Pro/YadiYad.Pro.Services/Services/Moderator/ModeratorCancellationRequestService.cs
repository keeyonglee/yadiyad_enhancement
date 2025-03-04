using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Moderator;

namespace YadiYad.Pro.Services.Services.Moderator
{
    public class ModeratorCancellationRequestService
    {
        #region Fields

        private readonly IRepository<ConsultationInvitation> _ConsultationInvitationRepository;
        private readonly IRepository<ServiceApplication> _ServiceApplicationRepository;
        private readonly IRepository<JobApplication> _JobApplicationRepository;
        private readonly IRepository<OrganizationProfile> _OrganizationProfileRepository;
        private readonly IRepository<IndividualProfile> _IndividualProfileRepository;
        private readonly IRepository<ServiceProfile> _ServiceProfileRepository;
        private readonly IRepository<Reason> _ReasonRepository;
        private readonly IRepository<JobProfile> _JobProfileRepository;
        private readonly IRepository<JobSeekerProfile> _JobSeekerProfilerepository;
        #endregion

        #region Ctor

        public ModeratorCancellationRequestService(
            IRepository<ConsultationInvitation> ConsultationInvitationRepository,
            IRepository<ServiceApplication> ServiceApplicationRepository,
            IRepository<JobApplication> JobApplicationRepository,
            IRepository<OrganizationProfile> OrganizationProfileRepository,
            IRepository<IndividualProfile> IndividualProfileRepository,
            IRepository<ServiceProfile> ServiceProfileRepository,
            IRepository<Reason> ReasonRepository,
            IRepository<JobProfile> JobProfileRepository,
            IRepository<JobSeekerProfile> JobSeekerProfilerepository)

        {
            _ConsultationInvitationRepository = ConsultationInvitationRepository;
            _ServiceApplicationRepository = ServiceApplicationRepository;
            _JobApplicationRepository = JobApplicationRepository;
            _OrganizationProfileRepository = OrganizationProfileRepository;
            _IndividualProfileRepository = IndividualProfileRepository;
            _ServiceProfileRepository = ServiceProfileRepository;
            _ReasonRepository = ReasonRepository;
            _JobProfileRepository = JobProfileRepository;
            _JobSeekerProfilerepository = JobSeekerProfilerepository;
        }

        #endregion

        public virtual IPagedList<Reason> GetAllCancellationReason(int engagementType, int party,
        int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _ReasonRepository.Table.Where(x => x.Published == true && x.EngagementType == engagementType && x.Party == party);
            var checking = (query.Select(x => x.Name).Contains("Others"));
            if (checking)
            {
                query = query.OrderBy(x => x.Name == "Others").ThenBy(x => x.Name);
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }
            var reason = new PagedList<Reason>(query, pageIndex, pageSize);

            return reason;
        }

        public virtual List<SelectListItem> GetBlockReasonSeller()
        {
            var query = (from r in _ReasonRepository.Table
                         where (r.Published == true && r.EngagementType == 4 && r.Party == 2)

                         select new SelectListItem
                         {
                             Text = r.Name,
                             Value = r.Id.ToString()
                         }).OrderBy(x => x.Text).ToList();
            return query;
        }

        public IQueryable<ModeratorCancellationRequestDTO> GetAllCancelledEngagements()
        {
            var query1 = (from ci in _ConsultationInvitationRepository.Table
                          where (ci.Deleted == false && (ci.ConsultationApplicationStatus == 6 || ci.ConsultationApplicationStatus == 7))

                          from op in _OrganizationProfileRepository.Table.DefaultIfEmpty()
                          .Where(op => op.Id == ci.OrganizationProfileId
                          && op.Deleted == false)

                          from sp in _ServiceProfileRepository.Table.DefaultIfEmpty()
                          .Where(sp => sp.Id == ci.ServiceProfileId
                          && sp.Deleted == false)

                          from ip in _IndividualProfileRepository.Table.DefaultIfEmpty()
                         .Where(ip => ip.CustomerId == sp.CustomerId
                          && ip.Deleted == false)

                          from r in _ReasonRepository.Table.DefaultIfEmpty()
                          .Where(r => r.Id == ci.CancellationReasonId
                          && r.Published == true)

                          select new ModeratorCancellationRequestDTO
                          {
                              SubmissionDate = ci.CreatedOnUTC,
                              EngagementType = EngagementType.Consultation.GetDescription(),
                              EngagementId = ci.Id,
                              BuyerName = op.Name,
                              SellerName = ip.FullName,
                              Reason = r.Name,
                              Remarks = ci.CancellationRemarks,
                              CancelledBy = ci.ConsultationApplicationStatus == 6 ? EngagementParty.Buyer.GetDescription() : EngagementParty.Seller.GetDescription(),
                              AttachmentId = ci.CancellationDownloadId.Value,
                              SellerId = ip.CustomerId
                          });

                        var query2 = 

                         (from sa in _ServiceApplicationRepository.Table
                         where (sa.Deleted == false && (sa.Status == 7 || sa.Status == 8))

                         from sp in _ServiceProfileRepository.Table.DefaultIfEmpty()
                         .Where(sp => sp.Id == sa.ServiceProfileId
                         && sp.Deleted == false)

                         from ipBuyer in _IndividualProfileRepository.Table.DefaultIfEmpty()
                        .Where(ip => ip.CustomerId == sa.CustomerId
                         && ip.Deleted == false)

                         from ipSeller in _IndividualProfileRepository.Table.DefaultIfEmpty()
                         .Where(ip => ip.CustomerId == sp.CustomerId
                          && ip.Deleted == false)

                         from r in _ReasonRepository.Table.DefaultIfEmpty()
                         .Where(r => r.Id == sa.CancellationReasonId
                         && r.Published == true)

                         select new ModeratorCancellationRequestDTO
                         {
                             SubmissionDate = sa.CreatedOnUTC,
                             EngagementType = EngagementType.Service.GetDescription(),
                             EngagementId = sa.Id,
                             BuyerName = ipBuyer.FullName,
                             SellerName = ipSeller.FullName,
                             Reason = r.Name,
                             Remarks = sa.CancellationRemarks,
                             CancelledBy = sa.Status == 7 ? EngagementParty.Buyer.GetDescription() : EngagementParty.Seller.GetDescription(),
                             AttachmentId = sa.CancellationDownloadId.Value,
                             SellerId = ipSeller.CustomerId
                         });

                    var query3 = (from ja in _JobApplicationRepository.Table
                     where (ja.Deleted == false && (ja.JobApplicationStatus == 12 || ja.JobApplicationStatus == 13))

                     from op in _OrganizationProfileRepository.Table.DefaultIfEmpty()
                     .Where(op => op.Id == ja.OrganizationProfileId
                     && op.Deleted == false)

                     from jsp in _JobSeekerProfilerepository.Table.DefaultIfEmpty()
                     .Where(jsp => jsp.Id == ja.JobSeekerProfileId
                     && jsp.Deleted == false)

                     from ip in _IndividualProfileRepository.Table.DefaultIfEmpty()
                     .Where(ip => ip.CustomerId == jsp.CustomerId
                     && ip.Deleted == false)

                     from r in _ReasonRepository.Table.DefaultIfEmpty()
                     .Where(r => r.Id == ja.CancellationReasonId
                     && r.Published == true)

                     select new ModeratorCancellationRequestDTO
                     {
                         SubmissionDate = ja.CreatedOnUTC,
                         EngagementType = EngagementType.Job.GetDescription(),
                         EngagementId = ja.Id,
                         BuyerName = op.Name,
                         SellerName = ip.FullName,
                         Reason = r.Name,
                         Remarks = ja.CancellationRemarks,
                         CancelledBy = ja.JobApplicationStatus == 12 ? EngagementParty.Buyer.GetDescription() : EngagementParty.Seller.GetDescription(),
                         AttachmentId = ja.CancellationDownloadId.Value,
                         SellerId = jsp.CustomerId
                     });

            query1 = query1.Union(query2);
            query1 = query1.Union(query3).OrderByDescending(x => x.SubmissionDate);
            return query1;
        }

        public IQueryable<ModeratorCancellationRequestDTO> GetConsultationEngagementEditById(int id)
        {
            var query = (from ci in _ConsultationInvitationRepository.Table
                          where (ci.Deleted == false && ci.Id == id)

                          from op in _OrganizationProfileRepository.Table.DefaultIfEmpty()
                          .Where(op => op.Id == ci.OrganizationProfileId
                          && op.Deleted == false)

                         from sp in _ServiceProfileRepository.Table.DefaultIfEmpty()
                          .Where(sp => sp.Id == ci.ServiceProfileId
                          && sp.Deleted == false)

                          from ip in _IndividualProfileRepository.Table.DefaultIfEmpty()
                         .Where(ip => ip.CustomerId == sp.CustomerId
                          && ip.Deleted == false)

                         from r in _ReasonRepository.Table.DefaultIfEmpty()
                          .Where(r => r.Id == ci.CancellationReasonId
                          && r.Published == true)

                         select new ModeratorCancellationRequestDTO
                          {
                              EngagementId = ci.Id,
                              SellerName = ip.FullName,
                              AttachmentId = ci.CancellationDownloadId == null ? 0 : ci.CancellationDownloadId.Value,
                              ConsultantAvailableTimeSlots = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(ci.ConsultantAvailableTimeSlot ?? "null"),
                              CancelledBy = ci.ConsultationApplicationStatus == 6 ? "Buyer" : "Seller",
                              Reason = r.Name,
                              Remarks = ci.CancellationRemarks,
                              BuyerId = op.CustomerId,
                              SellerId = ip.CustomerId,
                              ConsultationProfileId = ci.ConsultationProfileId,
                              BuyerName = op.Name,
                              SellerEmail = ip.Email,
                              BuyerEmail = op.ContactPersonEmail,
                              AppointmentEndDate = ci.AppointmentEndDate,
                              AppointmentStartDate = ci.AppointmentStartDate
                         }) ;

            return query;
        }

        public IQueryable<ModeratorCancellationRequestDTO> GetServiceEngagementEditById(int id)
        {
            var query =   (from sa in _ServiceApplicationRepository.Table
                          where (sa.Deleted == false && sa.Id == id)

                          from sp in _ServiceProfileRepository.Table.DefaultIfEmpty()
                          .Where(sp => sp.Id == sa.ServiceProfileId
                          && sp.Deleted == false)

                          from ipSeller in _IndividualProfileRepository.Table.DefaultIfEmpty()
                          .Where(ip => ip.CustomerId == sp.CustomerId
                           && ip.Deleted == false)

                          select new ModeratorCancellationRequestDTO
                          {
                              EngagementId = sa.Id,
                              SellerName = ipSeller.FullName,
                              AttachmentId = sa.CancellationDownloadId == null ? 0 : sa.CancellationDownloadId.Value,
                              SellerId = ipSeller.CustomerId
                          });

            return query;
        }

        public IQueryable<ModeratorCancellationRequestDTO> GetJobEngagementEditById(int id)
        {
            var query = (from ja in _JobApplicationRepository.Table
                          where (ja.Deleted == false && ja.Id == id)

                          from jsp in _JobSeekerProfilerepository.Table
                          .Where(jsp=>jsp.Deleted == false
                          && jsp.Id == ja.JobSeekerProfileId)
                          .DefaultIfEmpty()

                          from ip in _IndividualProfileRepository.Table.DefaultIfEmpty()
                          .Where(ip => ip.CustomerId == jsp.CustomerId
                          && ip.Deleted == false)

                          select new ModeratorCancellationRequestDTO
                          {
                              EngagementId = ja.Id,
                              SellerName = ip.FullName,
                              AttachmentId = ja.CancellationDownloadId == null ? 0 : ja.CancellationDownloadId.Value,
                              SellerId = ip.CustomerId
                          });

            return query;
        }


        public virtual ConsultationInvitation GetConsultationInvitationById(int id)
        {
            if (id == 0)
                return null;

            return _ConsultationInvitationRepository.GetById(id);
        }

        public virtual ServiceApplication GetServiceApplicationById(int id)
        {
            if (id == 0)
                return null;

            return _ServiceApplicationRepository.GetById(id);
        }

        public virtual JobApplication GetJobApplicationById(int id)
        {
            if (id == 0)
                return null;

            return _JobApplicationRepository.GetById(id);
        }

        public virtual void UpdateConsultation(ConsultationInvitation item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _ConsultationInvitationRepository.Update(item);
        }

        public virtual void UpdateService(ServiceApplication item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _ServiceApplicationRepository.Update(item);
        }

        public virtual void UpdateJob(JobApplication item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _JobApplicationRepository.Update(item);
        }

        public virtual ModeratorCancellationRequestDTO GetEngagementEdit(int id, EngagementType type)
        {
            var item = new ModeratorCancellationRequestDTO();
            if (type == EngagementType.Consultation)
            {
                var query = GetConsultationEngagementEditById(id);
                item = query.FirstOrDefault();
            }
            else if (type == EngagementType.Service)
            {
                var query = GetServiceEngagementEditById(id);
                item = query.FirstOrDefault();
            }
            else if (type == EngagementType.Job)
            {
                var query = GetJobEngagementEditById(id);
                item = query.FirstOrDefault();
            }

            return item;
        }

        public virtual ModeratorCancellationRequestDTO GetModeratorConsultantDTO(int id)
        {
            var query = GetConsultationEngagementEditById(id);
            return query.FirstOrDefault();
        }

        public void ApproveConsultationRequest(int id)
        {
            var consultation = _ConsultationInvitationRepository.GetById(id);
            if (consultation == null) throw new Exception("Consultation request not found.");

            consultation.IsApproved = true;
            _ConsultationInvitationRepository.Update(consultation);
        }

        // Approve Consultant Questionnaire Response
        public void ApproveConsultantResponse(int responseId)
        {
            var response = _ConsultationInvitationRepository.GetById(responseId);
            if (response == null) throw new Exception("Response not found.");

            response.IsApproved = true;
            _ConsultationInvitationRepository.Update(response);
        }

        // Setup Consultation Appointment
        public void SetupConsultationAppointment(int id, DateTime start, DateTime end, String status_remark)
        {
            var consultation = _ConsultationInvitationRepository.GetById(id);
            if (consultation == null) throw new Exception("Consultation not found.");

            consultation.AppointmentStartDate = start;
            consultation.AppointmentEndDate = end;
            consultation.StatusRemarks = status_remark;

            _ConsultationInvitationRepository.Update(consultation);
        }

        // Reschedule Consultation Appointment
        public void RescheduleConsultation(int id, DateTime newStart, DateTime newEnd)
        {
            var consultation = _ConsultationInvitationRepository.GetById(id);
            if (consultation == null) throw new Exception("Consultation not found.");

            consultation.AppointmentStartDate = newStart;
            consultation.AppointmentEndDate = newEnd;

            _ConsultationInvitationRepository.Update(consultation);
        }

        // Cancel Consultation Appointment
        public void CancelConsultation(int id, int cancelledBy, int reasonId, string status_remark, string remarks)
        {
            var consultation = _ConsultationInvitationRepository.GetById(id);
            if (consultation == null) throw new Exception("Consultation not found.");

            consultation.StatusRemarks = status_remark;
            consultation.CancellationReasonId = reasonId;
            consultation.CancellationRemarks = remarks;

            _ConsultationInvitationRepository.Update(consultation);
        }

        // Allow Rehiring After Cancellation
        public void AllowRehireAfterCancellation(int engagementId, int newConsultantId, string status_remark)
        {
            var consultation = _ConsultationInvitationRepository.GetById(engagementId);
            if (consultation == null) throw new Exception("Consultation not found.");

            consultation.ConsultationProfileId = newConsultantId;
            consultation.StatusRemarks = status_remark;

            _ConsultationInvitationRepository.Update(consultation);
        }
    }
}
