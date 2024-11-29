using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Models.Charge;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Web.Enums;

namespace Nop.Web.Areas.Admin.Factories
{
    public class ChargeModelFactory
    {
        #region Fields

        private readonly ChargeService _chargeService;

        #endregion

        #region Ctor

        public ChargeModelFactory(
            ChargeService chargeService)
        {
            _chargeService = chargeService;
        }

        #endregion

        #region Methods

        public virtual ChargeSearchModel PrepareChargeSearchModel(ChargeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual ChargeListModel PrepareChargeListModel(ChargeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var charge = _chargeService.SearchChargesTable(searchModel.SearchProductTypeId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new ChargeListModel().PrepareToGrid(searchModel, charge, () =>
            {
                return charge.Select(entity =>
                {
                    //fill in model values from the entity
                    var chargeModel = entity.ToModel<ChargeModel>();
                    chargeModel.ProductTypeName = ((ProductType)chargeModel.ProductTypeId).GetName();
                    chargeModel.ValueTypeName = ((ChargeValueType)chargeModel.ValueType).GetDescription();
                    //chargeModel.StartDate.ToShortDateString();
                    //chargeModel.EndDate.ToShortDateString();
                    if (chargeModel.ProductTypeId == (int)ProductType.ViewJobCandidateFullProfileSubscription)
                    {
                        chargeModel.SubProductTypeName = ((JobType)chargeModel.SubProductTypeId).GetDescription();

                    }
                    else if (chargeModel.ProductTypeId == (int)ProductType.ServiceEnagegementMatchingFee)
                    {
                        chargeModel.SubProductTypeName = ((ServiceType)chargeModel.SubProductTypeId).GetDescription();

                    }
                    else if (chargeModel.ProductTypeId == (int)ProductType.ModeratorFacilitateConsultationFee)
                    {
                        chargeModel.SubProductTypeName = ((ModeratorFacilitateConsultationFeeType)chargeModel.SubProductTypeId).GetDescription();

                    }
                    else
                    {
                        chargeModel.SubProductTypeName = "-";
                    }

                    return chargeModel;
                });
            });

            return model;
        }

        public virtual ChargeModel PrepareChargeModel(ChargeModel model, Charge charge, bool excludeProperties = false)
        {
            if (charge != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = charge.ToModel<ChargeModel>();
                }
                if (model.EndDate == null)
                {
                    model.EndDateNull = true;
                }

            }
            return model;
        }

        #endregion
    }
}
