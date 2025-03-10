﻿using Nop.Data.Mapping.Builders;
using FluentMigrator.Builders.Create.Table;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Data
{
    public class ApiSliderRecordBuilder : NopEntityBuilder<ApiSlider>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ApiSlider.PictureId))
                .AsInt32()
                .WithColumn(nameof(ApiSlider.ActiveStartDateUtc))
                .AsDateTime().Nullable()
                .WithColumn(nameof(ApiSlider.ActiveEndDateUtc))
                .AsDateTime().Nullable()
                .WithColumn(nameof(ApiSlider.SliderTypeId))
                .AsInt32()
                .WithColumn(nameof(ApiSlider.EntityId))
                .AsInt32()
                .WithColumn(nameof(ApiSlider.DisplayOrder))
                .AsInt32()
                .WithColumn(nameof(ApiSlider.CreatedOnUtc))
                .AsDateTime();
        }
    }
}
