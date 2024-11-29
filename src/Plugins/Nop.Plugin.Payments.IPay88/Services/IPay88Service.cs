using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Payments.IPay88.Domain;
using Nop.Services.Caching;

namespace Nop.Plugin.Payments.IPay88.Services
{
    public partial class IPay88Service : IIPay88Service
    {
        #region CatchKey
        /// <summary>
        /// Key for nopCommerce.com ipay88 cache
        /// </summary>
        public static string PaymentsIPay88AllPrefixCacheKey => "Nop.Payments.IPay88.";
        public static CacheKey PaymentsIPay88AllKey => new CacheKey("Nop.Payments.IPay88.all-{0}-{1}", PaymentsIPay88AllPrefixCacheKey);
        #endregion

        #region Constants

        private const string PAYMENTS_IPAY88_PATTERN_KEY = "Nop.Payments.IPay88.";

        #endregion

        #region Fields

        private readonly IRepository<IPay88PaymentRecord> _repository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;

        #endregion

        #region Ctor

        public IPay88Service(IStaticCacheManager staticCacheManager,
            ICacheKeyService cacheKeyService,
            IRepository<IPay88PaymentRecord> repository)
        {
            this._staticCacheManager = staticCacheManager;
            this._cacheKeyService = cacheKeyService;
            this._repository = repository;
        }

        #endregion

        #region Methods

        public virtual void DeleteIPay88PaymentRecord(IPay88PaymentRecord iPay88PaymentRecord)
        {
            if (iPay88PaymentRecord == null)
                throw new ArgumentNullException("iPay88PaymentRecord");

            _repository.Delete(iPay88PaymentRecord);

            _staticCacheManager.RemoveByPrefix(PAYMENTS_IPAY88_PATTERN_KEY);
        }

        public virtual List<IPay88PaymentRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(PaymentsIPay88AllKey, pageIndex, pageSize);

            return _staticCacheManager.Get(cacheKey, () =>
            {
                var query = from sbw in _repository.Table
                            orderby sbw.Id
                            select sbw;

                var records = query.ToList();
                return records;
            });
        }

        public virtual IPay88PaymentRecord FindRecord(string refNo, string internalPaymentStatus = "")
        {
            //filter by weight and shipping method
            var matchedPay88PaymentRecords = GetAll()
                .Where(p => p.PaymentNo == refNo);

            if (!string.IsNullOrEmpty(internalPaymentStatus))
            {
                matchedPay88PaymentRecords = matchedPay88PaymentRecords.Where(p => p.Status == internalPaymentStatus);
            }

            return matchedPay88PaymentRecords.FirstOrDefault();
        }

        public virtual IPay88PaymentRecord GetById(int iPay88PaymentRecordId)
        {
            if (iPay88PaymentRecordId == 0)
                return null;

            return _repository.GetById(iPay88PaymentRecordId);
        }

        public virtual void InsertIPay88PaymentRecord(IPay88PaymentRecord iPay88PaymentRecord)
        {
            if (iPay88PaymentRecord == null)
                throw new ArgumentNullException("iPay88PaymentRecord");

            _repository.Insert(iPay88PaymentRecord);

            _staticCacheManager.RemoveByPrefix(PAYMENTS_IPAY88_PATTERN_KEY);
        }

        public virtual void UpdateIPay88PaymentRecord(IPay88PaymentRecord iPay88PaymentRecord)
        {
            if (iPay88PaymentRecord == null)
                throw new ArgumentNullException("iPay88PaymentRecord");

            _repository.Update(iPay88PaymentRecord);

            _staticCacheManager.RemoveByPrefix(PAYMENTS_IPAY88_PATTERN_KEY);
        }

        public virtual IPay88PaymentRecord GetByOrderId(int orderId, int orderTypeId)
        {
            return _repository.Table
                .FirstOrDefault(q => q.UniqueId == orderId && q.OrderTypeId == orderTypeId);
        }

        #endregion
    }
}
