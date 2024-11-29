using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.StorageMedia.AmazonS3.Models
{
    public class UploadFileRequest
    {
        public string FileName { get; set; }
        public string FolderPath { get; set; }
        public IFormFile FormFile { get; set; }

        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string Extension { get; set; }

        // Upload success and delete existing file
        public string DeleteFileUrl { get; set; }
    }
}
