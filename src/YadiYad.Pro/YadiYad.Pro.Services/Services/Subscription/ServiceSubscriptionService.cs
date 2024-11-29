using AutoMapper;
using Nop.Data;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Services.Subscription
{
    public class ServiceSubscriptionService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<ServiceSubscription> _serviceSubscriptionRepository;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ServiceSubscriptionService
            (IMapper mapper,
            ILogger logger,
            IDateTimeHelper dateTimeHelper,
            IRepository<ServiceSubscription> serviceSubscriptionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _serviceSubscriptionRepository = serviceSubscriptionRepository;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        public DateTime? GetApplyJobEntitledEndDateTime(int customerId) 
        {
            var utcNow = DateTime.UtcNow;

            var applyJobEntitledEndDateTime = _serviceSubscriptionRepository.Table.Where(x =>
                !x.Deleted
                && x.CustomerId == customerId
                && x.EndDate >= utcNow
                && x.StartDate <= utcNow
                && x.SubscriptionTypeId == (int)SubscriptionType.ApplyJob)
                .Select(x => (DateTime?)(x.StopDate != null ? x.StopDate.Value : x.EndDate))
                .OrderByDescending(x => x)
                .FirstOrDefault<DateTime?>();

            return applyJobEntitledEndDateTime;
        }

        public DateTime? GetViewProfileEntitledEndDateTime(int customerId, int jobProfileId)
        {
            var utcNow = DateTime.UtcNow;

            var viewProfileEntitledEndDateTime = _serviceSubscriptionRepository.Table.Where(x =>
                !x.Deleted
                && x.CustomerId == customerId
                && x.RefId == jobProfileId
                && x.EndDate >= utcNow
                && x.StartDate <= utcNow
                && x.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile)
                .Select(x => (DateTime?)(x.StopDate != null ? x.StopDate.Value : x.EndDate))
                .OrderByDescending(x => x)
                .FirstOrDefault<DateTime?>();

            return viewProfileEntitledEndDateTime;
        }

        public ServiceSubscription CreateServiceSubscription(
            int actorId,
            int customerId,
            SubscriptionType subscriptionType,
            int days,
            int refId=0)
        {
            var userLocalDate = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc).Date;
            _logger.Information($"userLocalDate: {userLocalDate}");
            var userLocalDateUtc = _dateTimeHelper.ConvertToUtcTime(userLocalDate, _dateTimeHelper.CurrentTimeZone);
            _logger.Information($"userLocalDateUtc: {userLocalDateUtc}");
            var serviceSubscription = new ServiceSubscription();
            serviceSubscription.CustomerId = customerId;
            serviceSubscription.SubscriptionTypeId = (int)subscriptionType;
            serviceSubscription.StartDate = userLocalDateUtc;

            //full date with last minute
            serviceSubscription.EndDate = serviceSubscription.StartDate.AddDays(days+1).AddMilliseconds(-1);
            serviceSubscription.RefId = refId;

            CreateAudit(serviceSubscription, actorId);
            _serviceSubscriptionRepository.Insert(serviceSubscription);

            return serviceSubscription;
        }

        /// <summary>
        /// ensure subscription is active for next no of days
        /// subscription is extend by add in new service subscription record
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="customerId"></param>
        /// <param name="subscriptionType"></param>
        /// <param name="days"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public ServiceSubscription EnsureServiceSubscriptionActiveNextNoDays(
            int actorId,
            int customerId,
            SubscriptionType subscriptionType,
            int days,
            int refId)
        {
            var userLocalDate = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc).Date;
            _logger.Information($"userLocalDate: {userLocalDate}");
            var userLocalDateUtc = _dateTimeHelper.ConvertToUtcTime(userLocalDate, _dateTimeHelper.CurrentTimeZone);
            _logger.Information($"userLocalDateUtc: {userLocalDateUtc}");

            var maxSubcriptionEndDate = _serviceSubscriptionRepository.Table
                .Where(ss => ss.Deleted == false
                && ss.RefId == refId
                && ss.SubscriptionTypeId == (int)subscriptionType)
                .Select(ss => (ss.StopDate != null ? ss.StopDate.Value : ss.EndDate))
                .Max()
                .AddMilliseconds(1);

            if(maxSubcriptionEndDate.Second == 59)
            {
                maxSubcriptionEndDate = maxSubcriptionEndDate.AddSeconds(1);
            }

            maxSubcriptionEndDate = maxSubcriptionEndDate.AddDays(-1);

            var dateRequiredBeActive = userLocalDateUtc.AddDays(days);

            var dayDiff = (dateRequiredBeActive - maxSubcriptionEndDate).Days;

            if(dayDiff <= 0)
            {
                return null;
            }

            var serviceSubscription = new ServiceSubscription();
            serviceSubscription.CustomerId = customerId;
            serviceSubscription.SubscriptionTypeId = (int)subscriptionType;
            serviceSubscription.EndDate = dateRequiredBeActive.AddDays(1).AddMilliseconds(-1);
            serviceSubscription.StartDate = new DateTime(
                Math.Max(
                    serviceSubscription.EndDate.AddMilliseconds(1).AddDays(1).AddDays(-1 * days).Ticks, 
                    maxSubcriptionEndDate.AddDays(1).Ticks)
            );

            //full date with last minute
            serviceSubscription.RefId = refId;

            CreateAudit(serviceSubscription, actorId);
            _serviceSubscriptionRepository.Insert(serviceSubscription);

            return serviceSubscription;
        }

        /// <summary>
        /// stop service subscription
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="subscriptionType"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public List<ServiceSubscription> StopAllActiveServiceSubscription(
            int actorId,
            SubscriptionType subscriptionType,
            int refId)
        {
            var utcNow = DateTime.UtcNow;

            var activeSubscriptions = _serviceSubscriptionRepository.Table
                .Where(x => x.Deleted == false
                && x.SubscriptionTypeId == (int)subscriptionType
                && x.RefId == refId
                && x.EndDate > utcNow
                && (x.StopDate == null
                    || x.StopDate > utcNow))
                .ToList();

            activeSubscriptions.ForEach(x =>
            {
                x.StopDate = utcNow;
                x.UpdateAudit(actorId);
            });

            _serviceSubscriptionRepository.Update(activeSubscriptions);

            return activeSubscriptions;
        }

        /// <summary>
        /// To be used be campaign to increase the subscrption days
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="days"></param>
        /// <param name="subsId"></param>
        internal void ExtendSubscription(int actorId, int days, int subsId)
        {
            if (days <= 0)
                return;
            
            var subscription = _serviceSubscriptionRepository.GetById(subsId);

            if (subscription == null)
                return;

            subscription.EndDate = (subscription.StopDate != null ? subscription.StopDate.Value : subscription.EndDate).AddDays(days);
            UpdateAudit(subscription, subscription, actorId);
            _serviceSubscriptionRepository.Update(subscription);
        }

        #endregion
    }
}
