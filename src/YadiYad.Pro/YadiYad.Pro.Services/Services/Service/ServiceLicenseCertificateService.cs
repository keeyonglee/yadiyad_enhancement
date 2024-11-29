using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Services.Service
{
    public class ServiceLicenseCertificateService
    {
        #region Fields
        private readonly IRepository<ServiceLicenseCertificate> _ServiceLicenseCertificateRepository;

        #endregion

        #region Ctor

        public ServiceLicenseCertificateService
            (IRepository<ServiceLicenseCertificate> ServiceLicenseCertificateRepository)
        {
            _ServiceLicenseCertificateRepository = ServiceLicenseCertificateRepository;
        }

        #endregion


        #region Methods
        #endregion
    }
}
