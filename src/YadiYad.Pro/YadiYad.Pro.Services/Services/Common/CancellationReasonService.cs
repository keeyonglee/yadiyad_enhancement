using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Services.Services.Common
{
    public class CancellationReasonService
    {
        #region Fields
        private readonly IRepository<Reason> _CancellationServiceRepository;

        #endregion

        #region Ctor
        public CancellationReasonService(
            IRepository<Reason> CancellationServiceRepository)
        {
            _CancellationServiceRepository = CancellationServiceRepository;
        }
        #endregion

        #region Methods

        public virtual IPagedList<Reason> GetAllCancellationReason(int engagementType, int party,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _CancellationServiceRepository.Table.Where(x => x.Published == true && x.EngagementType == engagementType && x.Party == party);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            query = query.OrderBy(n => n.Name);

            var reason = new PagedList<Reason>(query, pageIndex, pageSize);

            return reason;
        }

        public virtual Reason GetReasonById(int id)
        {
            if (id == 0)
                return null;

            return _CancellationServiceRepository.GetById(id);
        }

        #endregion
    }
}
