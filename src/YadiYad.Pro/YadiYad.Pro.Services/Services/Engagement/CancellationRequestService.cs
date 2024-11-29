using System;
using System.Collections.Generic;
using System.Linq;

using Nop.Core;
using Nop.Data;

using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.Engagement;
using YadiYad.Pro.Services.Services.Engagement;
using YadiYad.Pro.Services.Services.Moderator;

namespace YadiYad.Pro.Services.Engagement
{
    public class CancellationRequestService
    {
        private readonly EngagementResolver _engagementResolver;
        private readonly IRepository<CancellationRequest> _cancellationRequestRepository;
        private readonly IRepository<Reason> _reasonRepository;
        private readonly BlockCustomerService _blockCustomerService;
        public CancellationRequestService(IRepository<CancellationRequest> cancellationRequestRepository,
            EngagementResolver engagementResolver,
            IRepository<Reason> reasonRepository,
            BlockCustomerService blockCustomerService)
        {
            _cancellationRequestRepository = cancellationRequestRepository;
            _engagementResolver = engagementResolver;
            _reasonRepository = reasonRepository;
            _blockCustomerService = blockCustomerService;
        }

        public Reason GetReasonById(int id)
        {
            return _reasonRepository.Table.Where(q => q.Id == id && q.Published).FirstOrDefault();
        }

        public bool Close(int cancellationRequestId, int actorId, string adminRemarks, int blockUserDays = 0, int? attachmentId = null)
        {
            var engagementRequest = _cancellationRequestRepository.Table
                .FirstOrDefault(q => q.Id == cancellationRequestId && !q.Deleted);

            if(engagementRequest == null)
                return false;

            if(engagementRequest.Status == CancellationStatus.Closed)
                return false;

            engagementRequest.AdminRemarks = adminRemarks;
            engagementRequest.CustomerBlockDays = blockUserDays;
            engagementRequest.AttachmentId = attachmentId;
            engagementRequest.Status = CancellationStatus.Closed;
            engagementRequest.UpdateAudit(actorId);

            _cancellationRequestRepository.Update(engagementRequest);

            return true;
        }

        public bool CreateRequest(int engagementId, EngagementType engagementType, int actorId,
            EngagementParty cancellingParty, int reasonId, string userRemarks = null,
            int blockUserDays = 0, PostCancellationAction postCancellationAction = PostCancellationAction.Rehire,
            int? attachmentId = null)
        {
            var engagementRequestCount = _cancellationRequestRepository.Table
                .Count(q => q.EngagementId == engagementId && q.EngagementType == engagementType && !q.Deleted);

            if (engagementRequestCount != 0)
                return false;

            var engagementRequest = new CancellationRequest();
            engagementRequest.EngagementId = engagementId;
            engagementRequest.EngagementType = engagementType;

            // check if dispute required
            var cancellationStatus = CancellationStatus.New;
            if(blockUserDays == 0)
                cancellationStatus = CancellationStatus.Closed;

            engagementRequest.ReasonId = reasonId;
            engagementRequest.UserRemarks = userRemarks;
            engagementRequest.CustomerBlockDays = blockUserDays;
            engagementRequest.AttachmentId = attachmentId;
            engagementRequest.Status = cancellationStatus;
            engagementRequest.PostCancellationAction = postCancellationAction;
            engagementRequest.CancelledBy = cancellingParty;
            
            engagementRequest.CreateAudit(actorId);

            _cancellationRequestRepository.Insert(engagementRequest);

            return true;
        }

