using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;

namespace YadiYad.Pro.Services.Individual
{
    public class IndividualProfileService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<IndividualProfile> _IndividualProfileRepository;
        private readonly IRepository<StateProvince> _StateProvinceRepository;
        private readonly IRepository<Country> _CountryRepository;
        private readonly IRepository<IndividualInterestHobby> _IndividualInterestRepository;
        private readonly IRepository<InterestHobby> _InterestHobbyRepository;
        private readonly IRepository<City> _CityRepository;
        private readonly BillingAddressService _billingAddressService;


        #endregion

        #region Ctor

        public IndividualProfileService
            (IMapper mapper,
            IRepository<IndividualProfile> IndividualProfileRepository,
            IRepository<StateProvince> StateProvinceRepository,
            IRepository<Country> CountryRepository,
            IRepository<IndividualInterestHobby> IndividualInterestRepository,
            IRepository<InterestHobby> InterestHobbyRepository,
            IRepository<City> CityRepository,
            BillingAddressService billingAddressService)
        {
            _mapper = mapper;
            _IndividualProfileRepository = IndividualProfileRepository;
            _StateProvinceRepository = StateProvinceRepository;
            _CountryRepository = CountryRepository;
            _IndividualInterestRepository = IndividualInterestRepository;
            _InterestHobbyRepository = InterestHobbyRepository;
            _CityRepository = CityRepository;
            _billingAddressService = billingAddressService;

        }

        #endregion

        private DateTime _maxBirthdayAllowed
        {
            get
            {
                return DateTime.Today.AddYears(-10);
            }
        }

        #region Methods

        public virtual void CreateIndividualProfile(IndividualProfileDTO dto)
        {
            if(dto.DateOfBirth > _maxBirthdayAllowed)
            {
                throw new Exception($"Date of birth should not later than {_maxBirthdayAllowed.ToString("dd MMM yyyy")}.");
            }
         
            var request = _mapper.Map<IndividualProfile>(dto);
            request.IsOnline = true;
            _IndividualProfileRepository.Insert(request);

            var interestHobbyRequest = dto.InterestHobbies
                .Select(x => new IndividualInterestHobby
                {
                    CustomerId = request.CustomerId,
                    InterestHobbyId = x.Id,
                    CreatedById = request.CustomerId,
                    CreatedOnUTC = DateTime.UtcNow
                }).ToList();
            _IndividualInterestRepository.Insert(interestHobbyRequest);

            if (dto.BillingAddress != null)
            {
                dto.BillingAddress.CustomerId = request.CustomerId;
                _billingAddressService.CreateBillingAddress(request.CustomerId, dto.BillingAddress);
            }

        }

        public virtual void UpdateIndividualProfile(int actorId, IndividualProfileDTO dto)
        {
            if (dto.DateOfBirth > _maxBirthdayAllowed)
            {
                throw new Exception($"Date of birth should not later than {_maxBirthdayAllowed.ToString("dd MMM yyyy")}.");
            }

            var request = _mapper.Map<IndividualProfile>(dto);
            var indvProflie = 
                _IndividualProfileRepository.Table
                .Where(x => x.Deleted == false 
                && x.Id == dto.Id)
                .FirstOrDefault();

            if(indvProflie == null)
            {
                throw new KeyNotFoundException("Individual profile not found.");
            }

            request.UpdateAudit(actorId);

            _IndividualProfileRepository.Update(request);

            var interestHobbyRequest = dto.InterestHobbies
                .Select(x => new IndividualInterestHobby
                {
                    CustomerId = request.CustomerId,
                    InterestHobbyId = x.Id,
                    CreatedById = request.CustomerId,
                    CreatedOnUTC = DateTime.UtcNow
                }).ToList();
            var individualInterestRecord = _IndividualInterestRepository.Table.Where(x => x.CustomerId == request.CustomerId).ToList();
            _IndividualInterestRepository.Delete(individualInterestRecord);
            _IndividualInterestRepository.Insert(interestHobbyRequest);

            if (dto.BillingAddress != null)
            {
                dto.BillingAddress.CustomerId = request.CustomerId;
                _billingAddressService.UpdateBillingAddressByCustomerId(request.CustomerId, dto.BillingAddress);
            }
        }
        public virtual void UpdateIndividualSSTRegNo(IndividualProfileDTO dto)
        {
            var request = _mapper.Map<IndividualProfile>(dto);
            _IndividualProfileRepository.Update(request);
        }

