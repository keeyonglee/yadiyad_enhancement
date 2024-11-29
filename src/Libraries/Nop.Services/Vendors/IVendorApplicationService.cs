using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// VendorApplication service interface
    /// </summary>
    public partial interface IVendorApplicationService
    {
        /// <summary>
        /// Gets a vendorApplication by vendorApplication identifier
        /// </summary>
        /// <param name="vendorApplicationId">VendorApplication identifier</param>
        /// <returns>VendorApplication</returns>
        VendorApplication GetVendorApplicationById(int vendorApplicationId);

        /// <summary>
        /// Gets a vendorApplication by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>VendorApplication</returns>
        VendorApplication GetVendorApplicationByCustomerId(int customerId);

        /// <summary>
        /// Gets all vendorApplications
        /// </summary>
        /// <param name="storeName">Store name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>VendorApplication</returns>
        IPagedList<VendorApplication> GetAllVendorApplications(bool? isApproved, string storeName = "", int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a vendorApplication
        /// </summary>
        /// <param name="vendorApplication">vendorApplication</param>
        VendorApplication InsertVendorApplication(VendorApplication vendorApplication);

        /// <summary>
        /// Updates the vendorApplication
        /// </summary>
        /// <param name="vendorApplication">vendorApplication</param>
        void UpdateVendorApplication(VendorApplication vendorApplication);

        /// <summary>
        /// Updates the vendor's approval status
        /// </summary>
        /// <param name="vendorApplicationId">vendorApplication identifier</param>
        /// <param name="newApprovalStatus">New approval status</param>
        bool UpdateVendorApplicationApprovalStatus(int vendorApplicationId, bool newApprovalStatus);


        /// <summary>
        /// Deletes a sample product picture
        /// </summary>
        /// <param name="productPicture">Product picture</param>
        void DeleteProductPicture(VendorApplicationSampleProductPicture sampleProductPicture);

        /// <summary>
        /// Gets a sample product pictures by va identifier
        /// </summary>
        /// <param name="vendorApplicationId">va identifier</param>
        /// <returns>Product pictures</returns>
        IList<VendorApplicationSampleProductPicture> GetSampleProductPicturesByVendorApplicationId(int vendorApplicationId);

        /// <summary>
        /// Inserts a sample product picture
        /// </summary>
        /// <param name="vendorApplicationSampleProductPicture">va sample product picture</param>
        void InsertSampleProductPicture(VendorApplicationSampleProductPicture sampleProductPicture);
        IPagedList<VendorApplication> GetAllVendorApplicationsPendingApproval(int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null);
    }
}