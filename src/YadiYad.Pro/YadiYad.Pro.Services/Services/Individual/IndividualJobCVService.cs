using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Nop.Data;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Service
{
    public class JobService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<IndividualProfile> _individualProfileRepository;
        private readonly IRepository<JobApplication> _jobApplicationRepository;
        private readonly IRepository<JobTransactionHistory> _transactionHistoryRepository;
        #endregion

        #region Ctor
        public JobService(
            IMapper mapper,
            IRepository<IndividualProfile> individualProfileRepository,
            IRepository<JobApplication> jobApplicationRepository,
            IRepository<JobTransactionHistory> transactionHistoryRepository)
        {
            _mapper = mapper;
            _individualProfileRepository = individualProfileRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _transactionHistoryRepository = transactionHistoryRepository;
        }
        #endregion

        #region Methods
        public virtual void CreateOrUpdateIndividualProfile(IndividualJobCVDTO dto)
        {
            var existingProfile = _individualProfileRepository.Table.FirstOrDefault(x => x.CustomerId == dto.CustomerId);
            if (existingProfile == null)
            {
                var newProfile = _mapper.Map<IndividualProfile>(dto);
                newProfile.CreatedOnUTC = DateTime.UtcNow;
                _individualProfileRepository.Insert(newProfile);
            }
            else
            {
                _mapper.Map(dto, existingProfile);
                existingProfile.UpdatedOnUTC = DateTime.UtcNow;
                _individualProfileRepository.Update(existingProfile);
            }
        }

        public virtual void ApplyForJob(int customerId, JobApplicationDTO dto)
        {
            var application = _mapper.Map<JobApplication>(dto);
            application.JobSeekerProfileId = customerId;
            application.CreatedOnUTC = DateTime.UtcNow;
            _jobApplicationRepository.Insert(application);
        }

        public virtual void RecordTransactionHistory(int customerId, string description, decimal amount)
        {
            var transaction = new JobTransactionHistory
            {
                CustomerId = customerId,
                Amount = amount,
                CreatedOnUTC = DateTime.UtcNow
            };
            _transactionHistoryRepository.Insert(transaction);
        }
        #endregion
    }
}