        public IndividualProfileDTO GetIndividualProfileByEmail(string email)
        {
            if (email == null)
                return null;

            var query = from i in _IndividualProfileRepository.Table
                        where i.Email == email
                        select i;

            var record = query.FirstOrDefault();

            if (record is null)
                return null;

            var titleRecord = ((IndividualTitle)record.Title).GetDescription();
            var genderRecord = ((Gender)record.Gender).GetDescription();
            var stateProvinceRecord = _StateProvinceRepository.Table.Where(x => x.Id == record.StateProvinceId).FirstOrDefault();
            var countryRecord = _CountryRepository.Table.ToList();
            var individualInterestQuery = from b in _InterestHobbyRepository.Table
                                          join u in _IndividualInterestRepository.Table on b.Id equals u.InterestHobbyId
                                          where u.Id == record.Id
                                          && u.Deleted == false
                                          select b;
            var individualInterestRecord = individualInterestQuery.Select(x => new InterestHobbyDTO { Id = x.Id, Name = x.Name }).ToList();

            var response = _mapper.Map<IndividualProfileDTO>(record);

            response.TitleName = titleRecord;
            response.GenderText = genderRecord;
            response.NationalityName = countryRecord.Where(x => x.Id == record.NationalityId).Select(x => x.Name).FirstOrDefault();
            response.StateProvinceName = stateProvinceRecord.Name;
            response.CountryName = countryRecord.Where(x => x.Id == record.CountryId).Select(x => x.Name).FirstOrDefault();
            response.InterestHobbies = individualInterestRecord;

            return response;

        }
        public IndividualProfileDTO GetIndividualProfileById(int id)
        {
            if (id == 0)
                return null;

            var query = _IndividualProfileRepository.Table;
            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var titleRecord = ((IndividualTitle)record.Title).GetDescription();
            var genderRecord = ((Gender)record.Gender).GetDescription();
            var stateProvinceRecord = _StateProvinceRepository.Table.Where(x => x.Id == record.StateProvinceId).FirstOrDefault();
            var countryRecord = _CountryRepository.Table.ToList();
            var individualInterestQuery = from b in _InterestHobbyRepository.Table
                                          join u in _IndividualInterestRepository.Table on b.Id equals u.InterestHobbyId
                                          where u.Id == id
                                          && u.Deleted == false
                                          select b;
            var individualInterestRecord = individualInterestQuery.Select(x => new InterestHobbyDTO { Id = x.Id, Name = x.Name }).ToList();

            var response = _mapper.Map<IndividualProfileDTO>(record);

            response.TitleName = titleRecord;
            response.GenderText = genderRecord;
            response.NationalityName = countryRecord.Where(x => x.Id == record.NationalityId).Select(x => x.Name).FirstOrDefault();
            response.StateProvinceName = stateProvinceRecord.Name;
            response.CountryName = countryRecord.Where(x => x.Id == record.CountryId).Select(x => x.Name).FirstOrDefault();
            response.InterestHobbies = individualInterestRecord;

            return response;
        }

        public IndividualProfileDTO GetIndividualProfileByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var query = _IndividualProfileRepository.Table;
            var record = query.Where(x => x.CustomerId == customerId && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var titleRecord = ((IndividualTitle)record.Title).GetDescription();
            var genderRecord = ((Gender)record.Gender).GetDescription();
            var cityRecord = _CityRepository.Table.Where(x => x.Id == record.CityId).FirstOrDefault();
            var stateProvinceRecord = _StateProvinceRepository.Table.Where(x => x.Id == record.StateProvinceId).FirstOrDefault();
            var countryRecord = _CountryRepository.Table.ToList();
            var individualInterestQuery = from b in _InterestHobbyRepository.Table
                                          join u in _IndividualInterestRepository.Table on b.Id equals u.InterestHobbyId
                                          where u.CustomerId == customerId
                                          select b;
            var individualInterestRecord = individualInterestQuery.Select(x => new InterestHobbyDTO { Id = x.Id, Name = x.Name }).ToList();

            var response = _mapper.Map<IndividualProfileDTO>(record);

            response.TitleName = titleRecord;
            response.GenderText = genderRecord;
            response.NationalityName = countryRecord.Where(x => x.Id == record.NationalityId).Select(x => x.Name).FirstOrDefault();
            response.CityName = cityRecord != null ? cityRecord.Name : string.Empty;
            response.StateProvinceName = stateProvinceRecord.Name;
            response.CountryName = countryRecord.Where(x => x.Id == record.CountryId).Select(x => x.Name).FirstOrDefault();
            response.InterestHobbies = individualInterestRecord;
            response.BillingAddress = _billingAddressService.GetBillingAddressByCustomerId(customerId);
            return response;
        }

        public virtual void ResetNumberOfCancellation(int actorId, IndividualProfileDTO individual)
        {
            var request = _mapper.Map<IndividualProfile>(individual);

            individual.NumberOfCancellation = 0;
            request.UpdateAudit(actorId);
            UpdateIndividualProfile(actorId, individual);
        }

        #endregion
    }
}
