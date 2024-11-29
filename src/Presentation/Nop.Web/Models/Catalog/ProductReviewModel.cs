using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductReviewOverviewModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public int RatingSum { get; set; }

        public int TotalReviews { get; set; }

        public bool AllowCustomerReviews { get; set; }
        public int RatingPercent { get; set; }

        public string ProductBusinessNature { get; set; }
        public decimal RatingAverage 
        {
            get
            {
                return TotalReviews == 0 ? 0 : decimal.Round(((decimal)RatingSum / (decimal)TotalReviews), 1) ;
            }
        }
    }

    public partial class ProductReviewsModel
    {
        public ProductReviewsModel()
        {
            Items = new List<ProductReviewModel>();
            AddProductReview = new AddProductReviewModel();
            ReviewTypeList = new List<ReviewTypeModel>();
            AddAdditionalProductReviewList = new List<AddProductReviewReviewTypeMappingModel>();
        }
        public partial class ProductReviewsRouteValues : IRouteValues
        {
            public int pageNumber { get; set; }
        }
        //public PagerModel PagerModel { get; set; }

        public string ProductBusinessNature { get; set; }
        public int ProductId { get; set; }
        [UIHint("Picture")]
        public int PictureId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public IList<ProductReviewModel> Items { get; set; }

        public AddProductReviewModel AddProductReview { get; set; }

        public IList<ReviewTypeModel> ReviewTypeList { get; set; }

        public IList<AddProductReviewReviewTypeMappingModel> AddAdditionalProductReviewList { get; set; }        
    }

    public partial class ReviewTypeModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        public bool VisibleToAllCustomers { get; set; }

        public double AverageRating { get; set; }
    }

    public partial class ProductReviewModel : BaseNopEntityModel
    {
        public ProductReviewModel()
        {
            AdditionalProductReviewList = new List<ProductReviewReviewTypeMappingModel>();
            PictureUrls = new List<string>(); 
        }


        public int CustomerId { get; set; }

        public string CustomerAvatarUrl { get; set; }

        public string CustomerName { get; set; }

        public bool AllowViewingProfiles { get; set; }
        
        public string Title { get; set; }

        public string ReviewText { get; set; }

        public string ReplyText { get; set; }

        public int Rating { get; set; }

        public string WrittenOnStr { get; set; }
        public string PictureUrl { get; set; }

        public ProductReviewHelpfulnessModel Helpfulness { get; set; }
        public IList<string> PictureUrls { get; set; }
        public IList<ProductReviewReviewTypeMappingModel> AdditionalProductReviewList { get; set; }


    }

    public partial class ProductReviewHelpfulnessModel : BaseNopModel
    {
        public int ProductReviewId { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }
        public bool? CheckBuyerVoted { get; set; }
    }

    public partial class AddProductReviewModel : BaseNopModel
    {
        [NopResourceDisplayName("Reviews.Fields.Title")]
        public string Title { get; set; }
        
        [NopResourceDisplayName("Reviews.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [NopResourceDisplayName("Reviews.Fields.Rating")]
        public int Rating { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool CanCurrentCustomerLeaveReview { get; set; }

        public bool SuccessfullyAdded { get; set; }

        public string Result { get; set; }
    }

    public partial class AddProductReviewReviewTypeMappingModel : BaseNopEntityModel
    {
        public int ProductReviewId { get; set; }

        public int ReviewTypeId { get; set; }

        public int Rating { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }
    }

    public partial class ProductReviewReviewTypeMappingModel : BaseNopEntityModel
    {
        public int ProductReviewId { get; set; }

        public int ReviewTypeId { get; set; }

        public int Rating { get; set; }

        public string Name { get; set; }

        public bool VisibleToAllCustomers { get; set; }
    }
}