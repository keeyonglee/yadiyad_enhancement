using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.ShippingShuq;

namespace YadiYad.Pro.Data.Mapping.Builders.Shipping
{
    public class VendorAttributeValue_ShippingMethod_MappingBuilder : NopEntityBuilder<VendorAttributeValue_ShippingMethod_Mapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VendorAttributeValue_ShippingMethod_Mapping.VendorAttributeValueId)).AsInt32().NotNullable()
                .WithColumn(nameof(VendorAttributeValue_ShippingMethod_Mapping.ShippingMethodId)).AsInt32().NotNullable();


        }

        #endregion
    }
}
