using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorNote> _vendorNoteRepository;
        private readonly IRepository<VendorPicture> _vendorPictureRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IVendorAttributeValueParser _vendorAttributeValueParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemrepository;
        private readonly IRepository<Customer> _customerrepository;
        private readonly IRepository<OrderItem> _orderItemrepository;
        private readonly IRepository<Order> _orderrepository;
        private readonly ShippingJntSettings _shippingJntSettings;
        private readonly ShippingLalamoveSettings _shippingLalamoveSettings;

        #endregion

        #region Ctor

        public VendorService(IEventPublisher eventPublisher,
            IRepository<Product> productRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository,
            IRepository<VendorPicture> vendorPictureRepository,
            IGenericAttributeService genericAttributeService,
            IVendorAttributeValueParser vendorAttributeValueParser,
            IVendorAttributeService vendorAttributeService,
            IStaticCacheManager staticCacheManager,
            IRepository<ShoppingCartItem> shoppingCartItemrepository,
            IRepository<Customer> customerrepository,
            IRepository<OrderItem> orderItemrepository,
            IRepository<Order> orderrepository,
            ShippingJntSettings shippingJntSettings,
            ShippingLalamoveSettings shippingLalamoveSettings)
        {
            _eventPublisher = eventPublisher;
            _productRepository = productRepository;
            _vendorRepository = vendorRepository;
            _vendorNoteRepository = vendorNoteRepository;
            _vendorPictureRepository = vendorPictureRepository;
            _genericAttributeService = genericAttributeService;
            _vendorAttributeValueParser = vendorAttributeValueParser;
            _vendorAttributeService = vendorAttributeService;
            _staticCacheManager = staticCacheManager;
            _shoppingCartItemrepository = shoppingCartItemrepository;
            _customerrepository = customerrepository;
            _orderItemrepository = orderItemrepository;
            _orderrepository = orderrepository;
            _shippingJntSettings = shippingJntSettings;
            _shippingLalamoveSettings = shippingLalamoveSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        public virtual Vendor GetVendorById(int vendorId)
        {
            if (vendorId == 0)
                return null;

            return _vendorRepository.ToCachedGetById(vendorId);
        }

        /// <summary>
        /// Gets a vendor by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Vendor</returns>
        public virtual Vendor GetVendorByProductId(int productId)
        {
            if (productId == 0)
                return null;

            return (from v in _vendorRepository.Table
                    join p in _productRepository.Table on v.Id equals p.VendorId
                    where p.Id == productId
                    select v).FirstOrDefault();
        }

        /// <summary>
        /// Gets a vendors by product identifiers
        /// </summary>
        /// <param name="productIds">Array of product identifiers</param>
        /// <returns>Vendors</returns>
        public virtual IList<Vendor> GetVendorsByProductIds(int[] productIds)
        {
            if (productIds is null)
                throw new ArgumentNullException(nameof(productIds));

            return (from v in _vendorRepository.Table
                    join p in _productRepository.Table on v.Id equals p.VendorId
                    where productIds.Contains(p.Id) && !v.Deleted && v.Active
                    group v by p.Id into v
                    select v.First()).ToList();
        }

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void DeleteVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            vendor.Deleted = true;
            UpdateVendor(vendor);

            //event notification
            _eventPublisher.EntityDeleted(vendor);
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="email">Vendor email</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetAllVendors(string name = "", string email = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(v => v.Email.Contains(email));

            if (!showHidden)
                query = query.Where(v => v.Active);

            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name).ThenBy(v => v.Email);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Gets vendors
        /// </summary>
        /// <param name="vendorIds">Vendor identifiers</param>
        /// <returns>Vendors</returns>
        public virtual IList<Vendor> GetVendorsByIds(int[] vendorIds)
        {
            var query = _vendorRepository.Table;
            if (vendorIds != null)
                query = query.Where(v => vendorIds.Contains(v.Id));

            return query.ToList();
        }

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void InsertVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            vendor.CreatedOnUtc = DateTime.UtcNow;
            _vendorRepository.Insert(vendor);

            //event notification
            _eventPublisher.EntityInserted(vendor);
        }

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void UpdateVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            _vendorRepository.Update(vendor);

            //event notification
            _eventPublisher.EntityUpdated(vendor);
            _staticCacheManager.RemoveByPrefix(NopOrderDefaults.ShoppingCartPrefixCacheKey);
        }

        /// <summary>
        /// Gets a vendor note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        public virtual VendorNote GetVendorNoteById(int vendorNoteId)
        {
            if (vendorNoteId == 0)
                return null;

            return _vendorNoteRepository.ToCachedGetById(vendorNoteId);
        }

        /// <summary>
        /// Gets all vendor notes
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Vendor notes</returns>
        public virtual IPagedList<VendorNote> GetVendorNotesByVendor(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _vendorNoteRepository.Table.Where(vn => vn.VendorId == vendorId);

            query = query.OrderBy(v => v.CreatedOnUtc).ThenBy(v => v.Id);

            return new PagedList<VendorNote>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        public virtual void DeleteVendorNote(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException(nameof(vendorNote));

            _vendorNoteRepository.Delete(vendorNote);

            //event notification
            _eventPublisher.EntityDeleted(vendorNote);
        }

        /// <summary>
        /// Inserts a vendor note
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        public virtual void InsertVendorNote(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException(nameof(vendorNote));

            _vendorNoteRepository.Insert(vendorNote);

            //event notification
            _eventPublisher.EntityInserted(vendorNote);
        }

        /// <summary>
        /// Formats the vendor note text
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatVendorNoteText(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException(nameof(vendorNote));

            var text = vendorNote.Note;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        /// <summary>
        /// Gets a vendor picture
        /// </summary>
        /// <param name="vendorPictureId">The vendor picture identifier</param>
        /// <returns>Vendor picture</returns>
        public virtual VendorPicture GetVendorPictureById(int vendorPictureId)
        {
            if (vendorPictureId == 0)
                return null;

            return _vendorPictureRepository.ToCachedGetById(vendorPictureId);
        }

        /// <summary>
        /// Gets all vendor pictures
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Vendor pictures</returns>
        public virtual IPagedList<VendorPicture> GetVendorPicturesByVendorId(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _vendorPictureRepository.Table.Where(vn => vn.VendorId == vendorId);

            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Id);

            return new PagedList<VendorPicture>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes a vendor picture
        /// </summary>
        /// <param name="vendorPicture">The vendor picture</param>
        public virtual void DeleteVendorPicture(VendorPicture vendorPicture)
        {
            if (vendorPicture == null)
                throw new ArgumentNullException(nameof(vendorPicture));

            _vendorPictureRepository.Delete(vendorPicture);

            //event notification
            _eventPublisher.EntityDeleted(vendorPicture);
        }

        /// <summary>
        /// Inserts a vendor picture
        /// </summary>
        /// <param name="vendorPicture">Vendor picture</param>
        public virtual void InsertVendorPicture(VendorPicture vendorPicture)
        {
            if (vendorPicture == null)
                throw new ArgumentNullException(nameof(vendorPicture));

            _vendorPictureRepository.Insert(vendorPicture);

            //event notification
            _eventPublisher.EntityInserted(vendorPicture);
        }

        /// <summary>
        /// Updates a vendor picture
        /// </summary>
        /// <param name="vendorPicture">Vendor picture</param>
        public virtual void UpdateVendorPicture(VendorPicture vendorPicture)
        {
            if (vendorPicture == null)
                throw new ArgumentNullException(nameof(vendorPicture));

            _vendorPictureRepository.Update(vendorPicture);

            //event notification
            _eventPublisher.EntityUpdated(vendorPicture);
        }

        public virtual Vendor GetVendorByOrderId(int orderId)
        {
            var query = from v in _vendorRepository.Table
                        join p in _productRepository.Table on v.Id equals p.VendorId
                        join oi in _orderItemrepository.Table on p.Id equals oi.ProductId
                        join o in _orderrepository.Table on oi.OrderId equals o.Id
                        where o.Id == orderId && v.Active == true
                        select v;
            return query.FirstOrDefault();
        }

        public int GetVendorCustomerIdByOrderId(int orderId)
        {
            var query = from v in _vendorRepository.Table
                        join p in _productRepository.Table on v.Id equals p.VendorId
                        join oi in _orderItemrepository.Table on p.Id equals oi.ProductId
                        join o in _orderrepository.Table on oi.OrderId equals o.Id
                        join c in _customerrepository.Table on v.Id equals c.VendorId
                        where o.Id == orderId && v.Active == true
                        select c;
            return query.FirstOrDefault().Id;
        }

        public VendorAttributeValue GetVendorAttributeValue(Vendor vendor, string attributeName)
        {
            var vendorAttributeValues = GetVendorAttributes(vendor);

            var attributeId = _vendorAttributeService.GetAllVendorAttributes()
                .FirstOrDefault(q => q.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase))
                ?.Id ?? 0;

            return vendorAttributeValues
                .FirstOrDefault(x => x.VendorAttributeId.Equals(attributeId));
        }

        public IList<VendorAttributeValue> GetVendorAttributes(Vendor vendor)
        {
            var vendorAttributeXml =
                _genericAttributeService.GetAttribute<string>(vendor, NopVendorDefaults.VendorAttributes);

            return _vendorAttributeValueParser.ParseVendorAttributeValues(vendorAttributeXml);
        }

        public bool GetVendorOnlineStatus(Vendor vendor)
        {
            return vendor.Online && (vendor.OfflineUntil == null || vendor.OfflineUntil < DateTime.UtcNow);
        }

        public decimal GetVendorMaxShipmentWeight(string categoryVendorName, bool deliverByCar)
        {
            if (NopVendorDefaults.VendorCategoryEats == categoryVendorName)
            {
                if (deliverByCar)
                {
                    return _shippingLalamoveSettings.MaxWeightCar;
                }
                else
                {
                    return _shippingLalamoveSettings.MaxWeightBike;
                }
            }
            else
            {
                return _shippingJntSettings.MaxWeight;
            }
        }

        public int GetCustomerIdByVendor(Vendor vendor)
        {
            var query = from c in _customerrepository.Table
                where c.VendorId == vendor.Id
                select c;
            return query.FirstOrDefault()?.Id ?? 0;
        }

        #endregion

    }
}