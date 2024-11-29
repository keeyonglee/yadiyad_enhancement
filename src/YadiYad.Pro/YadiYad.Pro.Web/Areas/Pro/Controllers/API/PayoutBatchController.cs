using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.DataTables;
using static YadiYad.Pro.Web.Models.DataTables.DataTableAjaxPostModel;
using Nop.Services.Localization;
using YadiYad.Pro.Core.Domain.Payout;
using Nop.Core.Infrastructure;
using Nop.Core.Caching;
using Nop.Services.Logging;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class PayoutBatchController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly PayoutBatchService _payoutBatchService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        public PayoutBatchController(
            IWorkContext workContext,
            ILocalizationService localizationService,
            PayoutBatchService payoutBatchService,
            ILogger logger)
        {
            _workContext = workContext;
            _localizationService = localizationService;
            _payoutBatchService = payoutBatchService;
            _logger = logger;
        }

        #endregion


        /// <summary>
        /// Initialize and execute task
        /// </summary>
        private void ExecuteTask(object[] parameter)
        {
            var response = parameter[0] as ResponseDTO;

            var customerId = _workContext.CurrentCustomer.Id;
            var status = _payoutBatchService.CreatePayoutBatch(customerId);
            var statusShuq = _payoutBatchService.CreatePayoutBatchShuq(customerId);

            if(status == PayoutBatchGenerationStatus.InProcess
                || statusShuq == PayoutBatchGenerationStatus.InProcess)
            {
                response.SetResponse(ResponseStatusCode.Warning,
                    _localizationService.GetResource("Admin.PayoutBatch.Status.InProcess"));
            }
            else if(status == PayoutBatchGenerationStatus.Success
                || statusShuq == PayoutBatchGenerationStatus.Success)
            {
                response.SetResponse(ResponseStatusCode.Success);
            }
            else
            {
                response.SetResponse(ResponseStatusCode.Warning,
                    _localizationService.GetResource("Admin.PayoutBatch.Status.NoItemToProcess"));
            }
        }

        [HttpPost("generate")]
        public IActionResult Generate(
            DataTableRequestModel<PayoutBatchFilterDTO> model)
        {
            var response = new ResponseDTO();
            //execute task with lock
            var locker = EngineContext.Current.Resolve<ILocker>();

            try
            {
                var status = locker.PerformActionWithLock("CreatePayoutBatch", new TimeSpan(0, 5, 0), ExecuteTask, response);

                if (!status)
                {
                    response.SetResponse(ResponseStatusCode.Warning,
                        _localizationService.GetResource("Admin.PayoutBatch.Status.InProcess"));
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString());

                response.SetResponse(ResponseStatusCode.Warning,
                        _localizationService.GetResource("Admin.PayoutBatch.Status.Exception"));
            }

            return Ok(response);
        }
        //[HttpPost("list")]
        //public IActionResult GetBatchList(
        //    DataTableRequestModel<PayoutBatchFilterDTO> model)
        //{
        //    var response = new ResponseDTO();
        //    var recordDTO = _payoutBatchService.GetPayoutBatchs(model.Start, model.Length, model.CustomFilter);

        //    var dtResponseDTO = new DataTableResponseModel<PayoutBatchDTO>
        //    {
        //        draw = model.Draw,
        //        recordsTotal = recordDTO.TotalCount,
        //        recordsFiltered = recordDTO.TotalCount,
        //        data = recordDTO.Data.ToList()
        //    };

        //    response.SetResponse(dtResponseDTO);

        //    return Ok(response);
        //}

        [HttpPost("{id}")]
        public IActionResult GetBatchDetails(
            [FromQuery(Name = "Id")] int batchId, 
            DataTableRequestModel<PayoutBatchFilterDTO> model)
        {
            var response = new ResponseDTO();
            var recordDTO = _payoutBatchService.GetPayoutBatchDetails(batchId);

            response.SetResponse(recordDTO);

            return Ok(response);
        }

        //[HttpPost("{id}/groups")]
        //public IActionResult GetGroupsByBatchId(
        //    [FromQuery(Name = "Id")] int batchId, 
        //    DataTableRequestModel<PayoutBatchFilterDTO> model)
        //{
        //    var response = new ResponseDTO();
        //    var recordDTO = _payoutBatchService.GetPayoutGroupsByBatchId(batchId, model.Start, model.Length, model.CustomFilter);

        //    var dtResponseDTO = new DataTableResponseModel<PayoutGroupDTO>
        //    {
        //        draw = model.Draw,
        //        recordsTotal = recordDTO.TotalCount,
        //        recordsFiltered = recordDTO.TotalCount,
        //        data = recordDTO.Data.ToList()
        //    };

        //    response.SetResponse(dtResponseDTO);

        //    return Ok(response);
        //}

        [HttpPost("{id}/groups/${groupId}")]
        public IActionResult GetGroupDetails(
            [FromQuery(Name = "Id")] int batchId,
            [FromQuery(Name = "GroupId")] int groupId,
            DataTableRequestModel<PayoutRequestFilterDTO> model)
        {
            var response = new ResponseDTO();

            var recordDTO = _payoutBatchService.GetPayoutGroupsDetails(groupId);

            response.SetResponse(recordDTO);

            return Ok(response);
        }
        //[HttpPost("{id}/groups/${groupId}/requests")]
        //public IActionResult GetRequestsByGroupId(
        //    [FromQuery(Name = "Id")] int batchId,
        //    [FromQuery(Name = "GroupId")] int groupId,
        //    DataTableRequestModel<PayoutRequestFilterDTO> model)
        //{
        //    var response = new ResponseDTO();
        //    var recordDTO = _payoutBatchService.GetRequestByGroupId(groupId, model.Start, model.Length);

        //    var dtResponseDTO = new DataTableResponseModel<PayoutAndGroupDTO>
        //    {
        //        draw = model.Draw,
        //        recordsTotal = recordDTO.TotalCount,
        //        recordsFiltered = recordDTO.TotalCount,
        //        data = recordDTO.ToList()
        //    };

        //    response.SetResponse(dtResponseDTO);

        //    return Ok(response);
        //}
    }
}
