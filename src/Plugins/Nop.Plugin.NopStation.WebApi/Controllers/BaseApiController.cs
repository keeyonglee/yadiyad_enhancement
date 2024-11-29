using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.WebApi.Filters;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [TokenAuthorize]
    [DeviceIdAuthorize]
    [SaveIpAddress]
    [SaveLastActivity]
    [NstAuthorize]
    [NopStationApiLicense]
    public class BaseApiController : Controller
    {
        public IActionResult Ok(string message)
        {
            var model = new BaseResponseModel();
            model.Message = message;
            return Ok(model);
        }

        public IActionResult Created(string message)
        {
            var model = new BaseResponseModel();
            model.Message = message;
            return StatusCode(StatusCodes.Status201Created, model);
        }

        public IActionResult BadRequest(string error)
        {
            var model = new BaseResponseModel();
            model.ErrorList.Add(error);
            return BadRequest(model);
        }

        public IActionResult BadRequest(List<string> errors)
        {
            var model = new BaseResponseModel();
            model.ErrorList.AddRange(errors);
            return BadRequest(model);
        }

        public IActionResult Unauthorized(string error)
        {
            var model = new BaseResponseModel();
            model.ErrorList.Add(error);
            return Unauthorized(model);
        }

        public IActionResult NotFound(string error)
        {
            var model = new BaseResponseModel();
            model.ErrorList.Add(error);
            return NotFound(model);
        }

        public IActionResult MethodNotAllowed()
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        public IActionResult LengthRequired()
        {
            return StatusCode(StatusCodes.Status411LengthRequired);
        }

        public IActionResult InternalServerError(string error)
        {
            var model = new BaseResponseModel();
            model.ErrorList.Add(error);
            return StatusCode(StatusCodes.Status500InternalServerError, model);
        }
    }
}
