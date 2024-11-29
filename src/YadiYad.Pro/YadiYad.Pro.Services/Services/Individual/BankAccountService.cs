using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Services.DTO.BankAccount;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Individual
{
    public class BankAccountService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SecuritySettings _securitySettings;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<BankAccount> _BankAccountRepository;
        private readonly IRepository<Bank> _BankRepository;
        private readonly IRepository<Download> _DownloadRepository;
        private readonly IRepository<Customer> _Customerrepository;

        #endregion

        #region Ctor

        public BankAccountService
            (IMapper mapper,
            SecuritySettings securitySettings,
            IDataProtectionProvider dataProtectionProvider,
            IEncryptionService encryptionService,
            IRepository<BankAccount> BankAccountRepository,
            IRepository<Bank> BankRepository,
            IRepository<Download> DownloadRepository,
            IRepository<Customer> Customerrepository)
        {
            _mapper = mapper;
            _securitySettings = securitySettings;
            _dataProtectionProvider = dataProtectionProvider;
            _encryptionService = encryptionService;
            _BankAccountRepository = BankAccountRepository;
            _BankRepository = BankRepository;
            _DownloadRepository = DownloadRepository;
            _Customerrepository = Customerrepository;

        }

        #endregion

        #region Methods

        public BankAccountDTO CreateBankAccount(int actorId, BankAccountDTO dto)
        {
            var request = _mapper.Map<BankAccount>(dto);

            CreateAudit(request, actorId);
            request = EncryptSensitiveInfo(request);

            _BankAccountRepository.Insert(request);

            dto.Id = request.Id;

            // invalidate old bankAcc
            var oldBankAccs = _BankAccountRepository.Table
                .Where(x => x.Id != dto.Id && !x.Deleted && x.CustomerId == actorId)
                .ToList()
                .Select(x =>
                {
                    x.Deleted = true;
                    UpdateAudit(x, x, actorId);
                    return x;
                });
            _BankAccountRepository.Update(oldBankAccs);

            return dto;
        }

        public BankAccountDTO UpdateBankAccountById(int actorId, BankAccountDTO dto)
        {
            var bankAcc = _BankAccountRepository.Table
                .Where(x => x.Id == dto.Id)
                .FirstOrDefault();

            var request = _mapper.Map<BankAccount>(dto);

            request = EncryptSensitiveInfo(request, bankAcc);

            UpdateAudit(bankAcc, request, actorId);
            _BankAccountRepository.Update(request);

            return dto;
        }

        public BankAccount EncryptSensitiveInfo(BankAccount newBankAccount, BankAccount oldBankAccount = null, string encryptionKey = null)
        {
            var salthKey = oldBankAccount?.SaltKey?? newBankAccount.SaltKey??_encryptionService.CreateSaltKey(10);

            var wholeKey = GetEncyptionWholeKey(salthKey, encryptionKey);

            newBankAccount.SaltKey = salthKey;

            if (string.IsNullOrEmpty(newBankAccount.Identity) == false
                && string.IsNullOrWhiteSpace(wholeKey) == false)
            {
                try
                {
                    var newIdentity = _encryptionService.EncryptText(newBankAccount.Identity, wholeKey);

                    newBankAccount.Identity = newIdentity;
                }
                catch(FormatException)
                {

                }
                catch (CryptographicException)
                {

                }
            }

            return newBankAccount;
        }

        public BankAccount DecryptSensitiveInfo(BankAccount bankAccount, string encryptionKey = null)
        {
            var wholeKey = GetEncyptionWholeKey(bankAccount.SaltKey, encryptionKey);

            if (string.IsNullOrEmpty(bankAccount.Identity) == false
                && string.IsNullOrWhiteSpace(wholeKey) == false)
            {
                try
                {
                    var newIdentity = _encryptionService.DecryptText(bankAccount.Identity, wholeKey);

                    bankAccount.Identity = newIdentity; ;
                }
                catch (FormatException)
                {

                }
                catch (CryptographicException)
                {

                }
            }

            return bankAccount;
        }

        public BankAccountDTO DecryptSensitiveInfo(BankAccountDTO bankAccountDTO, string encryptionKey = null)
        {
            if(bankAccountDTO == null)
            {
                return null;
            }

            var wholeKey = GetEncyptionWholeKey(bankAccountDTO.SaltKey, encryptionKey);

            if (string.IsNullOrWhiteSpace(bankAccountDTO.Identity) == false
                && string.IsNullOrWhiteSpace(wholeKey) == false)
            {
                try
                {
                    var newIdentity = _encryptionService.DecryptText(bankAccountDTO.Identity, wholeKey);

                    bankAccountDTO.Identity = newIdentity;
                }
                catch (FormatException)
                {

                }
                catch (CryptographicException)
                {

                }
            }

            bankAccountDTO.SaltKey = null;

            return bankAccountDTO;
        }

        public BankAccountDTO GetBankAccountById(int id)
        {
            if (id == 0)
                return null;

            var record = _BankAccountRepository.Table
                    .Where(x => !x.Deleted && x.Id == id)
                    .Join(_DownloadRepository.Table,
                        x => x.BankStatementDownloadId,
                        y => y.Id,
                        (x, y) => new { x, y }
                        )
                    .Join(_BankRepository.Table,
                        x => x.x.BankId,
                        z => z.Id,
                        (x, z) => new { x = x.x, y = x.y, z }
                    )
                    .Select(x => new BankAccountDTO
                    {
                        Id = x.x.Id,
                        CustomerId = x.x.CustomerId,
                        BankId = x.x.BankId,
                        AccountNumber = x.x.AccountNumber,
                        AccountHolderName = x.x.AccountHolderName,
                        BankStatementDownloadId = x.x.BankStatementDownloadId,
                        IsVerified = x.x.IsVerified,
                        Status = !x.x.IsVerified.HasValue? "Pending" : x.x.IsVerified.Value ? "Approved" : "Rejected",
                        CreatedById = x.x.CreatedById,
                        UpdatedById = x.x.UpdatedById,
                        CreatedOnUTC = x.x.CreatedOnUTC,
                        UpdatedOnUTC = x.x.UpdatedOnUTC,
                        Comment = x.x.Comment,
                        BankName = x.z.Name,
                        FileName = x.y.Filename,
                        Extension = x.y.Extension,
                        BankStatementDownloadGuid = x.y.DownloadGuid,
                        IdentityType = x.x.IdentityType,
                        IdentityTypeName = ((IdentityType)x.x.IdentityType).GetDescription(),
                        Identity = x.x.Identity,
                        SaltKey = x.x.SaltKey
                    })
                    .FirstOrDefault();

            record = DecryptSensitiveInfo(record);

            return record;
        }

        public BankAccountDTO GetBankAccountByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var record = _BankAccountRepository.Table
                    .Where(x => !x.Deleted && x.CustomerId == customerId)
                    .Join(_DownloadRepository.Table,
                        x => x.BankStatementDownloadId,
                        y => y.Id,
                        (x, y) => new { x, y }
                        )
                    .Join(_BankRepository.Table,
                        x => x.x.BankId,
                        z => z.Id,
                        (x, z) => new { x = x.x, y = x.y, z }
                    )
                    .Select(x => new BankAccountDTO
                    {
                        Id = x.x.Id,
                        CustomerId = x.x.CustomerId,
                        BankId = x.x.BankId,
                        AccountNumber = x.x.AccountNumber,
                        AccountHolderName = x.x.AccountHolderName,
                        BankStatementDownloadId = x.x.BankStatementDownloadId,
                        Comment = x.x.Comment,
                        IsVerified = x.x.IsVerified,
                        Status = !x.x.IsVerified.HasValue ? "Pending" : x.x.IsVerified.Value ? "Approved" : "Invalid",
                        CreatedById = x.x.CreatedById,
                        UpdatedById = x.x.UpdatedById,
                        CreatedOnUTC = x.x.CreatedOnUTC,
                        UpdatedOnUTC = x.x.UpdatedOnUTC,
                        BankName = x.z.Name,
                        FileName = x.y.Filename,
                        Extension = x.y.Extension,
                        BankStatementDownloadGuid = x.y.DownloadGuid,
                        IdentityType = x.x.IdentityType,
                        IdentityTypeName = ((IdentityType)x.x.IdentityType).GetDescription(),
                        Identity = x.x.Identity,
                        SaltKey = x.x.SaltKey
                    })
                    .FirstOrDefault();

            record = DecryptSensitiveInfo(record);

            return record;
        }

        public IPagedList<BankAccountDTO> GetBankAccountsUnverified(int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _BankAccountRepository.Table
                    .Where(x => !x.Deleted && x.IsVerified == null)
                    .Join(_BankRepository.Table,
                        x => x.BankId,
                        z => z.Id,
                        (x, z) => new { x, z }
                    )
                    .Select(x => new BankAccountDTO
                    {
                        Id = x.x.Id,
                        CustomerId = x.x.CustomerId,
                        AccountHolderName = x.x.AccountHolderName,
                        CreatedOnUTC = x.x.CreatedOnUTC,
                        UpdatedOnUTC = x.x.UpdatedOnUTC,
                        Comment = x.x.Comment,
                        BankName = x.z.Name,
                    })
                    .OrderByDescending(x => x.Id);

            return new PagedList<BankAccountDTO>(query, pageIndex, pageSize);

        }

        public string GetEncyptionWholeKey(string saltKey, string encryptionKey = null)
        {
            var dataProtectedEncryptionKey = _securitySettings.DataProtectedEncryptionKey;

            if (string.IsNullOrWhiteSpace(dataProtectedEncryptionKey) == false)
            {
                try
                {
                    var protector = _dataProtectionProvider.CreateProtector("PersonalData");
                    dataProtectedEncryptionKey = protector.Unprotect(dataProtectedEncryptionKey);
                }
                catch (CryptographicException)
                {

                }
            }

            var wholeKey = $"{saltKey.Right(10, '0')}|{encryptionKey?? dataProtectedEncryptionKey}";

            if(wholeKey != null && wholeKey.Length <= 16)
            {
                wholeKey = null;
            }

            return wholeKey;
        }

        public void UpdateEncryptionKey(string oldKey, string newKey)
        {
            var bankAccounts = _BankAccountRepository.Table
                .ToList();

            foreach(var bankAccount in bankAccounts)
            {
                var decryptedBankAccount = DecryptSensitiveInfo(bankAccount, oldKey);

                bankAccount.Identity = decryptedBankAccount.Identity;

                var encryptedBankAccount = EncryptSensitiveInfo(bankAccount, null, newKey);

                bankAccount.Identity = encryptedBankAccount.Identity;
                bankAccount.SaltKey = encryptedBankAccount.SaltKey;
            }

            _BankAccountRepository.Update(bankAccounts);
        }

        public PagedListDTO<BankAccountDTO> SearchBankAccounts(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            BankAccountSearchFilterDTO filterDTO = null)
        {
            var query = _BankAccountRepository.Table
                    .Where(x => !x.Deleted)
                    .Join(_DownloadRepository.Table,
                        x => x.BankStatementDownloadId,
                        y => y.Id,
                        (x, y) => new { x, y }
                        )
                    .Join(_BankRepository.Table,
                        x => x.x.BankId,
                        z => z.Id,
                        (x, z) => new { x = x.x, y = x.y, z }
                    )
                    .Select(x => new BankAccountDTO
                    {
                        Id = x.x.Id,
                        CustomerId = x.x.CustomerId,
                        BankId = x.x.BankId,
                        AccountNumber = x.x.AccountNumber,
                        AccountHolderName = x.x.AccountHolderName,
                        BankStatementDownloadId = x.x.BankStatementDownloadId,
                        IsVerified = x.x.IsVerified,
                        Status = !x.x.IsVerified.HasValue ? "Pending" : x.x.IsVerified.Value ? "Approved" : "Rejected",
                        CreatedById = x.x.CreatedById,
                        UpdatedById = x.x.UpdatedById,
                        CreatedOnUTC = x.x.CreatedOnUTC,
                        UpdatedOnUTC = x.x.UpdatedOnUTC,
                        BankName = x.z.Name,
                        FileName = x.y.Filename,
                        Extension = x.y.Extension,
                        BankStatementDownloadGuid = x.y.DownloadGuid,
                        IdentityType = x.x.IdentityType,
                        IdentityTypeName = ((IdentityType)x.x.IdentityType).GetDescription(),
                        Identity = x.x.Identity,
                    });

            if (filterDTO.IsVerified != null)
            {
                query = query.Where(x => filterDTO.IsVerified == filterDTO.IsVerified);
            }
            if (!String.IsNullOrEmpty(filterDTO.AccountHolderName))
            {
                query = query.Where(x => x.AccountHolderName.Contains(filterDTO.AccountHolderName));
            }

            List<BankAccountDTO> result = new List<BankAccountDTO>();
            result = query.ToList();

            var totalCount = result.Count();
            var records = result.ToList();

            var response = new PagedListDTO<BankAccountDTO>(records, pageIndex, pageSize, totalCount);

            return response;
        }

        public virtual IPagedList<BankAccountDTO> SearchBankAccountsAdmin(string accountHolderName, bool? isVerified,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _BankAccountRepository.Table
                .Where(x => !x.Deleted)
                .Join(_DownloadRepository.Table,
                    x => x.BankStatementDownloadId,
                    y => y.Id,
                    (x, y) => new { x, y }
                    )
                .Join(_BankRepository.Table,
                    x => x.x.BankId,
                    z => z.Id,
                    (x, z) => new { x = x.x, y = x.y, z }
                )
                .Select(x => new BankAccountDTO
                {
                    Id = x.x.Id,
                    CustomerId = x.x.CustomerId,
                    BankId = x.x.BankId,
                    AccountNumber = x.x.AccountNumber,
                    AccountHolderName = x.x.AccountHolderName,
                    BankStatementDownloadId = x.x.BankStatementDownloadId,
                    IsVerified = x.x.IsVerified,
                    Status = !x.x.IsVerified.HasValue ? "Pending" : x.x.IsVerified.Value ? "Approved" : "Invalid",
                    CreatedById = x.x.CreatedById,
                    UpdatedById = x.x.UpdatedById,
                    CreatedOnUTC = x.x.CreatedOnUTC,
                    UpdatedOnUTC = x.x.UpdatedOnUTC,
                    BankName = x.z.Name,
                    FileName = x.y.Filename,
                    Extension = x.y.Extension,
                    BankStatementDownloadGuid = x.y.DownloadGuid,
                    IdentityType = x.x.IdentityType,
                    IdentityTypeName = ((IdentityType)x.x.IdentityType).GetDescription(),
                    Identity = x.x.Identity,
                });

            if (isVerified != null)
            {
                query = query.Where(x => x.IsVerified == isVerified);
            }
            if (!String.IsNullOrEmpty(accountHolderName))
            {
                query = query.Where(x => x.AccountHolderName.Contains(accountHolderName));
            }

            query = query.OrderByDescending(x => x.CreatedOnUTC);

            var data = new PagedList<BankAccountDTO>(query, pageIndex, pageSize);

            return data;
        }


        public Customer GetCustomerByBankAccountId(int bankAccountId)
        {
            var query = from c in _Customerrepository.Table
                        join ba in _BankAccountRepository.Table on c.Id equals ba.CustomerId
                        where ba.Id == bankAccountId && !ba.Deleted
                        select c;
            return query.FirstOrDefault();
        }
        #endregion
    }
}
