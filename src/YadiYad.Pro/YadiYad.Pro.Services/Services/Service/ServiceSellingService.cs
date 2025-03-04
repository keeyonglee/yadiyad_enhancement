using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Nop.Data;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Service
{
    public class ServiceSellingService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<ServiceApplication> _serviceApplicationRepository;
        private readonly IRepository<ServiceProfile> _serviceProfileRepository;
        #endregion

        #region Ctor
        public ServiceSellingService(
            IMapper mapper,
            IRepository<ServiceApplication> serviceApplicationRepository,
            IRepository<ServiceProfile> serviceProfileRepository)
        {
            _mapper = mapper;
            _serviceApplicationRepository = serviceApplicationRepository;
            _serviceProfileRepository = serviceProfileRepository;
        }
        #endregion

        #region Methods
        public virtual void CreateServiceProfile(ServiceProfileDTO dto)
        {
            var profile = _mapper.Map<ServiceProfile>(dto);
            profile.CreatedOnUTC = DateTime.UtcNow;
            _serviceProfileRepository.Insert(profile);
        }

        public virtual void UpdateServiceProfile(int profileId, ServiceProfileDTO dto)
        {
            var profile = _serviceProfileRepository.GetById(profileId);
            if (profile == null)
                throw new Exception("Service profile not found");

            _mapper.Map(dto, profile);
            _serviceProfileRepository.Update(profile);
        }

        public virtual void DeleteServiceProfile(int profileId)
        {
            var profile = _serviceProfileRepository.GetById(profileId);
            if (profile == null)
                throw new Exception("Service profile not found");

            _serviceProfileRepository.Delete(profile);
        }

        public virtual List<ServiceProfileDTO> SearchServiceSellers(string keyword)
        {
            var profiles = _serviceProfileRepository.Table
                .Where(x => !x.Deleted && (x.Position.Contains(keyword) || x.Company.Contains(keyword)))
                .ToList();

            return _mapper.Map<List<ServiceProfileDTO>>(profiles);
        }

        public virtual void HireServiceSeller(int actorId, int customerId, ServiceApplicationDTO dto)
        {
            var profile = _serviceProfileRepository.Table.FirstOrDefault(x => x.Id == dto.ServiceProfileId && !x.Deleted);
            if (profile == null)
                throw new Exception("Service profile not found");

            dto.ServiceProfileServiceTypeId = profile.ServiceTypeId;
            dto.ServiceProfileServiceModelId = profile.ServiceModelId;
            dto.ServiceProfileServiceFee = profile.ServiceFee;
            dto.ServiceProfileOnsiteFee = profile.OnsiteFee ?? 0;

            var request = _mapper.Map<ServiceApplication>(dto);
            request.Status = (int)ServiceApplicationStatus.New;
            request.CustomerId = customerId;
            request.CreatedById = actorId;
            request.CreatedOnUTC = DateTime.UtcNow;

            _serviceApplicationRepository.Insert(request);
        }

        public virtual void RehireServiceSeller(int applicationId, int newSellerId, int actorId)
        {
            var existingApplication = _serviceApplicationRepository.GetById(applicationId);
            if (existingApplication == null)
                throw new Exception("Existing service application not found");

            if (existingApplication.Status != 6 && existingApplication.Status != 7)
                throw new Exception("Rehire only allowed for cancelled applications");

            var newApplication = new ServiceApplication
            {
                ServiceProfileId = newSellerId,
                CustomerId = existingApplication.CustomerId,
                CreatedById = actorId,
                CreatedOnUTC = DateTime.UtcNow,
                Status = (int)ServiceApplicationStatus.New
            };

            _serviceApplicationRepository.Insert(newApplication);
        }
        #endregion
    }
}
