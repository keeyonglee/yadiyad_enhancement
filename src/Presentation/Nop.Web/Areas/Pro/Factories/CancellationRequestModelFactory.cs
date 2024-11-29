using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Models.CancellationRequest;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Moderator;
using YadiYad.Pro.Services.Engagement;
using YadiYad.Pro.Services.Services.Moderator;

namespace Nop.Web.Areas.Pro.Factories
{
    public class CancellationRequestModelFactory
    {
        #region Fields

        private readonly ConsultationInvitationService _consultationInvitationService;
        private readonly ModeratorCancellationRequestService _moderatorCancellationRequestService;
        private readonly EngagementCancellationManager _engagementCancellationManager;
        #endregion

        #region Ctor

        public CancellationRequestModelFactory(
            ConsultationInvitationService consultationInvitationService,
            ModeratorCancellationRequestService moderatorCancellationRequestService,
            EngagementCancellationManager engagementCancellationManager)
        {
            _consultationInvitationService = consultationInvitationService;
            _moderatorCancellationRequestService = moderatorCancellationRequestService;
            _engagementCancellationManager = engagementCancellationManager;

        }

        #endregion

        #region Methods

        public virtual CancellationRequestSearchModel PrepareCancellationRequestSearchModel(CancellationRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual CancellationRequestListModel PrepareCancellationRequestListModel(CancellationRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //var cancellationRequest = _consultationInvitationService.GetAllConsultationInvitationsCancelled(searchModel.Date, searchModel.Type, searchModel.Buyer,
            //    searchModel.CancelledBy,
            //    pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var cancelRequest = _engagementCancellationManager.GetCancellationRequests(searchModel.Date, searchModel.Type, searchModel.Buyer, searchModel.CancelledBy,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new CancellationRequestListModel().PrepareToGrid(searchModel, cancelRequest, () =>
            {
                return cancelRequest.Select(entity =>
                {
                    var cancellationRequestModel = new CancellationRequestModel();
                    cancellationRequestModel.SubmissionDate = entity.SubmissionDate;
                    cancellationRequestModel.EngagementId = entity.EngagementId;
                    cancellationRequestModel.Remarks = entity.AdminRemarks;
                    cancellationRequestModel.CancelledBy = entity.CancelledByDescription;
                    cancellationRequestModel.EngagementType = entity.EngagementTypeDescription;
                    cancellationRequestModel.EngagementTypeId = (int)entity.EngagementType;
                    cancellationRequestModel.Reason = entity.Reason.Name;
                    cancellationRequestModel.BuyerName = entity.EngagementPartyInfo.BuyerName;
                    cancellationRequestModel.SellerName = entity.EngagementPartyInfo.SellerName;
                    cancellationRequestModel.ReasonAttachmentDownloadId = entity.AttachmentId != null ? entity.AttachmentId.Value : 0;
                    cancellationRequestModel.Status = entity.StatusDescription;
                    return cancellationRequestModel;
                });
            });
            return model;
        }

        public virtual CancellationRequestEditModel PrepareCancellationRequestModel(CancellationRequestEditModel model, ConsultationInvitation  entity, bool excludeProperties = false)
        {
            if (entity != null)
            {
                if (model == null)
                {
                    model = new CancellationRequestEditModel();
                    model.EngagementId = entity.Id;
                    model.ReasonAttachmentDownloadId = entity.CancellationDownloadId == null ? 0 : entity.CancellationDownloadId.Value;
                }
            }
            return model;
        }

        public virtual CancellationRequestEditModel PrepareCancellationRequestEditModel(CancellationRequestEditModel model, ModeratorCancellationRequestDTO dto)
        {
            if (dto != null)
            {
                if (model == null)
                {
                    model = new CancellationRequestEditModel();
                    model.EngagementId = dto.EngagementId;
                    model.ReasonAttachmentDownloadId = dto.AttachmentId;
                    model.SellerName = dto.SellerName;
                    model.SellerId = dto.SellerId;
                }
            }
            return model;
        }
        #endregion
    }
}
