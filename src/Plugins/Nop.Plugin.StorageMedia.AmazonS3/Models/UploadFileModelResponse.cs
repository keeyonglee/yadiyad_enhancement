using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.StorageMedia.AmazonS3.Models
{
    public class UploadFileModelResponse
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string MimeType { get; set; }
        public decimal? Size { get; set; }
        public string Bucket { get; set; }
        public bool Success { get; set; }
    }
}