        public PagedList<CancellationRequestDTO> GetCancellationRequest(
            DateTime? searchDate, int searchType, string searchBuyer, int searchCancelledBy,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null)
        {
            var query = QueryableCancellationRequestEntity();

            #region filter conditions
            if (searchDate != null)
            {
                query = query.Where(x => x.SubmissionDate >= searchDate);
            }
            if (searchType != 0)
            {
                query = query.Where(x => x.EngagementType == (EngagementType)searchType);
            }
            if (!string.IsNullOrWhiteSpace(searchBuyer))
            {
                query = query.Where(x => x.EngagementPartyInfo.BuyerName.ToLower().Contains(searchBuyer.ToLower()));
            }
            if (searchCancelledBy != 0)
            {
                query = query.Where(x => x.CancelledBy == (EngagementParty)searchCancelledBy);
            }
            #endregion
            query = query.OrderByDescending(x => x.SubmissionDate);

            var data = new PagedList<CancellationRequestDTO>(query, pageIndex, pageSize);

            foreach (var d in data)
            {
                d.SellerBlockStatus = _blockCustomerService.GetBlockStatus(d.EngagementPartyInfo.SellerId);
            }
            return data;
        }

        public CancellationRequestDTO GetCancellationRequestEntityById(int cancellationRequestId)
        {
            return QueryableCancellationRequestEntity().Where(q => q.Id == cancellationRequestId).FirstOrDefault();
        }

        public CancellationRequestDTO GetCancellationRequestEntityByEngagement(int engagementId, EngagementType engagementType)
        {
            return QueryableCancellationRequestEntity()
                .FirstOrDefault(q => q.EngagementId == engagementId
                                     && q.EngagementType == engagementType);
        }

        internal int? GetCancellationRequestId(int engagementId, EngagementType engagementType)
        {
            return _cancellationRequestRepository.Table
                .FirstOrDefault(q => q.EngagementId == engagementId && q.EngagementType == engagementType && !q.Deleted)?.Id;
        }

        internal bool Update(int cancellationRequestId, int actorId, string adminRemarks, int blockUserDays, int? attachmentId = null)
        {
            var request = _cancellationRequestRepository.Table
                .FirstOrDefault(q => q.Id == cancellationRequestId && !q.Deleted);

            if(request == null)
                return false;

            if(request.Status == CancellationStatus.Closed)
                return false;

            request.AdminRemarks = adminRemarks;
            request.AttachmentId = attachmentId;
            request.CustomerBlockDays = blockUserDays;
            request.Status = CancellationStatus.UnderInvestigation;
            request.UpdateAudit(actorId);

            _cancellationRequestRepository.Update(request);

            return true;
        }

        internal bool AddAttachmentAfterClose(int cancellationRequestId, int actorId, int? attachmentId)
        {
            if(attachmentId == null)
                return false;

            var request = _cancellationRequestRepository.Table
                .FirstOrDefault(q => q.Id == cancellationRequestId && !q.Deleted);

            if (request == null)
                return false;

            if(request.Status != CancellationStatus.Closed)
                return false;

            request.AttachmentId = attachmentId;
            request.UpdateAudit(actorId);

            _cancellationRequestRepository.Update(request);

            return true;
        }

        internal IQueryable<CancellationRequestDTO> QueryableCancellationRequestEntity()
        {
            return from s in _engagementResolver.EnumeratedQuery(_cancellationRequestRepository.Table)
                   join c in _cancellationRequestRepository.Table on new { s.EngagementId, s.EngagementType } equals new { c.EngagementId, c.EngagementType }
                   join r in _reasonRepository.Table.DefaultIfEmpty() on c.ReasonId equals r.Id
                   where !c.Deleted
                   select new CancellationRequestDTO
                   {
                       Id = c.Id,
                       SubmissionDate = c.SubmissionDate,
                       EngagementType = c.EngagementType,
                       EngagementId = c.EngagementId,
                       CancelledBy = c.CancelledBy,
                       ReasonId = c.ReasonId,
                       UserRemarks = c.UserRemarks,
                       AttachmentId = c.AttachmentId,
                       AdminRemarks = c.AdminRemarks,
                       Status = c.Status,
                       CustomerBlockDays = c.CustomerBlockDays,
                       PostCancellationAction = c.PostCancellationAction,
                       CancelledByDescription = c.CancelledBy.GetDescription(),
                       Reason = r,
                       EngagementPartyInfo = s
                   };
        }
    }
}