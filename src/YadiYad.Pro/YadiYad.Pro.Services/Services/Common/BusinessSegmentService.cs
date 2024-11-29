using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Services.Common
{
    public class BusinessSegmentService
    {
        #region Fields
        private readonly IRepository<BusinessSegment> _BusinessSegmentRepository;

        #endregion

        #region Ctor

        public BusinessSegmentService
            (IRepository<BusinessSegment> BusinessSegmentRepository)
        {
            _BusinessSegmentRepository = BusinessSegmentRepository;
        }

        #endregion


        #region Methods

        public virtual IPagedList<BusinessSegment> GetAllBusinessSegments(
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _BusinessSegmentRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            query = query.OrderBy(n => n.Name);

            var news = new PagedList<BusinessSegment>(query, pageIndex, pageSize);

            return news;
        }

        #endregion
    }
}
