using Amazon.S3.Model;
using Nop.Plugin.StorageMedia.AmazonS3;
using Nop.Plugin.StorageMedia.AmazonS3.Models;
using Nop.Plugin.StorageMedia.AmazonS3.Services;
using Nop.Services.Tests;
using NUnit.Framework;

namespace YadiYad.Pro.Integration.Tests.Services.AWS
{
    public class S3PluginTests : ServiceTest
    {
        private AmazonS3Service _amazonS3Service;
        private AmazonS3Settings _awsSettings;
        [SetUp]
        public void Setup()
        {
            _awsSettings = new AmazonS3Settings
            {
                AccessKey = "",
                AccessSecret = "",
                BucketName = "yadiyad-staging-data",
                PreSignedMinutes = 10,
                PrivateBucketName = "yadiyad-staging-private-data",
                S3BaseUrl = "https://static-stage.yadiyad.com"
            };
            _amazonS3Service = new AmazonS3Service(_awsSettings);
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void CheckPreSignedUrlUseCDN()
        {
            var key = $"/dir1/{_awsSettings.BucketName}/file.jpg";
            var url = _amazonS3Service.GetPreSignedUrl(key, false);
            Assert.True(url.Contains(_awsSettings.S3BaseUrl));
            Assert.True(url.StartsWith(_awsSettings.S3BaseUrl));
            Assert.True(url.Contains(key));
        }
        
        [Test]
        public void CheckPreSignedUrlDoesntUseCDNForPrivateContent()
        {
            var key = $"/dir1/{_awsSettings.PrivateBucketName}/file.jpg";
            var url = _amazonS3Service.GetPreSignedUrl(key, true);
            Assert.True(url.Contains(_awsSettings.PrivateBucketName));
            Assert.True(url.Contains(key));
        }
        
        [Test]
        public void CheckPreSignedUrlDoesntUseCDNForPublicContent_IfCDNUrlEmpty()
        {
            var awsSettings = new AmazonS3Settings
            {
                AccessKey = "",
                AccessSecret = "",
                BucketName = "yadiyad-staging-data",
                PreSignedMinutes = 10,
                PrivateBucketName = "yadiyad-staging-private-data",
                S3BaseUrl = ""
            };
            var amazonS3Service = new AmazonS3Service(awsSettings);
            var key = $"/dir1/file.jpg";
            var url = amazonS3Service.GetPreSignedUrl(key, false);
            Assert.True(url.Contains(awsSettings.BucketName));
            Assert.True(url.Contains(key));
        }

        [Test]
        public void Get_All_Files()
        {
            var files = _amazonS3Service
            .GetObjectsAsync("images/uploaded", false)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
        }

        [Test]
        public void GetAllDirectories()    
        {
            var files = _amazonS3Service
                .GetObjectsAsync("images/uploaded", false)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        [Test]
        public void DeleteFile()
        {
            var path = "/images/uploaded/def/0ebf6cf8_orangeCandy.jpg";
            path = path.TrimStart('/');
            var deleteRequest = new DeleteFileRequest
            {
                FileUrl = path
            };

            var response = _amazonS3Service.RemoveObjectAsync(deleteRequest, false)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}