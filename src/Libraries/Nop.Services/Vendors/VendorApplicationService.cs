using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorApplicationService : IVendorApplicationService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<VendorApplication> _vendorApplicationRepository;
        private readonly IRepository<VendorApplicationSampleProductPicture> _vendorApplicationSampleProductPictureRepository;

        #endregion

        #region Ctor

        public VendorApplicationService(IEventPublisher eventPublisher,
            IRepository<VendorApplication> vendorApplicationRepository,
            IRepository<VendorApplicationSampleProductPicture> vendorApplicationSampleProductPictureRepository)
        {
            _eventPublisher = eventPublisher;
            _vendorApplicationRepository = vendorApplicationRepository;
            _vendorApplicationSampleProductPictureRepository = vendorApplicationSampleProductPictureRepository;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets a vendorApplication by vendorApplication identifier
        /// </summary>
        /// <param name="vendorApplicationId">VendorApplication identifier</param>
        /// <returns>VendorApplication</returns>
        public virtual VendorApplication GetVendorApplicationById(int vendorApplicationId)
        {
            if (vendorApplicationId == 0)
                return null;

            return _vendorApplicationRepository.ToCachedGetById(vendorApplicationId);
        }

        /// <summary>
        /// Gets a vendorApplication by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>VendorApplication</returns>
        public virtual VendorApplication GetVendorApplicationByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            return (from v in _vendorApplicationRepository.Table
                    where v.CustomerId == customerId &&
                    v.IsApproved != false
                    select v).FirstOrDefault();
        }

        /// <summary>
        /// Gets all vendorApplications
        /// </summary>
        /// <param name="storeName">Store name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>VendorApplication</returns>
        public virtual IPagedList<VendorApplication> GetAllVendorApplications(bool? isApproved, string storeName = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _vendorApplicationRepository.Table;
            if (!string.IsNullOrWhiteSpace(storeName))
                query = query.Where(v => v.StoreName.Contains(storeName));
            query = isApproved != null ? query.Where(v => v.IsApproved == isApproved): query;
            query = query.Where(v => !v.Deleted);
            query = query.OrderByDescending(v => v.CreatedOnUtc).ThenByDescending(v => v.UpdatedOnUtc).ThenBy(v => v.StoreName);

            var vendorApplications = new PagedList<VendorApplication>(query, pageIndex, pageSize);
            return vendorApplications;
        }

        /// <summary>
        /// Inserts a vendorApplication
        /// </summary>
        /// <param name="vendorApplication">vendorApplication</param>
        public virtual VendorApplication InsertVendorApplication(VendorApplication vendorApplication)
        {
            if (vendorApplication == null)
                throw new ArgumentNullException(nameof(vendorApplication));

            _vendorApplicationRepository.Insert(vendorApplication);

            //event notification
            _eventPublisher.EntityInserted(vendorApplication);

            return vendorApplication;
        }

        /// <summary>
        /// Updates the vendorApplication
        /// </summary>
        /// <param name="vendorApplication">vendorApplication</param>
        public virtual void UpdateVendorApplication(VendorApplication vendorApplication)
        {
            if (vendorApplication == null)
                throw new ArgumentNullException(nameof(vendorApplication));

            _vendorApplicationRepository.Update(vendorApplication);

            //event notification
            _eventPublisher.EntityUpdated(vendorApplication);
        }

        /// <summary>
        /// Updates the vendor's approval status
        /// </summary>
        /// <param name="vendorApplicationId">vendorApplication identifier</param>
        /// <param name="newApprovalStatus">New approval status</param>
        public virtual bool UpdateVendorApplicationApprovalStatus(int vendorApplicationId, bool newApprovalStatus)
        {
            if (vendorApplicationId != 0)
            {
                var vendorApplication = _vendorApplicationRepository.ToCachedGetById(vendorApplicationId);

                if (vendorApplication != null)
                {
                    vendorApplication.IsApproved = newApprovalStatus;
                    _vendorApplicationRepository.Update(vendorApplication);

                    //event notification
                    _eventPublisher.EntityUpdated(vendorApplication);

                    return newApprovalStatus;
                }
            }
            return false;
        }

        #endregion

        #region sample product pictures
        /// <summary>
        /// Deletes a sample product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        public virtual void DeleteProductPicture(VendorApplicationSampleProductPicture sampleProductPicture)
        {
            if (sampleProductPicture == null)
                throw new ArgumentNullException(nameof(sampleProductPicture));

            _vendorApplicationSampleProductPictureRepository.Delete(sampleProductPicture);

            //event notification
            _eventPublisher.EntityDeleted(sampleProductPicture);
        }

        /// <summary>
        /// Gets a sample product pictures by va identifier
        /// </summary>
        /// <param name="vendorApplicationId">va identifier</param>
        /// <returns>Product pictures</returns>
        public virtual IList<VendorApplicationSampleProductPicture> GetSampleProductPicturesByVendorApplicationId(int vendorApplicationId)
        {
            var query = from pp in _vendorApplicationSampleProductPictureRepository.Table
                        where pp.VendorApplicationId == vendorApplicationId
                        orderby pp.Id
                        select pp;

            var sampleProductPictures = query.ToList();

            return sampleProductPictures;
        }

        /// <summary>
        /// Inserts a sample product picture
        /// </summary>
        /// <param name="vendorApplicationSampleProductPicture">va sample product picture</param>
        public virtual void InsertSampleProductPicture(VendorApplicationSampleProductPicture sampleProductPicture)
        {
            if (sampleProductPicture == null)
                throw new ArgumentNullException(nameof(sampleProductPicture));

            _vendorApplicationSampleProductPictureRepository.Insert(sampleProductPicture);

            //event notification
            _eventPublisher.EntityInserted(sampleProductPicture);
        }

        public IPagedList<VendorApplication> GetAllVendorApplicationsPendingApproval(int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = from vp in _vendorApplicationRepository.Table
                        where vp.Deleted == false && vp.IsApproved == null
                        select vp;

            query = query.OrderByDescending(x => x.Id);
            return new PagedList<VendorApplication>(query, pageIndex, pageSize);
        }

        #endregion

    }
}