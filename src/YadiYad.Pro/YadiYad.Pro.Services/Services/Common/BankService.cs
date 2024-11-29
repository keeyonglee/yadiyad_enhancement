using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.Common
{
    public class BankService
    {
        #region Fields
        private readonly IRepository<Bank> _bankRepository;

        #endregion

        #region Ctor

        public BankService
            (IRepository<Bank> bankRepository)
        {
            _bankRepository = bankRepository;
        }

        #endregion

        #region Methods

        public virtual IPagedList<Bank> GetAllBank(
            int pageIndex = 0, 
            int pageSize = int.MaxValue, 
            string keyword = null)
        {
            var query = _bankRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            query = query.OrderBy(n => n.Name);

            var data = new PagedList<Bank>(query, pageIndex, pageSize);

            return data;
        }

        public virtual Bank GetBankById(int bankId)
        {
            if (bankId == 0)
                return null;

            return _bankRepository.GetById(bankId);
        }

        #endregion
    }
}
