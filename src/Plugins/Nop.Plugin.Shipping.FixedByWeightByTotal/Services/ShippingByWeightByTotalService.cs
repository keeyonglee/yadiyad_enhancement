using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;
using Nop.Services.Caching;
using Nop.Services.ShippingShuq;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Services
{
    /// <summary>
    /// Represents service shipping by weight service implementation
    /// </summary>
    public partial class ShippingByWeightByTotalService : IShippingByWeightByTotalService
    {
        #region Constants

        /// <summary>
        /// Key for caching all records
        /// </summary>
        private readonly CacheKey _shippingByWeightByTotalAllKey = new CacheKey("Nop.shippingbyweightbytotal.all", SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY);
        private const string SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY = "Nop.shippingbyweightbytotal.";

        #endregion

        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IRepository<ShippingByWeightByTotalRecord> _sbwtRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly WarehouseService _warehouseService;

        #endregion

        #region Ctor

        public ShippingByWeightByTotalService(ICacheKeyService cacheKeyService,
            IRepository<ShippingByWeightByTotalRecord> sbwtRepository,
            IStaticCacheManager staticCacheManager,
            WarehouseService warehouseService)
        {
            _sbwtRepository = sbwtRepository;
            _staticCacheManager = staticCacheManager;
            _cacheKeyService = cacheKeyService;
            _warehouseService = warehouseService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all shipping by weight records
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of the shipping by weight record</returns>
        public virtual IPagedList<ShippingByWeightByTotalRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = _cacheKeyService.PrepareKeyForShortTermCache(_shippingByWeightByTotalAllKey);
            var rez = _staticCacheManager.Get(key, () =>
            {
                var query = from sbw in _sbwtRepository.Table
                            orderby sbw.StoreId, sbw.CountryId, sbw.StateProvinceId, sbw.Zip, sbw.ShippingMethodId, sbw.WeightFrom, sbw.OrderSubtotalFrom
                            select sbw;

                return query.ToList();
            });

            var records = new PagedList<ShippingByWeightByTotalRecord>(rez, pageIndex, pageSize);

            return records;
        }

        /// <summary>
        /// Filter Shipping Weight Records
        /// </summary>
        /// <param name="shippingMethodId">Shipping method identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="stateProvinceId">State identifier</param>
        /// <param name="zip">Zip postal code</param>
        /// <param name="weight">Weight</param>
        /// <param name="orderSubtotal">Order subtotal</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of the shipping by weight record</returns>
        public virtual IPagedList<ShippingByWeightByTotalRecord> FindRecords(int shippingMethodId, int storeId, int warehouseId,
            int countryId, int stateProvinceId, string zip, decimal? weight, decimal? orderSubtotal, int senderStateId, int pageIndex, int pageSize)
        {
            var senderFromPeninsularMalaysia = false;
            var senderFromEastMalaysia = false;
            var destinationGroup = "";
            switch (senderStateId)
            {
                case 334:
                case 335:
                case 336:
                case 337:
                case 338:
                case 339:
                case 340:
                case 341:
                case 342:
                case 345:
                case 346:
                case 348:
                case 349:
                case 350:
                    senderFromPeninsularMalaysia = true;
                    break;
                case 343:
                case 344:
                    senderFromEastMalaysia = true;
                    break;
                default:
                    break;
            }

            zip = zip?.Trim() ?? string.Empty;

            if (zip == "32300")
            {
                destinationGroup = "Pulau.Pangkor";
            }
            else if (zip == "07000" || zip == "07007" || zip == "07009")
            {
                destinationGroup = "Pulau.Langkawi";
            }
            else if (stateProvinceId == 334 || stateProvinceId == 335 || stateProvinceId == 336 || stateProvinceId == 337 ||
                stateProvinceId == 338 || stateProvinceId == 339 || stateProvinceId == 340 || stateProvinceId == 341 ||
                stateProvinceId == 342 || stateProvinceId == 345 || stateProvinceId == 346 || stateProvinceId == 348 ||
                stateProvinceId == 349 || stateProvinceId == 350)
            {
                if (senderFromPeninsularMalaysia)
                {
                    destinationGroup = "Within.Peninsular.Malaysia";
                }
                else if (senderFromEastMalaysia)
                {
                    destinationGroup = "East.Malaysia.To.Peninsular.Malaysia";
                }
            }
            //sabah
            else if (stateProvinceId == 343)
            {
                if (senderFromPeninsularMalaysia)
                {
                    destinationGroup = "Peninsular.Malaysia.To.Sabah";
                }
                else if (senderFromEastMalaysia)
                {
                    destinationGroup = "Within.East.Malaysia";
                }
            }
            //sarawak
            else if (stateProvinceId == 344)
            {
                if (senderFromPeninsularMalaysia)
                {
                    destinationGroup = "Peninsular.Malaysia.To.Sarawak";
                }
                else if (senderFromEastMalaysia)
                {
                    destinationGroup = "Within.East.Malaysia";
                }
            }

            var warehouseIdDestination = _warehouseService.GetWarehouseByName(destinationGroup);

            //filter by weight and shipping method
            var existingRates = GetAll()
                .Where(sbw => sbw.ShippingMethodId == shippingMethodId && (!weight.HasValue || weight >= sbw.WeightFrom && weight <= sbw.WeightTo)
                && sbw.WarehouseId == warehouseIdDestination).ToList();

            //sort from particular to general, more particular cases will be the first
            //var foundRecords = existingRates.OrderBy(r => r.StoreId == 0).ThenBy(r => r.WarehouseId == 0);

            var records = new PagedList<ShippingByWeightByTotalRecord>(existingRates, pageIndex, pageSize);
            
            return records;
        }

        /// <summary>
        /// Get a shipping by weight record by passed parameters
        /// </summary>
        /// <param name="shippingMethodId">Shipping method identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="stateProvinceId">State identifier</param>
        /// <param name="zip">Zip postal code</param>
        /// <param name="weight">Weight</param>
        /// <param name="orderSubtotal">Order subtotal</param>
        /// <returns>Shipping by weight record</returns>
        public virtual ShippingByWeightByTotalRecord FindRecords(int shippingMethodId, int storeId, int warehouseId, 
            int countryId, int stateProvinceId, string zip, decimal weight, decimal orderSubtotal, int senderStateId)
        {
            var foundRecords = FindRecords(shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip, weight, orderSubtotal, senderStateId, 0, int.MaxValue);

            return foundRecords.FirstOrDefault();
        }

        /// <summary>
        /// Get a shipping by weight record by identifier
        /// </summary>
        /// <param name="shippingByWeightRecordId">Record identifier</param>
        /// <returns>Shipping by weight record</returns>
        public virtual ShippingByWeightByTotalRecord GetById(int shippingByWeightRecordId)
        {
            if (shippingByWeightRecordId == 0)
                return null;

            return _sbwtRepository.GetById(shippingByWeightRecordId);
        }

        /// <summary>
        /// Insert the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        public virtual void InsertShippingByWeightRecord(ShippingByWeightByTotalRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwtRepository.Insert(shippingByWeightRecord);

            _staticCacheManager.RemoveByPrefix(SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY);
        }

        /// <summary>
        /// Update the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        public virtual void UpdateShippingByWeightRecord(ShippingByWeightByTotalRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwtRepository.Update(shippingByWeightRecord);

            _staticCacheManager.RemoveByPrefix(SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY);
        }

        /// <summary>
        /// Delete the shipping by weight record
        /// </summary>
        /// <param name="shippingByWeightRecord">Shipping by weight record</param>
        public virtual void DeleteShippingByWeightRecord(ShippingByWeightByTotalRecord shippingByWeightRecord)
        {
            if (shippingByWeightRecord == null)
                throw new ArgumentNullException(nameof(shippingByWeightRecord));

            _sbwtRepository.Delete(shippingByWeightRecord);

            _staticCacheManager.RemoveByPrefix(SHIPPINGBYWEIGHTBYTOTAL_PATTERN_KEY);
        }

        #endregion
    }
}
