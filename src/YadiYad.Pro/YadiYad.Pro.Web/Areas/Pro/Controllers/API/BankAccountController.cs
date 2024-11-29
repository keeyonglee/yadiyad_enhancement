using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Web.Infrastructure;
using Nop.Services.Media;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Enums;
using YadiYad.Pro.Web.Filters;
using Nop.Services.Security;
using Nop.Services.Directory;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Services.DTO.BankAccount;
using Nop.Services.Customers;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{

    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class BankAccountController : BaseController
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly BankAccountService _bankAccountserviceService;

        #endregion

        #region Ctor

        public BankAccountController(
            IMapper mapper,
            IWorkContext workContext,
            BankAccountService bankAccountserviceService)
        {
            _mapper = mapper;
            _workContext = workContext;
            _bankAccountserviceService = bankAccountserviceService;
        }

        #endregion

        [AuthorizeAccess()]
        [HttpGet()]
        public virtual IActionResult GetBankAccount()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var result = _bankAccountserviceService.GetBankAccountByCustomerId(customerId);

            response.SetResponse(result);
            return Ok(response);
        }

        [AuthorizeAccess()]
        [HttpPost()]
        public IActionResult SubmitBankAccount([FromBody]BankAccountDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            dto.CustomerId = _workContext.CurrentCustomer.Id;
            dto.CreatedById = _workContext.CurrentCustomer.Id;
            dto.CreatedOnUTC = DateTime.UtcNow;
            _bankAccountserviceService.CreateBankAccount(_workContext.CurrentCustomer.Id, dto);

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.AccessAdminPanel))]
        [HttpGet("{id}")]
        public virtual IActionResult GetBankAccountById(int id)
        {
            if (id == 0)
            {
                return Problem(
                    statusCode: 400,
                    detail: "Id is required",
                    title: "Error");
            }

            var response = new ResponseDTO();
            var result = _bankAccountserviceService.GetBankAccountById(id);

            response.SetResponse(result);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.AccessAdminPanel))]
        [HttpPost("list")]
        public virtual IActionResult GetBankAccounts([FromBody] ListFilterDTO<BankAccountSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();

            var dto = _bankAccountserviceService.SearchBankAccounts(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.AdvancedFilter);

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.AccessAdminPanel))]
        [HttpPost("approve/{id}")]
        public IActionResult ApproveBankAccount(int id)
        {
            if (id == 0)
            {
                return Problem(
                    statusCode: 400,
                    detail: "Id is required",
                    title: "Error");
            }

            var response = new ResponseDTO();
            var existing = _bankAccountserviceService.GetBankAccountById(id);

            if(existing == null)
            {
                return Problem(
                    statusCode: 400,
                    detail: "Record not found",
                    title: "Error");
            }
            if (existing.IsVerified.HasValue && existing.IsVerified.Value)
            {
                return Problem(
                    statusCode: 400,
                    detail: "Bank account already verified",
                    title: "Error");
            }

            existing.IsVerified = true;
            _bankAccountserviceService.UpdateBankAccountById(_workContext.CurrentCustomer.Id, existing);

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

    }
}
