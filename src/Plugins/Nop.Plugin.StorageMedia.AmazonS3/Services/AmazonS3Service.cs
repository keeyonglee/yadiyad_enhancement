using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Nop.Plugin.StorageMedia.AmazonS3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model.Internal.MarshallTransformations;
using MySqlX.XDevAPI;

namespace Nop.Plugin.StorageMedia.AmazonS3.Services
{
    public class AmazonS3Service
    {
        private readonly AmazonS3Settings _awsS3Config;
        public AmazonS3Service(AmazonS3Settings awsS3Config)
        {
            _awsS3Config = awsS3Config;

        }

        private static RegionEndpoint bucketRegion = RegionEndpoint.APSoutheast1;

        public async Task<UploadFileModelResponse> UploadObjectAsync(
            UploadFileRequest file, bool isPrivateContent)
        {
            try
            {
                var bucketName = isPrivateContent ? _awsS3Config.PrivateBucketName : _awsS3Config.BucketName;

                if (bucketName == null || _awsS3Config.AccessKey == null || _awsS3Config.AccessSecret == null)
                {
                    return new UploadFileModelResponse
                    {
                        Success = false,
                        FileName = file.FileName,
                        Bucket = bucketName,
                        FullPath = file.FolderPath + file.FileName
                    };
                }

                // connecting to the client
                var client = CreateClient();

                byte[] fileBytes = null;
                string contentType = null;
                string fileName = null;
                string ext = null;

                //store file by bytes array
                if (file.FileBytes != null)
                {
                    contentType = file.ContentType;
                    fileBytes = file.FileBytes;
                    fileName = file.FileName;
                    ext = file.Extension;
                }

                //store file by IFormFile
                else if(file.FormFile != null)
                {
                    contentType = file.FormFile.ContentType;
                    fileBytes = new Byte[file.FormFile.Length];
                    file.FormFile.OpenReadStream().Read(fileBytes, 0, Int32.Parse(file.FormFile.Length.ToString()));

                    if (string.IsNullOrEmpty(file.FileName))
                    {
                        fileName = Path.GetFileNameWithoutExtension(file.FormFile.FileName);
                    }
                    else
                    {
                        fileName = file.FileName;
                    }

                }

                fileName = file.FolderPath + fileName + ext;

                PutObjectResponse response = null;

                using (var stream = new MemoryStream(fileBytes))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        InputStream = stream,
                        ContentType = contentType,
                        CannedACL = S3CannedACL.BucketOwnerRead
                    };

                    response = await client.PutObjectAsync(request);
                };

