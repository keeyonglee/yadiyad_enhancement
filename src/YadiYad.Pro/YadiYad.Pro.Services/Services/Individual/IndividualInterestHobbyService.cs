using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Individual;

namespace YadiYad.Pro.Services.Individual
{
    public class IndividualInterestHobbyService
    {
        #region Fields
        private readonly IRepository<IndividualInterestHobby> _IndividualInterestHobbyRepository;

        #endregion

        #region Ctor

        public IndividualInterestHobbyService
            (IRepository<IndividualInterestHobby> IndividualInterestHobbyRepository)
        {
            _IndividualInterestHobbyRepository = IndividualInterestHobbyRepository;
        }

        #endregion


        #region Methods

        public virtual void CreateIndividualInterestHobbies(List<IndividualInterestHobby> dto)
        {
            _IndividualInterestHobbyRepository.Insert(dto);
        }

        public virtual void UpdateIndividualInterestHobbyByCustomerId(int id, List<IndividualInterestHobby> dtos)
        {
            var records = _IndividualInterestHobbyRepository.Table.Where(x => x.CustomerId == id).ToList();
            _IndividualInterestHobbyRepository.Delete(records);
            _IndividualInterestHobbyRepository.Insert(dtos);

        }

        public IndividualInterestHobby GetIndividualInterestHobbyById(int id)
        {
            if (id == 0)
                return null;

            var query = _IndividualInterestHobbyRepository.Table;

            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            return record;
        }

        public IndividualInterestHobby GetIndividualInterestHobbyByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var query = _IndividualInterestHobbyRepository.Table;

            var record = query.Where(x => x.CustomerId == customerId && !x.Deleted).FirstOrDefault();

            return record;
        }

        #endregion
    }
}
