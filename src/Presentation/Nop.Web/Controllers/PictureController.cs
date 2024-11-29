using Microsoft.AspNetCore.Mvc;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Infrastructure;

namespace Nop.Web.Controllers
{
    [CamelCaseResponseFormatter]
    [Route("api/shuq/[controller]")]
    public class PictureController : Controller
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        [HttpPost("asyncUpload")]
        //do not validate request token (XSRF)
        [IgnoreAntiforgeryToken]
        public virtual IActionResult AsyncUpload()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded"
                });
            }

            const string qqFileNameParameter = "qqfilename";

            var qqFileName = Request.Form.ContainsKey(qqFileNameParameter)
                ? Request.Form[qqFileNameParameter].ToString()
                : string.Empty;

            var picture = _pictureService.InsertPicture(httpPostedFile, qqFileName);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                downloadId = picture.Id,
                downloadUrl = _pictureService.GetPictureUrl(ref picture, 100)
            });
        }
    }
}
