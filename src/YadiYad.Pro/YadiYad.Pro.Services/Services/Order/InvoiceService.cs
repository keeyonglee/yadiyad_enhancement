using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Documents;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Order
{
    public class InvoiceService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<ProInvoice> _proInvoiceRepository;
        private readonly DocumentNumberService _documentNumberService;

        #endregion

        #region Ctor

        public InvoiceService
            (IMapper mapper,
            IRepository<ProInvoice> proInvoiceRepository,
            DocumentNumberService documentNumberService)
        {
            _mapper = mapper;
            _proInvoiceRepository = proInvoiceRepository;
            _documentNumberService = documentNumberService;
        }

        #endregion

        #region Methods

        public string GetNewInvoiceNumber(RunningNumberType runningNumberType, int? customerId = null)
        {
            var docNumbers = _documentNumberService
                .GetDocumentNumbers(runningNumberType, 1, customerId);

            if (docNumbers == null
                || docNumbers.Count == 0)
            {
                throw new NopException("Fail to identify invoice number.");
            }

            var invoiceNumber = docNumbers.First();

            return invoiceNumber;
        }

        public InvoiceDTO CreateInvoice(int actorId, InvoiceDTO dto, RunningNumberType runningNumberType, int? customerId = null)
        {
            var invoiceNumber = GetNewInvoiceNumber(runningNumberType, customerId);

            dto.InvoiceNumber = invoiceNumber;

            var invoice = _mapper.Map<ProInvoice>(dto);

            CreateAudit(invoice, actorId);

            _proInvoiceRepository.Insert(invoice);

            dto.Id = invoice.Id;

            return dto;
        }

        public virtual IPagedList<ProInvoice> GetAll(
          int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _proInvoiceRepository.Table;

            query = query.OrderBy(n => n.Id);

            var data = new PagedList<ProInvoice>(query, pageIndex, pageSize);

            return data;
        }
        #endregion
    }
}
