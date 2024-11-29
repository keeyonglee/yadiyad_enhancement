using Nop.Core.Domain.Localization;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Plugins;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ApiCacheEventConsumer :
        //languages
        IConsumer<EntityInsertedEvent<Language>>,
        IConsumer<EntityUpdatedEvent<Language>>,
        IConsumer<EntityDeletedEvent<Language>>,
        //currencies
        IConsumer<EntityInsertedEvent<Currency>>,
        IConsumer<EntityUpdatedEvent<Currency>>,
        IConsumer<EntityDeletedEvent<Currency>>,
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        //vendors
        IConsumer<EntityInsertedEvent<Vendor>>,
        IConsumer<EntityUpdatedEvent<Vendor>>,
        IConsumer<EntityDeletedEvent<Vendor>>,
        //product manufacturers
        IConsumer<EntityInsertedEvent<ProductManufacturer>>,
        IConsumer<EntityUpdatedEvent<ProductManufacturer>>,
        IConsumer<EntityDeletedEvent<ProductManufacturer>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //product categories
        IConsumer<EntityInsertedEvent<ProductCategory>>,
        IConsumer<EntityUpdatedEvent<ProductCategory>>,
        IConsumer<EntityDeletedEvent<ProductCategory>>,
        //products
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        //related product
        IConsumer<EntityInsertedEvent<RelatedProduct>>,
        IConsumer<EntityUpdatedEvent<RelatedProduct>>,
        IConsumer<EntityDeletedEvent<RelatedProduct>>,
        //product tags
        IConsumer<EntityInsertedEvent<ProductTag>>,
        IConsumer<EntityUpdatedEvent<ProductTag>>,
        IConsumer<EntityDeletedEvent<ProductTag>>,
        //specification attributes
        IConsumer<EntityUpdatedEvent<SpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<SpecificationAttribute>>,
        //specification attribute options
        IConsumer<EntityUpdatedEvent<SpecificationAttributeOption>>,
        IConsumer<EntityDeletedEvent<SpecificationAttributeOption>>,
        //Product specification attribute
        IConsumer<EntityInsertedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityUpdatedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<ProductSpecificationAttribute>>,
        //Product attribute values
        IConsumer<EntityUpdatedEvent<ProductAttributeValue>>,
        //Topics
        IConsumer<EntityInsertedEvent<Topic>>,
        IConsumer<EntityUpdatedEvent<Topic>>,
        IConsumer<EntityDeletedEvent<Topic>>,
        //Orders
        IConsumer<EntityInsertedEvent<Order>>,
        IConsumer<EntityUpdatedEvent<Order>>,
        IConsumer<EntityDeletedEvent<Order>>,
        //Picture
        IConsumer<EntityInsertedEvent<Picture>>,
        IConsumer<EntityUpdatedEvent<Picture>>,
        IConsumer<EntityDeletedEvent<Picture>>,
        //Product picture mapping
        IConsumer<EntityInsertedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductPicture>>,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        //Product review
        IConsumer<EntityDeletedEvent<ProductReview>>,
        //polls
        IConsumer<EntityInsertedEvent<Poll>>,
        IConsumer<EntityUpdatedEvent<Poll>>,
        IConsumer<EntityDeletedEvent<Poll>>,
        //blog posts
        IConsumer<EntityInsertedEvent<BlogPost>>,
        IConsumer<EntityUpdatedEvent<BlogPost>>,
        IConsumer<EntityDeletedEvent<BlogPost>>,
        //blog comments
        IConsumer<EntityDeletedEvent<BlogComment>>,
        //news items
        IConsumer<EntityInsertedEvent<NewsItem>>,
        IConsumer<EntityUpdatedEvent<NewsItem>>,
        IConsumer<EntityDeletedEvent<NewsItem>>,
        //news comments
        IConsumer<EntityDeletedEvent<NewsComment>>,
        //states/province
        IConsumer<EntityInsertedEvent<StateProvince>>,
        IConsumer<EntityUpdatedEvent<StateProvince>>,
        IConsumer<EntityDeletedEvent<StateProvince>>,
        //return requests
        IConsumer<EntityInsertedEvent<ReturnRequestAction>>,
        IConsumer<EntityUpdatedEvent<ReturnRequestAction>>,
        IConsumer<EntityDeletedEvent<ReturnRequestAction>>,
        IConsumer<EntityInsertedEvent<ReturnRequestReason>>,
        IConsumer<EntityUpdatedEvent<ReturnRequestReason>>,
        IConsumer<EntityDeletedEvent<ReturnRequestReason>>,
        //templates
        IConsumer<EntityInsertedEvent<CategoryTemplate>>,
        IConsumer<EntityUpdatedEvent<CategoryTemplate>>,
        IConsumer<EntityDeletedEvent<CategoryTemplate>>,
        IConsumer<EntityInsertedEvent<ManufacturerTemplate>>,
        IConsumer<EntityUpdatedEvent<ManufacturerTemplate>>,
        IConsumer<EntityDeletedEvent<ManufacturerTemplate>>,
        IConsumer<EntityInsertedEvent<ProductTemplate>>,
        IConsumer<EntityUpdatedEvent<ProductTemplate>>,
        IConsumer<EntityDeletedEvent<ProductTemplate>>,
        IConsumer<EntityInsertedEvent<TopicTemplate>>,
        IConsumer<EntityUpdatedEvent<TopicTemplate>>,
        IConsumer<EntityDeletedEvent<TopicTemplate>>,
        //checkout attributes
        IConsumer<EntityInsertedEvent<CheckoutAttribute>>,
        IConsumer<EntityUpdatedEvent<CheckoutAttribute>>,
        IConsumer<EntityDeletedEvent<CheckoutAttribute>>,
        //shopping cart items
        IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
        //plugins
        IConsumer<PluginUpdatedEvent>,
        //string resources
        IConsumer<EntityInsertedEvent<LocaleStringResource>>,
        IConsumer<EntityDeletedEvent<LocaleStringResource>>,
        IConsumer<EntityUpdatedEvent<LocaleStringResource>>
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ApiCacheEventConsumer(CatalogSettings catalogSettings, IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        //languages
        public void HandleEvent(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SearchCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductSpecsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StateProvincesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableLanguagesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableCurrenciesPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SearchCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductSpecsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StateProvincesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableLanguagesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableCurrenciesPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SearchCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductSpecsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StateProvincesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableLanguagesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableCurrenciesPrefixCacheKey);
        }

        //currencies
        public void HandleEvent(EntityInsertedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableCurrenciesPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableCurrenciesPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.AvailableCurrenciesPrefixCacheKey);
        }

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagPopularPrefixCacheKey); //depends on CatalogSettings.NumberOfProductTags
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerNavigationPrefixCacheKey); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.VendorNavigationPrefixCacheKey); //depends on VendorSettings.VendorBlockItemsToDisplay
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryNumberOfProductsPrefixCacheKey); //depends on CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsRelatedIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.BlogPrefixCacheKey); //depends on BlogSettings.NumberOfTags
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StoreLogoPathPrefixCacheKey); //depends on StoreInformationSettings.LogoPictureId
        }

        //vendors
        public void HandleEvent(EntityInsertedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.VendorNavigationPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.VendorNavigationPrefixCacheKey);
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.VendorPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeletedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.VendorNavigationPrefixCacheKey);
        }

        //manufacturers
        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);

        }
        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ManufacturerPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }

        //product manufacturers
        public void HandleEvent(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ManufacturerHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.ManufacturerId));
        }
        public void HandleEvent(EntityUpdatedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ManufacturerHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.ManufacturerId));
        }
        public void HandleEvent(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductManufacturersPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ManufacturerHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.ManufacturerId));
        }

        //categories
        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SearchCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategorySubcategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SearchCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryBreadcrumbPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategorySubcategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.CategoryPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }
        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SearchCategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryBreadcrumbPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategorySubcategoriesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }

        //product categories
        public void HandleEvent(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKeyById, eventMessage.Entity.ProductId));
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
                _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryNumberOfProductsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.CategoryHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.CategoryId));
        }
        public void HandleEvent(EntityUpdatedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryNumberOfProductsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.CategoryHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.CategoryId));
        }
        public void HandleEvent(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductBreadcrumbPrefixCacheKeyById, eventMessage.Entity.ProductId));
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryAllPrefixCacheKey);
                _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryNumberOfProductsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.CategoryHasFeaturedProductsPrefixCacheKeyById, eventMessage.Entity.CategoryId));
        }

        //products
        public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsRelatedIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.Id));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagByProductPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsRelatedIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }

        //product tags
        public void HandleEvent(EntityInsertedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagPopularPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagByProductPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagPopularPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagByProductPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagPopularPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTagByProductPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }

        //related products
        public void HandleEvent(EntityInsertedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsRelatedIdsPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsRelatedIdsPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsRelatedIdsPrefixCacheKey);
        }

        //specification attributes
        public void HandleEvent(EntityUpdatedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductSpecsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductSpecsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        //specification attribute options
        public void HandleEvent(EntityUpdatedEvent<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductSpecsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductSpecsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        //Product specification attribute
        public void HandleEvent(EntityInsertedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductSpecsPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductSpecsPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductSpecsPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        //Product attributes
        public void HandleEvent(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributeImageSquarePicturePrefixCacheKey);
        }

        //Topics
        public void HandleEvent(EntityInsertedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }

        //Orders
        public void HandleEvent(EntityInsertedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        //Pictures
        public void HandleEvent(EntityInsertedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CartPicturePrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CartPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductDefaultPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.VendorPicturePrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CartPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductDefaultPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        //Product picture mappings
        public void HandleEvent(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CartPicturePrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CartPicturePrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        //Polls
        public void HandleEvent(EntityInsertedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.PollsPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.PollsPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.PollsPrefixCacheKey);
        }

        //Blog posts
        public void HandleEvent(EntityInsertedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.BlogPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.BlogPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.BlogPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }

        //Blog comments
        public void HandleEvent(EntityDeletedEvent<BlogComment> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.BlogCommentsPrefixCacheKey);
        }

        //News items
        public void HandleEvent(EntityInsertedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.NewsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.NewsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.NewsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SitemapPrefixCacheKey);
        }
        //News comments
        public void HandleEvent(EntityDeletedEvent<NewsComment> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.NewsCommentsPrefixCacheKey);
        }

        //State/province
        public void HandleEvent(EntityInsertedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StateProvincesPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StateProvincesPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StateProvincesPrefixCacheKey);
        }

        //return requests
        public void HandleEvent(EntityInsertedEvent<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ReturnRequestActionsPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ReturnRequestActionsPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ReturnRequestAction> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ReturnRequestActionsPrefixCacheKey);
        }
        public void HandleEvent(EntityInsertedEvent<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ReturnRequestReasonsPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ReturnRequestReasonsPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ReturnRequestReason> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ReturnRequestReasonsPrefixCacheKey);
        }

        //templates
        public void HandleEvent(EntityInsertedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CategoryTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityInsertedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ManufacturerTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityInsertedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.ProductTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityInsertedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicTemplatePrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.TopicTemplatePrefixCacheKey);
        }

        //checkout attributes
        public void HandleEvent(EntityInsertedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CheckoutAttributesPrefixCacheKey);
        }
        public void HandleEvent(EntityUpdatedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CheckoutAttributesPrefixCacheKey);
        }
        public void HandleEvent(EntityDeletedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CheckoutAttributesPrefixCacheKey);
        }

        //shopping cart items
        public void HandleEvent(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        //product reviews
        public void HandleEvent(EntityDeletedEvent<ProductReview> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.ProductId));
        }

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PluginUpdatedEvent eventMessage)
        {
            if (eventMessage?.Plugin?.Instance<IWidgetPlugin>() != null)
                _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.WidgetPrefixCacheKey);
        }

        public void HandleEvent(EntityInsertedEvent<LocaleStringResource> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.StringResourceKey, eventMessage.Entity.LanguageId));
        }

        public void HandleEvent(EntityDeletedEvent<LocaleStringResource> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.StringResourceKey, eventMessage.Entity.LanguageId));
        }

        public void HandleEvent(EntityUpdatedEvent<LocaleStringResource> eventMessage)
        {
            _cacheManager.RemoveByPrefix(string.Format(ApiModelCacheDefaults.StringResourceKey, eventMessage.Entity.LanguageId));
        }

        #endregion
    }
}