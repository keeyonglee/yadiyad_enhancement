using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Individual
{
    public class BillingAddressService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<BillingAddress> _billingAddressRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;

        #endregion

        #region Ctor

        public BillingAddressService
            (IMapper mapper,
            IRepository<BillingAddress> billingAddressRepository,
            IRepository<City> cityRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository)
        {
            _mapper = mapper;
            _billingAddressRepository = billingAddressRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
        }

        #endregion


        #region Methods

        public BillingAddressDTO CreateBillingAddress(int actorId, BillingAddressDTO dto)
        {
            var request = _mapper.Map<BillingAddress>(dto);

            CreateAudit(request, actorId);

            _billingAddressRepository.Insert(request);

            dto.Id = request.Id;

            return dto;
        }

        public BillingAddressDTO UpdateBillingAddressByCustomerId(int actorId, BillingAddressDTO dto)
        {
            var billingAddress = _billingAddressRepository.Table
                .Where(x => x.CustomerId == dto.CustomerId)
                .FirstOrDefault();

            if (billingAddress == null)
            {
                return CreateBillingAddress(actorId, dto);
            }

            dto.Id = billingAddress.Id;
            var request = _mapper.Map<BillingAddress>(dto);
            UpdateAudit(billingAddress, request, actorId);
            _billingAddressRepository.Update(request);

            return dto;
        }

        public BillingAddressDTO GetBillingAddressById(int id)
        {
            if (id == 0)
                return null;

            var record = _billingAddressRepository.Table
                    .Where(x => !x.Deleted && x.Id == id)
                    .Join(_cityRepository.Table,
                        x => x.CityId,
                        y => y.Id,
                        (x, y) => new { x, y }
                        )
                    .Join(_stateProvinceRepository.Table,
                        x => x.x.StateProvinceId,
                        z => z.Id,
                        (x, z) => new { x = x.x, y = x.y, z }
                    )
                    .Join(_countryRepository.Table,
                        x => x.x.CountryId,
                        c => c.Id,
                        (x, c) => new { x = x.x, y = x.y, z = x.z, c }
                    )
                    .Select(x => new BillingAddressDTO
                    {
                        Id = x.x.Id,
                        CustomerId = x.x.CustomerId,
                        Address1 = x.x.Address1,
                        Address2 = x.x.Address2,
                        ZipPostalCode = x.x.ZipPostalCode,
                        CityId = x.x.CityId,
                        CityName = x.y.Name,
                        StateProvinceId = x.x.StateProvinceId,
                        StateProvinceName = x.z.Name,
                        CountryId = x.x.CountryId,
                        CountryName = x.c.Name,
                        CreatedById = x.x.CreatedById,
                        UpdatedById = x.x.UpdatedById,
                        CreatedOnUTC = x.x.CreatedOnUTC,
                        UpdatedOnUTC = x.x.UpdatedOnUTC,
                    })
                    .FirstOrDefault();

            return record;
        }

        public BillingAddressDTO GetBillingAddressByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var record = _billingAddressRepository.Table
                    .Where(x => !x.Deleted && x.CustomerId == customerId)
                    .Join(_cityRepository.Table,
                        x => x.CityId,
                        y => y.Id,
                        (x, y) => new { x, y }
                        )
                    .Join(_stateProvinceRepository.Table,
                        x => x.x.StateProvinceId,
                        z => z.Id,
                        (x, z) => new { x = x.x, y = x.y, z }
                    )
                    .Join(_countryRepository.Table,
                        x => x.x.CountryId,
                        c => c.Id,
                        (x, c) => new { x = x.x, y = x.y, z = x.z, c }
                    )
                    .Select(x => new BillingAddressDTO
                    {
                        Id = x.x.Id,
                        CustomerId = x.x.CustomerId,
                        Address1 = x.x.Address1,
                        Address2 = x.x.Address2,
                        ZipPostalCode = x.x.ZipPostalCode,
                        CityId = x.x.CityId,
                        CityName = x.y.Name,
                        StateProvinceId = x.x.StateProvinceId,
                        StateProvinceName = x.z.Name,
                        CountryId = x.x.CountryId,
                        CountryName = x.c.Name,
                        CreatedById = x.x.CreatedById,
                        UpdatedById = x.x.UpdatedById,
                        CreatedOnUTC = x.x.CreatedOnUTC,
                        UpdatedOnUTC = x.x.UpdatedOnUTC,
                    })
                    .FirstOrDefault();

            return record;
        }

        #endregion
    }
}
