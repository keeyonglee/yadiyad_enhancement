using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Web.Infrastructure;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public partial class PictureController : BaseController
    {
        #region Fields

        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        #endregion

        #region Ctor

        public PictureController(
            IPictureService pictureService,
            MediaSettings mediaSettings)
        {
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        [HttpPost]
        //do not validate request token (XSRF)
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> AsyncUploadAsync()
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
                pictureId = picture.Id,
                imageUrl = _pictureService.GetPictureUrl(ref picture, _mediaSettings.ProductDetailsPictureSize)
            });
        }

        #endregion
    }
}