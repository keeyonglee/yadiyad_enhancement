using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.DTO.Moderator;

namespace YadiYad.Pro.Services.Services.Moderator
{
    public class BlockCustomerService
    {
        #region Fields

        private readonly IRepository<BlockCustomer> _BlockCustomerRepository;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public BlockCustomerService(
            IRepository<BlockCustomer> BlockCustomerRepository,
            IWorkContext workContext)
        {
            _BlockCustomerRepository = BlockCustomerRepository;
            _workContext = workContext;
        }
        #endregion

        #region Methods

        public virtual BlockCustomerDTO GetBlockStatus(int id)
        {
            var model = new BlockCustomerDTO();
            var query = _BlockCustomerRepository.Table.Where(x => x.CustomerId == id && !x.Deleted);

            if (query.Count() == 0)
            {
                model.IsCurrentlyBlock = false;
                model.WasBlockLast90Days = false;
            }
            else
            {
                var last90DaysDate = DateTime.UtcNow.AddDays(-90);
                var queryCheck90Days = query
                    .Where(x => x.EndDate > last90DaysDate || x.StartDate > last90DaysDate)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                var queryCheckCurrentStatus = query
                    .Where(x => x.EndDate > DateTime.UtcNow)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();
                model.IsCurrentlyBlock = queryCheckCurrentStatus != null;
                model.WasBlockLast90Days = queryCheck90Days != null;
                model.BlockQuantity = query.Count();
                if (model.IsCurrentlyBlock)
                {
                    model.EndDate = queryCheckCurrentStatus.EndDate;
                }
            }
            return model;
        }
        
        protected internal virtual IQueryable<int> GetBlockedCustomers(DateTime? atTime = null)
        {
            atTime ??= DateTime.UtcNow;
            return _BlockCustomerRepository.Table
                .Where(q => !q.Deleted
                            && q.EndDate > atTime
                            && q.StartDate < atTime)
                .Select(s => s.CustomerId);
        }

        public virtual void CreateBlockCustomer(int customerId, int reason, string remarks)
        {
            var blockStatus = GetBlockStatus(customerId);
            if (!blockStatus.IsCurrentlyBlock)
            {
                var daysToBlock = CustomerBlockDays(blockStatus.BlockQuantity);
                CreateEntry(customerId, reason, remarks, daysToBlock);
            }
        }

        public virtual void CreateBlockCustomer(int customerId, int reason, string remarks, int blockDays)
        {
            var blockStatus = GetBlockStatus(customerId);
            if (!blockStatus.IsCurrentlyBlock)
            {
                CreateEntry(customerId, reason, remarks, blockDays);
            }
        }

        private void CreateEntry(int customerId, int reason, string remarks, int blockDays)
        {
            var newBlock = new BlockCustomer
            {
                CustomerId = customerId,
                Reason = reason,
                Remarks = remarks,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(blockDays),
                CreatedById = _workContext.CurrentCustomer.Id,
                CreatedOnUTC = DateTime.UtcNow
            };
            _BlockCustomerRepository.Insert(newBlock);
        }

        public virtual int CustomerBlockDays(int currentBlockCount) =>
            currentBlockCount switch
            {
                0 => 15,
                1 => 30,
                _ => 45
            };

        public virtual int GetCustomerNextBlockDuration(int customerId)
        {
            var blockStatus = GetBlockStatus(customerId);
            return CustomerBlockDays(blockStatus.BlockQuantity);
        }

        #endregion
    }
}
