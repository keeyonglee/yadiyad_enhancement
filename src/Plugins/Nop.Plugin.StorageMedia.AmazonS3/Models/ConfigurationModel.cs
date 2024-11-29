using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.StorageMedia.AmazonS3.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.StorageMedia.AmazonS3.Fields.BucketName")]
        public string BucketName { get; set; }
        public bool BucketName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.StorageMedia.AmazonS3.Fields.PrivateBucketName")]
        public string PrivateBucketName { get; set; }
        public bool PrivateBucketName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.StorageMedia.AmazonS3.Fields.AccessKey")]
        public string AccessKey { get; set; }
        public bool AccessKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.StorageMedia.AmazonS3.Fields.AccessSecret")]
        public string AccessSecret { get; set; }
        public bool AccessSecret_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.StorageMedia.AmazonS3.Fields.S3BaseUrl")]
        public string S3BaseUrl { get; set; }
        public bool S3BaseUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.StorageMedia.AmazonS3.Fields.PreSignedMinutes")]
        public int PreSignedMinutes { get; set; }
        public bool PreSignedMinutes_OverrideForStore { get; set; }
    }
}
