using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Media.RoxyFileman;
using System.Text;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Plugin.StorageMedia.AmazonS3.Models;
using Nop.Services.Customers;
using NUglify.Helpers;

namespace Nop.Plugin.StorageMedia.AmazonS3.Services
{
    public class AmazonS3RoxyFileManService : FileRoxyFilemanService, IRoxyFilemanService
    {
        #region Fields

        private readonly AmazonS3Service _amazonS3Service;
        private readonly ICustomerService _customerService;

        // state saved, so cannot use singleton or scoped
        private List<S3Object> S3Objects;

        #endregion

        #region Ctor
        public AmazonS3RoxyFileManService(IWebHostEnvironment webHostEnvironment,
           IHttpContextAccessor httpContextAccessor,
           INopFileProvider fileProvider,
           IWebHelper webHelper,
           IWorkContext workContext,
           MediaSettings mediaSettings,
           AmazonS3Service amazonS3Service,
           AmazonS3Settings amazonS3Settings,
           ICustomerService customerService)
           : base(webHostEnvironment, httpContextAccessor,
               fileProvider, webHelper, workContext, mediaSettings)
        {
            _amazonS3Service = amazonS3Service;
            _customerService = customerService;
            Configure();
        }

        #endregion

        #region Utilities
        
        private async Task RecursiveDeleteAsync(string path)
        {
            var subObjects = await _amazonS3Service
                .GetObjectsAsync(path, false);

            foreach (var obj in subObjects)
            {
                var deleteFileRequest = new DeleteFileRequest
                {
                    FileUrl = obj.Key
                };
                // aws object is same for file and directory
                var _ = await _amazonS3Service.RemoveObjectAsync(deleteFileRequest, false)
                    .ConfigureAwait(false);
            }
            
            await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
        }
        
        private async Task<List<string>> GetDirectoryListAsync(string path = "")
        {
            path ??= string.Empty;
            path = path.TrimStart('/');

            var result = await GetAllObjectsAtRootPathAsync()
                .ConfigureAwait(false);

            result ??= new List<S3Object>();

            return result
                .Where(q => q.Key.EndsWith('/') && q.Size == 0)
                .Where(q => q.Key.StartsWith(path))
                .Select(q => q.Key.TrimEnd('/'))
                .Append(path)
                .OrderBy(q => q)
                .ToList();
        }

        private async Task<List<S3Object>> GetAllObjectsAtRootPathAsync()
        {
            if (S3Objects != null)
                return S3Objects;

            S3Objects = await _amazonS3Service
                .GetObjectsAsync(GetRootDirectory().TrimStart('/'), false)
                .ConfigureAwait(false);

            return S3Objects;
        }

        private string UniqueFileName(string fileName)
        {
            var randomString = Guid.NewGuid()
                .ToString()
                .Replace("-", "")
                .Substring(0, 8);
            return $"{randomString}_{fileName.Trim()}";
        }

        private async Task UploadFileAsync(string path, IFormFile formFile)
        {
            var fileName = UniqueFileName(formFile.FileName);

            int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out var w);
            int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out var h);
            // resize and return stream
            var imageBytes = ResizeImageFromStream(formFile, w, h);

            var uploadFileRequest = new UploadFileRequest
            {
                ContentType = formFile.ContentType,
                FileBytes = imageBytes,
                FolderPath = path.Trim('/') + '/',
                FileName = Path.GetFileNameWithoutExtension(fileName),
                Extension = _fileProvider.GetFileExtension(formFile.FileName)
            };

