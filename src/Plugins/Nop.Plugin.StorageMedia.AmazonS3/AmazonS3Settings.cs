using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.StorageMedia.AmazonS3
{
    public class AmazonS3Settings : ISettings
    {
        public string BucketName { get; set; }
        public string PrivateBucketName { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
        public string S3BaseUrl { get; set; }
        public int PreSignedMinutes { get; set; }
    }
}