                if (!string.IsNullOrEmpty(file.DeleteFileUrl))
                {
                    DeleteFileRequest deleteRequest = new DeleteFileRequest()
                    {
                        FileUrl = file.DeleteFileUrl
                    };
                    await RemoveObjectAsync(deleteRequest, isPrivateContent);
                }

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // this model is up to you, in my case I have to use it following;
                    return new UploadFileModelResponse
                    {
                        Success = true,
                        FileName = file.FileName,
                        Bucket = bucketName,
                        FullPath = fileName,
                        Size = fileBytes.Length,
                        MimeType = contentType
                    };
                }
                else
                {
                    // this model is up to you, in my case I have to use it following;
                    return new UploadFileModelResponse
                    {
                        Success = false,
                        FileName = file.FileName,
                        Bucket = bucketName,
                        FullPath = fileName
                    };
                }
            }
            catch (Exception e)
            {
                return new UploadFileModelResponse
                {
                    Success = false,
                    FileName = e.ToString()
                };
            }
        }

        public async Task<UploadFileModelResponse> CreateDirectoryAsync(string path, bool isPrivateContent)
        {
            UploadFileModelResponse response = null;

            try
            {
                var bucketName = isPrivateContent ? _awsS3Config.PrivateBucketName : _awsS3Config.BucketName;

                if (bucketName == null || _awsS3Config.AccessKey == null || _awsS3Config.AccessSecret == null)
                {
                    return response;
                }
                else
                {
                    var putObjectRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = path
                    };
                    var client = CreateClient();
                    await client.PutObjectAsync(putObjectRequest);
                    return new UploadFileModelResponse
                    {
                        Success = true,
                        Bucket = bucketName,
                        FullPath = path
                    };
                }

            }
            catch (Exception e)
            {
                return new UploadFileModelResponse
                {
                    Success = false,
                    FileName = e.ToString()
                };
            }
        }

        private AmazonS3Client CreateClient()
        {
            return new AmazonS3Client(_awsS3Config.AccessKey, _awsS3Config.AccessSecret, bucketRegion);
        }

        public async Task<bool> IsFileExistsAsync(string bucket, string filename)
        {
            try
            {
                var client = CreateClient();
                var res = await client.GetObjectAsync(bucket, filename);
                return (res != null ? true : false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when getting an object", ex.Message);
                return false;
            }
        }

        public async Task<DeleteFileResponse> RemoveObjectAsync(DeleteFileRequest deleteFileRequest, bool isPrivateContent)
        {
            var bucketName = isPrivateContent ? _awsS3Config.PrivateBucketName : _awsS3Config.BucketName;

            if (await IsFileExistsAsync(bucketName, deleteFileRequest.FileUrl) == true)
            {
                var client = CreateClient();

                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = deleteFileRequest.FileUrl
                };

                try
                {
                    var response = await client.DeleteObjectAsync(request);
                    return new DeleteFileResponse
                    {
                        Success = true,
                        FileName = deleteFileRequest.FileUrl,
                        Bucket = bucketName
                    };
                }
                catch(Exception ex)
                {
                    return new DeleteFileResponse
                    {
                        Success = false,
                        FileName = deleteFileRequest.FileUrl,
                        Bucket = bucketName
                    };
                }
            }
            else
            {
                return new DeleteFileResponse
                {
                    Success = false,
                    FileName = deleteFileRequest.FileUrl,
                    Bucket = bucketName
                };
            }
            /*bug //ckf fix later
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new DeleteFileResponse
                {
                    Success = true,
                    FileName = deleteFileRequest.FileName,
                    Bucket = deleteFileRequest.Bucket
                };
            }
            else
            {
                return new DeleteFileResponse
                {
                    Success = false,
                    FileName = deleteFileRequest.FileName,
                    Bucket = deleteFileRequest.Bucket
                };
            }*/
        }

        public static string ConstructDirectory(string directory, string fileType, string uniqueKey = null)
        {
            return directory.Replace("{0}", fileType).Replace("{1}", uniqueKey);
        }

        public static string ConstructDirectory2(string directory, string fileType, string uniqueKey = null, string uniqueKey2 = null)
        {
            return directory.Replace("{0}", fileType).Replace("{1}", uniqueKey).Replace("{2}", uniqueKey2);
        }

        public string GetPreSignedUrl(string fileUrl, bool isPrivateContent)
        {
            string urlString = "";
            try
            {
                if (!isPrivateContent)
                {
                    if(!string.IsNullOrWhiteSpace(_awsS3Config.S3BaseUrl))
                    {
                        urlString = $"{_awsS3Config.S3BaseUrl.TrimEnd('/')}/{fileUrl.TrimStart('/')}";
                    }
                }
                
                if(string.IsNullOrWhiteSpace(urlString))
                {
                    var bucketName = isPrivateContent ? _awsS3Config.PrivateBucketName : _awsS3Config.BucketName;
                    var client = CreateClient();

                    var request = new GetPreSignedUrlRequest
                    {
                        BucketName = bucketName,
                        Key = fileUrl,
                        Expires = DateTime.UtcNow.AddMinutes(_awsS3Config.PreSignedMinutes)
                    };
                    urlString = client.GetPreSignedURL(request);
                }
                
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return urlString;
        }

        public async Task<byte[]> GetObjectDataAsync(string fileUrl, bool isPrivateContent)
        {
            var client = CreateClient();

            var bucketName = isPrivateContent ? _awsS3Config.PrivateBucketName : _awsS3Config.BucketName;

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileUrl
            };

            GetObjectResponse response = null;

            try
            {
                response = await client.GetObjectAsync(request);
            }
            catch(Exception ex)
            {
                return null;
            }

            MemoryStream memoryStream = new MemoryStream();

            using (Stream responseStream = response.ResponseStream)
            {
                responseStream.CopyTo(memoryStream);
            }

            return memoryStream.ToArray();
        }

        public async Task GetObjectDataStreamAsync(MemoryStream memoryStream, string fileUrl, bool isPrivateContent)
        {
            var bucketName = isPrivateContent ? _awsS3Config.PrivateBucketName : _awsS3Config.BucketName;
            var client = CreateClient();

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileUrl
            };

            GetObjectResponse response = await client.GetObjectAsync(request);
            using (Stream responseStream = response.ResponseStream)
            {
                await responseStream.CopyToAsync(memoryStream);
            }
        }

        public byte[] ReadStream(Stream responseStream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                responseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public async Task<List<S3Object>> GetObjectsAsync(string path = "", bool isPrivateContent = false)
        {
            var bucket = isPrivateContent ? _awsS3Config.PrivateBucketName : _awsS3Config.BucketName;
            
            var listRequest = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = path
            };

            return await GetObjectsAsync(listRequest).ConfigureAwait(false);
        }

        public async Task<List<S3Object>> GetObjectsAsync(ListObjectsV2Request request)
        {
            var client = CreateClient();

            ListObjectsV2Response result = null;
            var s3Objects = new List<S3Object>();

            do
            {
                request.ContinuationToken = result?.NextContinuationToken;
                result = await client.ListObjectsV2Async(request).ConfigureAwait(false);

                if ((result?.S3Objects?.Count ?? 0) > 0)
                    s3Objects.AddRange(result.S3Objects);

            } while (result?.KeyCount == request.MaxKeys && result.NextContinuationToken != null);
                
            return s3Objects;
        }
        
    }
    
    public class S3File
    {
        public string Key { get; set; }
        public long Size { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
