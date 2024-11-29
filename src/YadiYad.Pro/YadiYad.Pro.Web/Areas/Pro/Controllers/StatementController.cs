using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Factories;
using YadiYad.Pro.Web.Models.Customer;

using Wkhtmltopdf.NetCore;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Deposit;
using System.Linq;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Order;
using System.IO;
using System.Runtime.InteropServices;
using YadiYad.Pro.Services.Services.Order;
using Nop.Services.Logging;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class StatementController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IGeneratePdf _generatePdf;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly StatementService _statementService;

        public StatementController(
            ILogger logger,
            IGeneratePdf generatePdf,
            IPermissionService permissionService,
            IWorkContext workContext,
            StatementService statementService)
        {
            _logger = logger;
            _generatePdf = generatePdf;
            _statementService = statementService;
            _workContext = workContext;
            _permissionService = permissionService;
        }

        [HttpGet("pro/[controller]/{documentType}/{id}")]
        public IActionResult Details()
        {
            return View();
        }

        [HttpGet("pro/[controller]/Pdf/{documentType}/{id}")]
        public async Task<IActionResult> Pdf(string documentType, int id)
        {
            var dto = new StatementDTO();
            if (documentType.ToLower() == StatementType.Invoice.GetDescription().ToLower())
            {
                dto = _statementService.GetInvoiceStatement(id);

            }
            else if (documentType.ToLower() == StatementType.Deposit.GetDescription().ToLower())
            {
                dto = _statementService.GetDepositRequestStatement(id);
            }
            else if (documentType.ToLower() == StatementType.Refund.GetDescription().ToLower())
            {
                dto = _statementService.GetRefundStatement(id);
            }

            if(dto == null)
            {
                return NotFound();
            }

            if(_permissionService.Authorize(StandardPermissionProvider.ManageAcl) == false)
            {
                if(_workContext.CurrentCustomer.Id != dto.StatementTo
                    && _workContext.CurrentCustomer.Id != dto.StatementFrom)
                {
                    return Unauthorized();
                }

                if (dto.StatementTo == 0)
                {
                    return Unauthorized();
                }
            }

            foreach (var item in dto.StatementItems)
            {
                item.ItemNames = item.ItemName.Split("\r\n").ToList();
                item.Price = item.Quantity * item.UnitPrice;
            }

            string view = $"Areas/Pro/Views/Statement/Pdf.cshtml";

            return await _generatePdf.GetPdf<StatementDTO>(view, dto);
        }


        [HttpGet("pro/[controller]/Pdf/escrow/{id}")]
        public async Task<IActionResult> EscrowInvoice(int id)
        {
            var dto = new StatementDTO();
            dto = _statementService.GetEscrowProcessingInvoiceStatement(id);

            if (dto == null)
            {
                return NotFound();
            }

            if (_permissionService.Authorize(StandardPermissionProvider.ManageAcl) == false)
            {
                if (_workContext.CurrentCustomer.Id != dto.StatementTo
                    && _workContext.CurrentCustomer.Id != dto.StatementFrom)
                {
                    return Unauthorized();
                }

                if (dto.StatementTo == 0)
                {
                    return Unauthorized();
                }
            }

            foreach (var item in dto.StatementItems)
            {
                item.ItemNames = item.ItemName.Split("\r\n").ToList();
                item.Price = item.Quantity * item.UnitPrice;
            }

            string view = $"Areas/Pro/Views/Statement/Pdf.cshtml";

            return await _generatePdf.GetPdf<StatementDTO>(view, dto);
        }
    }
}
