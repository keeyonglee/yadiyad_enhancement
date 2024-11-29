using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Services.DTO.Campaign;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class CampaignManagementService
    {
        #region Fields

        private readonly IRepository<CampaignManagement> _campaignManagementRepository;

        #endregion

        #region Ctor
        public CampaignManagementService(
            IRepository<CampaignManagement> campaignManagementRepository)
        {
            _campaignManagementRepository = campaignManagementRepository;
        }

        #endregion

        #region Methods
        public Dictionary<int, int[]> GetValidShuqRewardsIndividual()
        {
            var validIndividualRewards = new Dictionary<int, int[]>();
            validIndividualRewards.Add((int)CampaignActivityIndividual.Payout, new int[] { (int)CampaignType.ChargesWaiver });
            return validIndividualRewards;
        }

        public Dictionary<int, int[]> GetValidRewardsIndividual()
        {
            var validIndividualRewards = new Dictionary<int, int[]>();
            validIndividualRewards.Add((int)CampaignActivityIndividual.Registration, new int[] { (int)CampaignType.PayToApplyJobs, (int)CampaignType.CreditVoucher });
            validIndividualRewards.Add((int)CampaignActivityIndividual.RegistrationReferral, new int[] { (int)CampaignType.PayToApplyJobs, (int)CampaignType.CreditVoucherReferral });
            validIndividualRewards.Add((int)CampaignActivityIndividual.Payout, new int[] { (int)CampaignType.ChargesWaiver });
            validIndividualRewards.Add((int)CampaignActivityIndividual.SubmitRating, new int[] { (int)CampaignType.CreditVoucher });
            return validIndividualRewards;
        }

        public Dictionary<int, int[]> GetValidRewardsOrganization()
        {
            var validOrganizationRewards = new Dictionary<int, int[]>();
            validOrganizationRewards.Add((int)CampaignActivityOrganization.NewJobPost, new int[] { (int)CampaignType.PayToViewAndInvite});
            validOrganizationRewards.Add((int)CampaignActivityOrganization.ExtendJobPost, new int[] { (int)CampaignType.ExtendPayToViewAndInvite});
            validOrganizationRewards.Add((int)CampaignActivityOrganization.PayToView, new int[] {(int)CampaignType.CreditVoucher });
            validOrganizationRewards.Add((int)CampaignActivityOrganization.SubmitRating, new int[] {(int)CampaignType.CreditVoucher });
            
            return validOrganizationRewards;
        }

        public virtual IPagedList<CampaignManagement> SearchCampaignManagementTable(
            string title, DateTime? date, int channel, int beneficiary, int? engagementType, int campaignType,
          int pageIndex = 0,
          int pageSize = int.MaxValue)
        {
            var query = _campaignManagementRepository.Table.Where(x => x.Deleted == false);

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(n => n.Title.ToLower().Contains(title.ToLower()));
            }
            if (date != null)
            {
                query = query.Where(n => n.From >= date);
            }
            if (channel != 0)
            {
                query = query.Where(n => n.Channel == channel);
            }
            if (beneficiary != 0)
            {
                query = query.Where(n => n.Beneficiary == beneficiary);
            }
            if (engagementType != 0 && engagementType != null)
            {
                query = query.Where(n => n.EngagementType == engagementType);
            }
            if (campaignType != 0)
            {
                query = query.Where(n => n.Type == campaignType);
            }

            query = query.OrderByDescending(n => n.Id);

            var data = new PagedList<CampaignManagement>(query, pageIndex, pageSize);

            return data;
        }

        public virtual void Insert(CampaignManagement expertise)
        {
            if (expertise == null)
                throw new ArgumentNullException(nameof(expertise));

            _campaignManagementRepository.Insert(expertise);
        }

        public virtual CampaignManagement GetById(int campaignId)
        {
            if (campaignId == 0)
                return null;

            return _campaignManagementRepository.Table
                .FirstOrDefault(q => q.Id == campaignId);
        }

        public virtual void Update(CampaignManagement expertise)
        {
            if (expertise == null)
                throw new ArgumentNullException(nameof(expertise));

            _campaignManagementRepository.Update(expertise);
        }
        
        public virtual List<CampaignManagement> GetActiveCampaignsForActivity(CampaignActivityIndividual activity, CampaignChannel channel)
        {
            return GetActiveCampaigns(channel)
                .Where(q => q.Activity == (int)activity
                            && q.Beneficiary == (int)CampaignBeneficiary.Individual)
                .ToList();
        }

        public virtual List<CampaignManagement> GetActiveCampaignsForActivity(CampaignActivityOrganization activity, CampaignChannel channel)
        {
            return GetActiveCampaigns(channel)
                .Where(q => q.Activity == (int)activity
                            && q.Beneficiary == (int)CampaignBeneficiary.Organization)
                .ToList();
        }

        internal bool IsCampaignActive(int campaignId, CampaignActivityOrganization activity, CampaignChannel channel)
        {
            return GetActiveCampaigns(channel)
                .Any(q => !q.Deleted
                          && q.Activity == (int)activity
                          && q.Beneficiary == (int)CampaignBeneficiary.Organization
                          && q.Id == campaignId);
        }

        #endregion
        
        #region private
        private IQueryable<CampaignManagement> GetActiveCampaigns(CampaignChannel channel)
        {
            return _campaignManagementRepository.Table
                .Where(q => q.From < DateTime.Now
                            && q.Until > DateTime.Now
                            && q.Channel == (int)channel
                            && !q.Deleted);
        }

        #endregion
    }
}
