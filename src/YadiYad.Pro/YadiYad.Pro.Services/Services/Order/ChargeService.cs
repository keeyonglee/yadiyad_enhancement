using AutoMapper;
using Nop.Core;
using Nop.Data;
using Nop.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Order
{
    public class ChargeService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<Charge> _chargeRepository;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public ChargeService
            (IMapper mapper,
            IDateTimeHelper dateTimeHelper,
            IRepository<Charge> chargeRepository)
        {
            _mapper = mapper;
            _dateTimeHelper = dateTimeHelper;
            _chargeRepository = chargeRepository;
        }

        #endregion

        #region Methods

        public ChargeDTO CreateCharge(int actorId, int customerId, ChargeDTO dto)
        {
            var charge = dto.ToModel(_mapper);

            CreateAudit(charge, actorId);

            _chargeRepository.Insert(charge);

            dto.Id = charge.Id;

            return dto;
        }

        public ChargeDTO UpdateCharge(int actorId, ChargeDTO dto)
        {
            #region update charge

            var charge = _chargeRepository.Table
                .Where(x => x.Id == dto.Id)
                .FirstOrDefault();

            var updatingOrder = dto.ToModel(_mapper);

            UpdateAudit(charge, updatingOrder, actorId);

            _chargeRepository.Update(updatingOrder);

            #endregion

            return dto;
        }

        public PagedListDTO<ChargeDTO> SearchCharges(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            ChargeSearchFilterDTO filterDTO = null)
        {
            var query = _chargeRepository.Table;
            query = query.Where(x => !x.Deleted);
            if (filterDTO != null)
            {
                if (filterDTO.ProductTypeId != 0)
                {
                    query = query.Where(x => x.ProductTypeId == filterDTO.ProductTypeId);
                }
                if (filterDTO.SubProductTypeId != 0)
                {
                    query = query.Where(x => x.SubProductTypeId == filterDTO.SubProductTypeId);
                }
                if (filterDTO.StartDate != null)
                {
                    query = query.Where(x => x.StartDate >= filterDTO.StartDate);
                }
                if (filterDTO.EndDate != null)
                {
                    query = query.Where(x => x.EndDate <= filterDTO.EndDate);
                }
                if (filterDTO.IsActive != null)
                {
                    query = query.Where(x => x.IsActive == filterDTO.IsActive);
                }
            }
            var record = query.ToList();
            List<ChargeDTO> result = record.Select(x => _mapper.Map<ChargeDTO>(x)).ToList();

            var totalCount = result.Count();
            var records = result.ToList();

            var response = new PagedListDTO<ChargeDTO>(records, pageIndex, pageSize, totalCount);

            return response;
        }

        public virtual IPagedList<Charge> SearchChargesTable(
            int searchProductId,
            int pageIndex = 0, int pageSize = int.MaxValue, 
            string keyword = null)
        {
            var query = _chargeRepository.Table
                .Where(x=>x.Deleted == false);

            if (searchProductId > 0)
                query = query.Where(n => n.ProductTypeId == searchProductId);


            query = query.OrderBy(n => n.ProductTypeId);

            var data = new PagedList<Charge>(query, pageIndex, pageSize);

            return data;
        }

        public virtual ChargeDTO GetById(int id)
        {
            if (id == 0)
                return null;

            var result = _chargeRepository.Table
                .Where(x => !x.Deleted
                    && x.Id == id
                )
                .Select(x => _mapper.Map<ChargeDTO>(x))
                .FirstOrDefault();

            return result;
        }

        public ChargeDTO GetLatestCharge(int productTypeId, int subProductTypeId, decimal? tierValue = null)
        {
            var today = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, TimeZoneInfo.Utc);

            var query = _chargeRepository.Table
                .Where(x => !x.Deleted
                    && x.IsActive
                    && x.ProductTypeId == productTypeId
                    && x.SubProductTypeId == subProductTypeId
                    && x.StartDate <= today.Date
                    && (x.EndDate == null
                       || x.EndDate >= today.Date)
                );

            //if(foc)
            //{
            //    query = query.Where(x => x.Value == 0);
            //}
            //else
            //{
            //    query = query.Where(x => x.Value > 0);
            //}

            var results = query
               .OrderByDescending(x => x.StartDate)
               .ThenByDescending(x => x.CreatedOnUTC)              
               .ToList();

            // Perform min and max range checking on payAmount
            var finalResult = results.Where(x => (x.MinRange == null && x.MaxRange == null)
                                                || (x.MinRange != null && x.MaxRange != null && x.MinRange <= tierValue && x.MaxRange >= tierValue))
                                .Select(x => _mapper.Map<ChargeDTO>(x))
                                .FirstOrDefault();

            return finalResult;
        }

        public virtual void Insert(Charge item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _chargeRepository.Insert(item);
        }

        public virtual Charge GetItemById(int id)
        {
            if (id == 0)
                return null;

            return _chargeRepository.GetById(id);
        }

        public virtual void Update(Charge item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _chargeRepository.Update(item);
        }


        #endregion
    }
}
