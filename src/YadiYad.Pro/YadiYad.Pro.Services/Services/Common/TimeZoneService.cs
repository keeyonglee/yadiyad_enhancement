using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using TimeZone = YadiYad.Pro.Core.Domain.Common.TimeZone;

namespace YadiYad.Pro.Services.Common
{
    public class TimeZoneService
    {
        #region Fields
        private readonly IRepository<TimeZone> _TimeZoneRepository;

        #endregion

        #region Ctor

        public TimeZoneService
            (IRepository<TimeZone> TimeZoneRepository)
        {
            _TimeZoneRepository = TimeZoneRepository;
        }

        #endregion


        #region Methods

        public virtual IPagedList<TimeZone> GetAllTimeZones(
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _TimeZoneRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            query = query.OrderBy(n => n.Name);

            var news = new PagedList<TimeZone>(query, pageIndex, pageSize);

            return news;
        }

        #endregion
    }
}