            await _amazonS3Service.UploadObjectAsync(uploadFileRequest, false)
                .ConfigureAwait(false);
        }

        private string GetFullPublicUrl(string fileKey)
        {
            //return $"{_amazonS3Settings.S3BaseUrl.TrimEnd('/')}/{fileKey}";
            return $"/{fileKey}";
        }

        protected virtual byte[] ResizeImageFromStream(IFormFile file, int width, int height)
        {
            using var stream = new MemoryStream();
            file.CopyTo(stream);

            using var image = Image.FromStream(stream);
            var ratio = image.Width / (float)image.Height;
            if (image.Width <= width && image.Height <= height)
                return stream.ToArray();

            if (width == 0 && height == 0)
                return stream.ToArray();

            var newWidth = width;
            int newHeight = Convert.ToInt16(Math.Floor(newWidth / ratio));
            if ((height > 0 && newHeight > height) || width == 0)
            {
                newHeight = height;
                newWidth = Convert.ToInt16(Math.Floor(newHeight * ratio));
            }

            using var newImage = new Bitmap(newWidth, newHeight);
            using var graphics = Graphics.FromImage(newImage);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            using var streamOut = new MemoryStream();
            newImage.Save(streamOut, GetImageFormat(file.FileName));
            return streamOut.ToArray();
        }
        protected virtual bool PathIsRoot(string path)
        {
            return (path.Trim('/') + '/').Equals(GetRootDirectory().Trim('/') + '/');
        }
        protected override bool IsPathAllowed(string path)
        {
            return (path.Trim('/') + '/').StartsWith(GetRootDirectory().Trim('/') + '/');
        }

        protected async Task<List<string>> GetFilesListAsync(string directoryPath, string type)
        {
            return (await GetS3FilesAsync(directoryPath, type)).Select(q => q.Key).ToList();
        }

        protected virtual async Task<List<S3File>> GetS3FilesAsync(string path = "", string type = "image")
        {
            path ??= string.Empty;
            path = path.TrimStart('/');

            var result = await GetAllObjectsAtRootPathAsync()
                .ConfigureAwait(false);

            result ??= new List<S3Object>();

            return result
                .Where(q => !q.Key.EndsWith('/') && q.Size != 0)
                .Where(q => q.Key.StartsWith(path))
                .Where(q => IsKeyInDir(q.Key, path))
                .Where(q => IsFileOfType(q.Key, type))
                .Select(q => new S3File
                {
                    Key = q.Key,
                    Size = q.Size,
                    LastModified = q.LastModified
                })?
                .ToList();
        }
        
        protected override string GetRootDirectory()
        {
            var format = GetSetting("INTERPOLATED_PATH");
            return !format.IsNullOrWhiteSpace() ? string.Format(format, CustomFolderName()) : base.GetRootDirectory();
        }

        protected virtual string CustomFolderName()
        {
            var customer = _workContext.CurrentCustomer;
            if (_customerService.IsInCustomerRole(customer, NopCustomerDefaults.OperatorRoleName)
                || _customerService.IsInCustomerRole(customer, NopCustomerDefaults.AdministratorRoleName))
                return "A0";
            return $"C{customer.Id.ToString()}";
        }

        protected virtual bool IsFileOfType(string fileKey, string type)
        {
            var fileExtension = _fileProvider.GetFileExtension(fileKey);
            return GetFileType(fileExtension) == type;
        }

        protected virtual bool IsKeyInDir(string key, string path)
        {
            var keyTreeLength = key.Count(c => c == '/');
            var pathTreeLength = path.Count(c => c == '/');
            return keyTreeLength == (pathTreeLength + 1);
        }

        #endregion

        #region Methods
        public override async Task GetFilesAsync(string directoryPath, string type)
        {
            if (!IsPathAllowed(directoryPath))
                throw new Exception(GetLanguageResource("E_ActionDisabled"));
                
            var files = await GetS3FilesAsync(directoryPath, type).ConfigureAwait(false);

            //ignore non image types
            files = files.Where(q => IsFileOfType(q.Key, type)).ToList();
            var (width, height) = (400, 400);

            await GetHttpContext().Response.WriteAsync("[");
            for (var i = 0; i < files.Count; i++)
            {
                var publicUrl = GetFullPublicUrl(files[i].Key);
                var timeStamp = Math.Ceiling(GetTimestamp(files[i].LastModified.GetValueOrDefault())).ToString();
                var fileSize = files[i].Size.ToString();

                await GetHttpContext().Response.WriteAsync($"{{\"p\":\"{publicUrl}\",\"t\":\"{timeStamp}\",\"s\":\"{fileSize}\",\"w\":\"{width}\",\"h\":\"{height}\"}}");

                if (i < files.Count - 1)
                    await GetHttpContext().Response.WriteAsync(",");
            }

            await GetHttpContext().Response.WriteAsync("]");
        }

        /// <summary>
        /// Get all available directories as a directory tree
        /// </summary>
        /// <param name="type">Type of the file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task GetDirectoriesAsync(string type)
        {
            var allDirectories = await GetDirectoryListAsync(GetRootDirectory()).ConfigureAwait(false);

            var localPath = GetFullPath(null);
            await GetHttpContext().Response.WriteAsync("[");
            for (var i = 0; i < allDirectories.Count; i++)
            {
                var directoryPath = (string)allDirectories[i];
                var dirPath = directoryPath.Replace(localPath, string.Empty).Replace("\\", "/").TrimStart('/');
                var fileCount = (await GetFilesListAsync(directoryPath, type)).Count.ToString();
                var subdirLength = (await GetDirectoryListAsync(directoryPath)).Count.ToString();
                await GetHttpContext().Response.WriteAsync($"{{\"p\":\"/{dirPath}\",\"f\":\"{fileCount}\",\"d\":\"{subdirLength}\"}}");
                if (i < allDirectories.Count - 1)
                    await GetHttpContext().Response.WriteAsync(",");
            }

            await GetHttpContext().Response.WriteAsync("]");
        }

        /// <summary>
        /// Create the thumbnail of the image and write it to the response
        /// </summary>
        /// <param name="path">Path to the image</param>
        public override void CreateImageThumbnail(string path)
        {
            if (path.IsNullOrWhiteSpace())
                return;

            int.TryParse(GetHttpContext().Request.Query["width"].ToString().Replace("px", string.Empty), out var width);
            int.TryParse(GetHttpContext().Request.Query["height"].ToString().Replace("px", string.Empty), out var height);

            using var stream = new MemoryStream();

            _amazonS3Service
                .GetObjectDataStreamAsync(stream, path.TrimStart('/'), false)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (stream.Length == 0)
                return;

            //using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var image = new Bitmap(Image.FromStream(stream));
            var cropX = 0;
            var cropY = 0;

            var imgRatio = image.Width / (double)image.Height;

            if (height == 0)
                height = Convert.ToInt32(Math.Floor(width / imgRatio));

            if (width > image.Width)
                width = image.Width;
            if (height > image.Height)
                height = image.Height;

            var cropRatio = width / (double)height;
            var cropWidth = Convert.ToInt32(Math.Floor(image.Height * cropRatio));
            var cropHeight = Convert.ToInt32(Math.Floor(cropWidth / cropRatio));

            if (cropWidth > image.Width)
            {
                cropWidth = image.Width;
                cropHeight = Convert.ToInt32(Math.Floor(cropWidth / cropRatio));
            }

            if (cropHeight > image.Height)
            {
                cropHeight = image.Height;
                cropWidth = Convert.ToInt32(Math.Floor(cropHeight * cropRatio));
            }

            if (cropWidth < image.Width)
                cropX = Convert.ToInt32(Math.Floor((double)(image.Width - cropWidth) / 2));
            if (cropHeight < image.Height)
                cropY = Convert.ToInt32(Math.Floor((double)(image.Height - cropHeight) / 2));

            using var cropImg = image.Clone(new Rectangle(cropX, cropY, cropWidth, cropHeight), PixelFormat.DontCare);
            GetHttpContext().Response.Headers.Add("Content-Type", MimeTypes.ImagePng);

            using var thumbnailImageStream = new MemoryStream();
            cropImg.GetThumbnailImage(width, height, () => false, IntPtr.Zero).Save(thumbnailImageStream, ImageFormat.Png);
            var thumbnailImageBinary = thumbnailImageStream.ToArray();
            GetHttpContext().Response.Body.WriteAsync(thumbnailImageBinary);
            GetHttpContext().Response.Body.Close();
        }

        /// <summary>
        /// Upload files to a directory on passed path
        /// </summary>
        /// <param name="directoryPath">Path to directory to upload files</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task UploadFilesAsync(string directoryPath)
        {
            var result = GetSuccessResponse();
            var hasErrors = false;

            if (!IsPathAllowed(directoryPath))
                throw new Exception(GetLanguageResource("E_UploadNotAll"));

            try
            {
                foreach (var formFile in GetHttpContext().Request.Form.Files)
                {
                    var fileName = formFile.FileName;
                    if (CanHandleFile(fileName))
                    {
                        if (GetFileType(new FileInfo(fileName).Extension) != "image")
                            continue;
                        
                        await UploadFileAsync(directoryPath, formFile).ConfigureAwait(false);
                    }
                    else
                    {
                        hasErrors = true;
                        result = GetErrorResponse(GetLanguageResource("E_UploadNotAll"));
                    }
                }
            }
            catch (Exception ex)
            {
                result = GetErrorResponse(ex.Message);
            }

            if (IsAjaxRequest())
            {
                if (hasErrors)
                    result = GetErrorResponse(GetLanguageResource("E_UploadNotAll"));

                await GetHttpContext().Response.WriteAsync(result);
            }
            else
                await GetHttpContext().Response.WriteAsync($"<script>parent.fileUploaded({result});</script>");
        }

        public override async Task CreateDirectoryAsync(string parentDirectoryPath, string name)
        {
            if (String.IsNullOrWhiteSpace(parentDirectoryPath))
                throw new ArgumentException($"{nameof(parentDirectoryPath)} cannot be empty");

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{nameof(name)} cannot be empty");

            if (!IsPathAllowed(parentDirectoryPath))
                throw new Exception(GetLanguageResource("E_CreateDirInvalidPath"));

            var result = GetSuccessResponse();
            var path = $"{parentDirectoryPath.TrimStart('/')}/{name}/";

            try
            {
                await _amazonS3Service.CreateDirectoryAsync(path, false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result = GetErrorResponse(ex.Message);
            }

            await GetHttpContext().Response.WriteAsync(result);
        }

        public override async Task DeleteFileAsync(string path)
        {
            if (!IsPathAllowed(path))
                throw new Exception(GetLanguageResource("E_ActionDisabled"));  
            
            var result = GetSuccessResponse();
            path = path.TrimStart('/');
            try
            {
                var deleteRequest = new DeleteFileRequest
                {
                    FileUrl = path
                };
                var response = await _amazonS3Service.RemoveObjectAsync(deleteRequest, false).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                result = GetErrorResponse(ex.Message);
            }

            await GetHttpContext().Response.WriteAsync(result);
        }
        
        /// <summary>
        /// Delete the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task DeleteDirectoryAsync(string path)
        {
            if (!IsPathAllowed(path))
                throw new Exception(GetLanguageResource("E_DeleteDirInvalidPath"));
            
            if (PathIsRoot(path))
                throw new Exception(GetLanguageResource("E_CannotDeleteRoot"));

            path = path.Trim('/') + '/';
            try
            {
                await RecursiveDeleteAsync(path).ConfigureAwait(false);
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_CannotDeleteDir"));
            }
        }

        #endregion
    }
}