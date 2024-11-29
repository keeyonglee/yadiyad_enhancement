using System;
using System.Linq.Expressions;

using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class EngagementCustomer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }

    public class EngagementPartyTypeInfo
    {
        public string Buyer { get; set; } = "Buyer";
        public string Moderator { get; set; } = "Moderator";
        public string Seller { get; set; } = "Seller";

        public string GetInfo(EngagementParty engagementParty)
            => engagementParty switch
            {
                EngagementParty.Buyer => Buyer,
                EngagementParty.Seller => Seller,
                EngagementParty.Moderator => Moderator
            };
    }

    public class EngagementPartyInfo
    {
        public int EngagementId { get; set; }
        public EngagementType EngagementType { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public int? ModeratorId { get; set; }
        public string ModeratorName { get; set; }
        public bool IsEscrow { get; set; }

        public void SetBuyer(EngagementCustomer customer)
        {
            BuyerId = customer?.CustomerId ?? 0;
            BuyerName = customer?.Name;
        }

        public void SetModerator(EngagementCustomer customer)
        {
            ModeratorId = customer?.CustomerId ?? 0;
            ModeratorName = customer?.Name;
        }

        public void SetSeller(EngagementCustomer customer)
        {
            SellerId = customer?.CustomerId ?? 0;
            SellerName = customer?.Name;
        }

        public EngagementCustomer GetBuyer() => new EngagementCustomer { CustomerId = BuyerId, Name = BuyerName };
        public EngagementCustomer GetSeller() => new EngagementCustomer { CustomerId = SellerId, Name = SellerName };
        public EngagementCustomer GetModerator()
        {
            return ModeratorId == null ? null : new EngagementCustomer { CustomerId = ModeratorId.Value, Name = ModeratorName };
        }

        public EngagementCustomer GetCustomer(EngagementParty engagementParty)
            => engagementParty switch
            {
                EngagementParty.Buyer => GetBuyer(),
                EngagementParty.Seller => GetSeller(),
                EngagementParty.Moderator => GetModerator()
            };


        public static Expression<Func<EngagementPartyInfo, TIn, EngagementPartyInfo>> QueryFunc<TIn>()
            where TIn:EngagementBaseEntity
        {
            return (p, m) => new EngagementPartyInfo
            {
                EngagementId = p.EngagementId,
                EngagementType = m.EngagementType,
                SellerId = p.SellerId,
                BuyerId = p.BuyerId,
                SellerName = p.SellerName,
                BuyerName = p.BuyerName,
                ModeratorId = p.ModeratorId,
                ModeratorName = p.ModeratorName
            };
        }
    }
}