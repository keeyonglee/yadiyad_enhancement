using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Models.Order
{
    public class UploadFileReturnRequestModel
    {
        public string DownloadUrl { get; set; }

        public Guid DownloadGuid { get; set; }
        public int PictureId { get; set; }
    }
}
