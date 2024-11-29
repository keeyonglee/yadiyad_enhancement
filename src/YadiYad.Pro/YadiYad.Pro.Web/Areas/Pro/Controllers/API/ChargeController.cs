using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Subscription;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Infrastructure;


namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class ChargeController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ChargeService _chargeService;
        private readonly AccountContext _accountContext;


        #endregion

        #region Ctor

        public ChargeController(
            IWorkContext workContext,
            ChargeService chargeService,
            AccountContext accountContext)
        {
            _workContext = workContext;
            _chargeService = chargeService;
            _accountContext = accountContext;
        }

        #endregion

        [HttpPost("{id}")]
        public IActionResult SubmitCharge(int id, [FromBody] ChargeDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                var actorId = _workContext.CurrentCustomer.Id;
                var createdOrder = _chargeService.CreateCharge(actorId, actorId, dto);
                response.SetResponse(createdOrder);
            }
            else
            {
                // update charge
                var actorId = _workContext.CurrentCustomer.Id;
                var existing = _chargeService.GetById(id);
                dto.Id = id;
                dto.ProductTypeId = existing.ProductTypeId;
                dto.SubProductTypeId = existing.SubProductTypeId;
                var updatedDTO = _chargeService.UpdateCharge(actorId, dto);
                response.SetResponse(updatedDTO);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public virtual IActionResult Get(int id)
        {
            var response = new ResponseDTO();
            var dto = _chargeService.GetById(id);
            response.SetResponse(dto);

            return Ok(response);
        }

        [HttpPost("list")]
        public IActionResult SearchCharges([FromBody] ListFilterDTO<ChargeSearchFilterDTO> searchDTO)
        {
            searchDTO.RecordSize = 10;

            var response = new ResponseDTO();

            var dto = _chargeService.SearchCharges(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.AdvancedFilter);

            response.SetResponse(dto);

            return Ok(response);
        }
    }
}
