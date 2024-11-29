using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Models.CampaignManagement;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Services.Services.Campaign;

namespace Nop.Web.Areas.Pro.Factories
{
    public class CampaignManagementModelFactory
    {
        #region Fields

        private readonly CampaignManagementService _campaignManagementService;
        #endregion

        #region Ctor
        public CampaignManagementModelFactory(
            CampaignManagementService campaignManagementService)
        {
            _campaignManagementService = campaignManagementService;
        }

        #endregion

        #region Methods

        public virtual CampaignManagementSearchModel PrepareCampaignManagementSearchModel(CampaignManagementSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual CampaignManagementListModel PrepareCampaignManagementListModel(CampaignManagementSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            var campaign = _campaignManagementService.SearchCampaignManagementTable(searchModel.Title, searchModel.From, searchModel.Channel, searchModel.Beneficiary,
                searchModel.EngagementType,  searchModel.Type,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new CampaignManagementListModel().PrepareToGrid(searchModel, campaign, () =>
            {
                return campaign.Select(entity =>
                {
                    var campaignModel = new CampaignManagementModel();
                    campaignModel.Id = entity.Id;
                    campaignModel.Channel = entity.Channel;
                    campaignModel.Activity = entity.Activity;
                    campaignModel.FromText = entity.From.ToShortDateString();
                    campaignModel.UntilText = entity.Until != null ? entity.Until.Value.ToShortDateString() : "-";
                    campaignModel.TransactionLimit = entity.TransactionLimit;
                    campaignModel.EngagementType = entity.EngagementType.GetValueOrDefault();
                    campaignModel.Beneficiary = entity.Beneficiary;
                    campaignModel.Type = entity.Type;
                    campaignModel.Value1 = entity.Value1;
                    campaignModel.Value2 = entity.Value2;
                    campaignModel.Title = entity.Title;
                    campaignModel.CreatedOnUTC = entity.CreatedOnUTC;
                    return campaignModel;
                });
            });
            return model;
        }

        public virtual CampaignManagementModel PrepareCampaignManagementModel(CampaignManagementModel model, CampaignManagement campaign, bool excludeProperties = false)
        {
            if (campaign != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = campaign.ToModel<CampaignManagementModel>();
                }
            }
            return model;
        }

        #endregion
    }
}
