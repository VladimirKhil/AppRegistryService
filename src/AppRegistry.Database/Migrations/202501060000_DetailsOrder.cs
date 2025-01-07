using AppRegistry.Database.Models;
using FluentMigrator;

namespace AppRegistry.Database.Migrations;

[Migration(202501060000, "Family details and app/app installer order")]
public sealed class DetailsOrder : Migration
{
    public override void Up()
    {
        Alter.Table(DbConstants.AppFamilies).AddColumn(nameof(AppFamily.Details)).AsString().Nullable();
        Alter.Table(DbConstants.AppFamiliesLocalized).AddColumn(nameof(AppFamilyLocalized.Details)).AsString().Nullable();
        Alter.Table(DbConstants.Apps).AddColumn(nameof(App.Order)).AsInt32().NotNullable().WithDefaultValue(0);
        Alter.Table(DbConstants.AppInstallers).AddColumn(nameof(AppInstaller.Order)).AsInt32().NotNullable().WithDefaultValue(0);
    }

    public override void Down()
    {
        Delete.Column(nameof(AppInstaller.Order)).FromTable(DbConstants.AppInstallers);
        Delete.Column(nameof(App.Order)).FromTable(DbConstants.Apps);
        Delete.Column(nameof(AppFamilyLocalized.Details)).FromTable(DbConstants.AppFamiliesLocalized);
        Delete.Column(nameof(AppFamily.Details)).FromTable(DbConstants.AppFamilies);
    }
}
