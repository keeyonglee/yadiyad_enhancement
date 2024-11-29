using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Services.Service
{
    public class ServiceLanguageProficiencyService
    {
        #region Fields
        private readonly IRepository<ServiceLanguageProficiency> _ServiceLanguageProficiencyRepository;

        #endregion

        #region Ctor

        public ServiceLanguageProficiencyService
            (IRepository<ServiceLanguageProficiency> ServiceLanguageProficiencyRepository)
        {
            _ServiceLanguageProficiencyRepository = ServiceLanguageProficiencyRepository;
        }

        #endregion


        #region Methods
        #endregion
    }
}
