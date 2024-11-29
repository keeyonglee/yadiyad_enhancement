using System;

namespace Nop.Plugin.NopStation.WebApi.Models.ShoppingCart
{
    public class UploadFileModel
    {
        public string DownloadUrl { get; set; }

        public Guid DownloadGuid { get; set; }
    }
}
