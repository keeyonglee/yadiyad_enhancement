using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Services.Common
{
    public class InterestHobbyService
    {
        #region Fields
        private readonly IRepository<InterestHobby> _InterestHobbyRepository;

        #endregion

        #region Ctor

        public InterestHobbyService
            (IRepository<InterestHobby> InterestHobbyRepository)
        {
            _InterestHobbyRepository = InterestHobbyRepository;
        }

        #endregion


        #region Methods

        public virtual IPagedList<InterestHobby> GetAllInterestHobbies(
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _InterestHobbyRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            query = query.OrderBy(n => n.Name);

            var news = new PagedList<InterestHobby>(query, pageIndex, pageSize);

            return news;
        }

        #endregion
    }
}
