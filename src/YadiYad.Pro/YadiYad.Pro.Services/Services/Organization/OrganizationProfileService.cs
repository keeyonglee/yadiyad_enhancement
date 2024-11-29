using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Services.DTO.Organization;

namespace YadiYad.Pro.Services.Organization
{
    public class OrganizationProfileService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<OrganizationProfile> _OrganizationProfileRepository;
        private readonly IRepository<BusinessSegment> _BusinessSegmentRepository;
        private readonly IRepository<StateProvince> _StateProvinceRepository;
        private readonly IRepository<Country> _CountryRepository;



        #endregion

        #region Ctor

        public OrganizationProfileService
            (IMapper mapper,
            IRepository<OrganizationProfile> OrganizationProfileRepository, 
            IRepository<BusinessSegment> BusinessSegmentRepository,
            IRepository<StateProvince> StateProvinceRepository,
            IRepository<Country> CountryRepository)
        {
            _mapper = mapper;
            _OrganizationProfileRepository = OrganizationProfileRepository;
            _BusinessSegmentRepository = BusinessSegmentRepository;
            _StateProvinceRepository = StateProvinceRepository;
            _CountryRepository = CountryRepository;
        }

        #endregion


        #region Methods

        public virtual void CreateOrganizationProfile(OrganizationProfileDTO dto)
        {
            var request = _mapper.Map<OrganizationProfile>(dto);
            _OrganizationProfileRepository.Insert(request);
        }

        public virtual void UpdateOrganizationProfile(OrganizationProfileDTO dto)
        {
            var request = _mapper.Map<OrganizationProfile>(dto);
            _OrganizationProfileRepository.Update(request);
        }

        public OrganizationProfileDTO GetOrganizationProfileById(int id)
        {
            if (id == 0)
                return null;

            var query = _OrganizationProfileRepository.Table;
            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var segmentRecord = _BusinessSegmentRepository.Table.Where(x => x.Id == record.SegmentId).FirstOrDefault();
            var companySizeRecord = ((CompanySize)record.CompanySize).GetDescription();
            var stateProvinceRecord = _StateProvinceRepository.Table.Where(x => x.Id == record.StateProvinceId).FirstOrDefault();
            var countryRecord = _CountryRepository.Table.Where(x => x.Id == record.CountryId).FirstOrDefault();
            var ContactPersonTitleRecord = ((IndividualTitle)record.ContactPersonTitle).GetDescription();

            var response = _mapper.Map<OrganizationProfileDTO>(record);
            response.SegmentName = segmentRecord.Name;
            response.CompanySizeName = companySizeRecord;
            response.StateProvinceName = stateProvinceRecord.Name;
            response.CountryName = countryRecord.Name;
            response.ContactPersonTitleName = ContactPersonTitleRecord;

            return response;
        }

        public OrganizationProfileDTO GetOrganizationProfileByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var query = _OrganizationProfileRepository.Table;
            var record = query.Where(x => x.CustomerId == customerId && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var segmentRecord = _BusinessSegmentRepository.Table.Where(x => x.Id == record.SegmentId).FirstOrDefault();
            var companySizeRecord = ((CompanySize)record.CompanySize).GetDescription();
            var stateProvinceRecord = _StateProvinceRepository.Table.Where(x => x.Id == record.StateProvinceId).FirstOrDefault();
            var countryRecord = _CountryRepository.Table.Where(x => x.Id == record.CountryId).FirstOrDefault();
            var ContactPersonTitleRecord = ((IndividualTitle)record.ContactPersonTitle).GetDescription();

            var response = _mapper.Map<OrganizationProfileDTO>(record);
            response.SegmentName = segmentRecord.Name;
            response.CompanySizeName = companySizeRecord;
            response.StateProvinceName = stateProvinceRecord.Name;
            response.CountryName = countryRecord.Name;
            response.ContactPersonTitleName = ContactPersonTitleRecord;

            return response;
        }

        #endregion
    }
}
