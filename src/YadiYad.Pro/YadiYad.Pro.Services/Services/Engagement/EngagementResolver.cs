using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Common;
using System.Linq.Expressions;
namespace YadiYad.Pro.Services.Services.Engagement
{
    public class EngagementResolver
    {
        private readonly IEnumerable<IEngagementService> _engagementServices;
        private readonly EngagementType[] _engagementTypes = (EngagementType[])Enum.GetValues(typeof(EngagementType));

        public EngagementResolver(
            IEnumerable<IEngagementService> engagementServices)
        {
            _engagementServices = engagementServices;
        }

        public ProductType GetEngagementProductType(EngagementType engagementType)
            => engagementType switch
            {
                EngagementType.Job => ProductType.JobEnagegementFee,
                EngagementType.Service => ProductType.ServiceEnagegementFee,
                EngagementType.Consultation => ProductType.ConsultationEngagementFee,
                _ => throw new InvalidOperationException("Fail to cast EngagementType to ProductType.")
            };

        public IEngagementService Resolve(EngagementType engagementType)
            => _engagementServices.Last(q => q.EngagementType == engagementType);

        private IQueryable<EngagementPartyInfo> QueryableParty(EngagementType engagementType)
            => Resolve(engagementType).QueryableEngagementParties();

        public IQueryable<EngagementPartyInfo> EnumeratedQuery<TIn>(IQueryable<TIn> entity)
            where TIn : EngagementBaseEntity
        {
            var query = QueryWithType<TIn>(_engagementTypes[0], entity);
            for (int i = 1; i < _engagementTypes.Length; i++)
            {
                query = query.Union(QueryWithType<TIn>(_engagementTypes[i], entity));
            }
            return query;
        }

        public IQueryable<TOut> EnumeratedQuery<TIn, TOut>(IQueryable<TIn> entity, Expression<Func<EngagementPartyInfo, TIn, TOut>> selector)
            where TIn : EngagementBaseEntity
            where TOut : EngagementPartyInfo, new()
        {
            var query = QueryWithType<TIn, TOut>(_engagementTypes[0], entity, selector);
            for (int i = 1; i < _engagementTypes.Length; i++)
            {
                query = query.Union(QueryWithType<TIn, TOut>(_engagementTypes[i], entity, selector));
            }
            return query;
        }

        private IQueryable<TOut> QueryWithType<TIn, TOut>(
            EngagementType engType,
            IQueryable<TIn> queryingEntity,
            Expression<Func<EngagementPartyInfo, TIn, TOut>> selector)
            where TIn : EngagementBaseEntity
            where TOut : EngagementPartyInfo, new()
                => QueryableParty(engType)
                  .Join(queryingEntity, m => new { m.EngagementId, m.EngagementType }, p => new { p.EngagementId, p.EngagementType }, selector);

        private IQueryable<EngagementPartyInfo> QueryWithType<TIn>(
            EngagementType engType,
            IQueryable<TIn> queryingEntity)
            where TIn : EngagementBaseEntity
        {
            return QueryableParty(engType)
                  .Join(queryingEntity, m => new { m.EngagementId, m.EngagementType }, p => new { p.EngagementId, p.EngagementType }, EngagementPartyInfo.QueryFunc<TIn>());
        }
    }
}