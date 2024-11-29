using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Services.Models;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public class QueuedPushNotificationService : IQueuedPushNotificationService
    {
        #region Fields

        private readonly INopDataProvider _dataProvider;
        private readonly IRepository<AppQueuedPushNotification> _queuedPushNotificationRepository;

        #endregion

        #region Ctor

        public QueuedPushNotificationService(
            INopDataProvider dataProvider,
            IRepository<AppQueuedPushNotification> queuedPushNotificationRepository)
        {
            _dataProvider = dataProvider;
            _queuedPushNotificationRepository = queuedPushNotificationRepository;
        }

        #endregion

        #region Methods

        public void DeleteQueuedPushNotification(AppQueuedPushNotification queuedPushNotification)
        {
            if (queuedPushNotification == null)
                throw new ArgumentNullException(nameof(queuedPushNotification));

            _queuedPushNotificationRepository.Delete(queuedPushNotification);
        }
        public void DeleteSentQueuedPushNotification()
        {
            var query = _queuedPushNotificationRepository.Table;
            query = query.Where(qe => qe.SentOnUtc.HasValue);

            _queuedPushNotificationRepository.Delete(query);
        }

        public void InsertQueuedPushNotification(AppQueuedPushNotification queuedPushNotification)
        {
            if (queuedPushNotification == null)
                throw new ArgumentNullException(nameof(queuedPushNotification));

            _queuedPushNotificationRepository.Insert(queuedPushNotification);
        }

        public void UpdateQueuedPushNotification(AppQueuedPushNotification queuedPushNotification)
        {
            if (queuedPushNotification == null)
                throw new ArgumentNullException(nameof(queuedPushNotification));

            _queuedPushNotificationRepository.Update(queuedPushNotification);
        }

        public AppQueuedPushNotification GetQueuedPushNotificationById(int queuedPushNotificationId)
        {
            if (queuedPushNotificationId == 0)
                return null;

            return _queuedPushNotificationRepository.GetById(queuedPushNotificationId);
        }

        public IPagedList<AppQueuedPushNotification> GetAllQueuedPushNotifications(bool? sentStatus = null,
            bool enableDateConsideration = false, DateTime? sentFromUtc = null, DateTime? sentToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _queuedPushNotificationRepository.Table;

            if (sentStatus.HasValue)
                query = query.Where(qe => qe.SentOnUtc.HasValue == sentStatus.Value);

            if (enableDateConsideration)
            {
                query = query.Where(x => x.SentTries < 3);
                query = query.Where(e => !e.DontSendBeforeDateUtc.HasValue || e.DontSendBeforeDateUtc < DateTime.UtcNow);
            }

            if (sentFromUtc.HasValue)
                query = query.Where(e => e.SentOnUtc >= sentFromUtc);

            if (sentToUtc.HasValue)
                query = query.Where(e => e.SentOnUtc <= sentToUtc);

            query = query.OrderByDescending(e => e.Id);

            return new PagedList<AppQueuedPushNotification>(query, pageIndex, pageSize);
        }

        public IPagedList<AppNotificationModel> GetCustomerNotifications(int customerId, int appTypeId, int pageIndex, int pageSize)
        {
            var query = _queuedPushNotificationRepository.Table
                .Where(q => q.CustomerId == customerId)
                .Where(q => q.SentOnUtc != null);

            if (appTypeId > 0)
                query = query.Where(q => q.AppTypeId == appTypeId);

            var customerNotifications = from n in query
                group new {Id = n.Id} by new {CustomerId = n.CustomerId, UniqueId = n.UniqueNotificationId}
                into uniqueNotifications
                select new {Id = uniqueNotifications.Max(s => s.Id)};

            var notifications = from q in query
                join u in customerNotifications on q.Id equals u.Id
                select q;

            var result = notifications
                .OrderByDescending(q => q.SentOnUtc)
                .Select( r =>  new AppNotificationModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Body = r.Body,
                    ItemType = r.ActionTypeId,
                    ItemId = r.ActionValue,
                    BigPictureUrl = r.ImageUrl,
                    ApptypeId = r.AppTypeId
                });

            return new PagedList<AppNotificationModel>(result, pageIndex, pageSize);
        }

        #endregion
    }
}