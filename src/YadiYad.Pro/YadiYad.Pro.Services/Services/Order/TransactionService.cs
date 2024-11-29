using AutoMapper;
using Nop.Core;
using Nop.Data;
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
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Order
{
    public class TransactionService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<Transaction> _transactionRepository;

        #endregion

        #region Ctor

        public TransactionService
            (IMapper mapper,
            IRepository<Transaction> transactionRepository)
        {
            _mapper = mapper;
            _transactionRepository = transactionRepository;
        }

        #endregion

        #region Methods

        public TransactionDTO CreateTransaction(int actorId, int customerId, TransactionDTO dto)
        {
            var transaction = dto.ToModel(_mapper);

            CreateAudit(transaction, actorId);

            _transactionRepository.Insert(transaction);

            dto.Id = transaction.Id;

            return dto;
        }
        #endregion
    }
}
