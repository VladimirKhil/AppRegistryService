using AppRegistry.Database.Models;
using FluentMigrator;

namespace AppRegistry.Database.Migrations;

[Migration(202403050000, "Mandatory releases")]
public sealed class MandatoryReleases : Migration
{
    public override void Up() =>
        Alter.Table(DbConstants.AppReleases).AddColumn(nameof(AppRelease.IsMandatory)).AsBoolean().NotNullable().WithDefaultValue(false);

    public override void Down() => Delete.Column(nameof(AppRelease.IsMandatory)).FromTable(DbConstants.AppReleases);
}
