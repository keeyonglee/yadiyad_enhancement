using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.qBoSlider.Domain;

namespace Nop.Plugin.Widgets.qBoSlider.Mapping
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/09/19 13:40:00:1687541", "M210919_1440_Alter_WidgetZone_Add_Height")]
    public class M210919_1440_Alter_WidgetZone_Add_Height : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public M210919_1440_Alter_WidgetZone_Add_Height(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            if (!Schema.Table("Baroque_qBoSlider_WidgetZone")
             .Column(nameof(WidgetZone.MaxSlideWidgetZoneHeight))
             .Exists())
            {
                Create.Column(nameof(WidgetZone.MaxSlideWidgetZoneHeight))
                    .OnTable("Baroque_qBoSlider_WidgetZone")
                    .AsInt32()
                    .Nullable();
            }

            if (!Schema.Table("Baroque_qBoSlider_WidgetZone")
             .Column(nameof(WidgetZone.MinSlideWidgetZoneHeight))
             .Exists())
            {
                Create.Column(nameof(WidgetZone.MinSlideWidgetZoneHeight))
                    .OnTable("Baroque_qBoSlider_WidgetZone")
                    .AsInt32()
                    .Nullable();
            }
        }
    }
}
