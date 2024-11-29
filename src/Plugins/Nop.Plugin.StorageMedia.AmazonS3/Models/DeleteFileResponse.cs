using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.StorageMedia.AmazonS3.Models
{
    public class DeleteFileResponse
    {
        public string FileName { get; set; }
        public string Bucket { get; set; }
        public bool Success { get; set; }
    }
}
