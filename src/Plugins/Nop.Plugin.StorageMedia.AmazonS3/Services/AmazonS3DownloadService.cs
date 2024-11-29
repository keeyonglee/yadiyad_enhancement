using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Plugin.StorageMedia.AmazonS3.Models;
using Nop.Services.Events;
using Nop.Services.Media;

namespace Nop.Plugin.StorageMedia.AmazonS3.Services
{  /// <summary>
   /// Download service
   /// </summary>
    public partial class AmazonS3DownloadService : IDownloadService
    {
        #region Fields

        private const string AWS_DOC_FILE_PATH = "pro/doc/";
        private readonly IEventPublisher _eventPubisher;
        private readonly IRepository<Download> _downloadRepository;
        private readonly AmazonS3Service _amazonS3Service;
        #endregion

        #region Ctor

        public AmazonS3DownloadService(IEventPublisher eventPubisher,
            IRepository<Download> downloadRepository,
            AmazonS3Service amazonS3Service)
        {
            _eventPubisher = eventPubisher;
            _downloadRepository = downloadRepository;
            _amazonS3Service = amazonS3Service;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        /// <returns>Download</returns>
        public virtual Download GetDownloadById(int downloadId)
        {
            if (downloadId == 0)
                return null;

            return _downloadRepository.GetById(downloadId);
        }

        /// <summary>
        /// Gets a download by GUID
        /// </summary>
        /// <param name="downloadGuid">Download GUID</param>
        /// <returns>Download</returns>
        public virtual Download GetDownloadByGuid(Guid downloadGuid)
        {
            if (downloadGuid == Guid.Empty)
                return null;

            var query = from o in _downloadRepository.Table
                        where o.DownloadGuid == downloadGuid
                        select o;

            var download = query.FirstOrDefault();

            if (download != null) 
            {
                var filePath = GetAWSFilePath(download);

                download.DownloadBinary = _amazonS3Service.GetObjectDataAsync(filePath, download.IsPrivateContent).Result;
            }

            return download;
        }

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void DeleteDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            _downloadRepository.Delete(download);

            var filePath = GetAWSFilePath(download);

            var deleteFileRequest = new DeleteFileRequest
            {
                FileUrl = filePath
            };

            var deleteFileResponse = _amazonS3Service.RemoveObjectAsync(deleteFileRequest, download.IsPrivateContent).Result;

            //event notification
            _eventPubisher.EntityDeleted(download);
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            var uploadObjRequest = new Models.UploadFileRequest
            {
                FileBytes = download.DownloadBinary,
                ContentType = download.ContentType,
                Extension = download.Extension,
                FileName = download.Filename,
                FolderPath = AWS_DOC_FILE_PATH
            };

            var uploadObjResponse = _amazonS3Service.UploadObjectAsync(uploadObjRequest, download.IsPrivateContent).Result;

            if(uploadObjResponse.Success == false)
            {
                throw new Exception(uploadObjResponse.FileName);
            }
            else
            {
                download.DownloadBinary = Array.Empty<byte>();
            }

            _downloadRepository.Insert(download);

            //event notification
            _eventPubisher.EntityInserted(download);
        }

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            var uploadObjRequest = new Models.UploadFileRequest
            {
                FileBytes = download.DownloadBinary,
                ContentType = download.ContentType,
                Extension = download.Extension,
                FileName = download.Filename,
                FolderPath = AWS_DOC_FILE_PATH
            };

            var uploadObjResponse = _amazonS3Service.UploadObjectAsync(uploadObjRequest, download.IsPrivateContent).Result;

            if (uploadObjResponse.Success == false)
            {
                throw new Exception(uploadObjResponse.FileName);
            }
            else
            {
                download.DownloadBinary = Array.Empty<byte>();
            }

            _downloadRepository.Update(download);

            //event notification
            _eventPubisher.EntityUpdated(download);
        }

        /// <summary>
        /// Gets the download binary array
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>Download binary array</returns>
        public virtual byte[] GetDownloadBits(IFormFile file)
        {
            using var fileStream = file.OpenReadStream();
            using var ms = new MemoryStream();
            fileStream.CopyTo(ms);
            var fileBytes = ms.ToArray();
            return fileBytes;
        }

        public string GetAWSFilePath(Download download)
        {
            var filePath = $"{AWS_DOC_FILE_PATH}{download.Filename}{download.Extension}";

            return filePath;
        }

        public string GetDownloadUrl(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            var filePath = GetAWSFilePath(download);

            return _amazonS3Service.GetPreSignedUrl(filePath, false);
        }

        #endregion
    }
}
