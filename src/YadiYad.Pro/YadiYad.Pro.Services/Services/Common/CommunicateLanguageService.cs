using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Services.Services.Common
{
    public class CommunicateLanguageService
    {
        #region Fields
        private readonly IRepository<CommunicateLanguage> _CommunicateLanguageRepository;

        #endregion

        #region Ctor

        public CommunicateLanguageService
            (IRepository<CommunicateLanguage> CommunicateLanguageRepository)
        {
            _CommunicateLanguageRepository = CommunicateLanguageRepository;
        }

        #endregion


        #region Methods

        public virtual IPagedList<CommunicateLanguage> GetCommunicateLanguage(
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _CommunicateLanguageRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            query = query.OrderBy(n => n.DisplayOrder);

            var news = new PagedList<CommunicateLanguage>(query, pageIndex, pageSize);

            return news;
        }

        #endregion
    }
}
