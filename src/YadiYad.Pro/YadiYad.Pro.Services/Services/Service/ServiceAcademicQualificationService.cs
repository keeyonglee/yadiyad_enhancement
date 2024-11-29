using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Services.Service
{
    public class ServiceAcademicQualificationService
    {
        #region Fields
        private readonly IRepository<ServiceAcademicQualification> _ServiceAcademicQualificationRepository;

        #endregion

        #region Ctor

        public ServiceAcademicQualificationService
            (IRepository<ServiceAcademicQualification> ServiceAcademicQualificationRepository)
        {
            _ServiceAcademicQualificationRepository = ServiceAcademicQualificationRepository;
        }

        #endregion


        #region Methods
        #endregion
    }
}
