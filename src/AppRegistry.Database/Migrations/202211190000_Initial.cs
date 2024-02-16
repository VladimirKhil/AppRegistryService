using AppRegistry.Database.Models;
using FluentMigrator;

namespace AppRegistry.Database.Migrations;

[Migration(202211190000, "Initial migration")]
public sealed class Initial : Migration
{
    private const string AppReleasesConstraintName = $"UC_{DbConstants.AppReleases}";
    private const string AppErrorsConstraintName = $"UC_{DbConstants.AppErrors}";
    private const string AppRunsConstraintName = $"UC_{DbConstants.AppRuns}";

    public override void Up()
    {
        Create.Table(DbConstants.Languages)
            .WithColumn(nameof(Language.Id)).AsInt32().PrimaryKey()
            .WithColumn(nameof(Language.Code)).AsString().Unique().NotNullable();

        Create.Table(DbConstants.AppFamilies)
            .WithColumn(nameof(AppFamily.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(AppFamily.Name)).AsString().Unique().NotNullable()
            .WithColumn(nameof(AppFamily.Description)).AsString().Nullable()
            .WithColumn(nameof(AppFamily.LogoUri)).AsString().Nullable();

        Create.Table(DbConstants.AppFamiliesLocalized)
            .WithColumn(nameof(AppFamilyLocalized.FamilyId)).AsGuid().PrimaryKey().ForeignKey(nameof(DbConstants.AppFamilies), nameof(AppFamily.Id))
            .WithColumn(nameof(AppFamilyLocalized.LanguageId)).AsInt32().PrimaryKey().ForeignKey(nameof(DbConstants.Languages), nameof(Language.Id))
            .WithColumn(nameof(AppFamilyLocalized.Description)).AsString().Nullable();

        Create.Table(DbConstants.Apps)
            .WithColumn(nameof(App.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(App.FamilyId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.AppFamilies), nameof(AppFamily.Id))
            .WithColumn(nameof(App.Name)).AsString().Unique().NotNullable()
            .WithColumn(nameof(App.KnownIssues)).AsString().Nullable();

        Create.Table(DbConstants.AppsLocalized)
            .WithColumn(nameof(AppLocalized.AppId)).AsGuid().PrimaryKey().ForeignKey(nameof(DbConstants.Apps), nameof(App.Id))
            .WithColumn(nameof(AppLocalized.LanguageId)).AsInt32().PrimaryKey().ForeignKey(nameof(DbConstants.Languages), nameof(Language.Id))
            .WithColumn(nameof(AppLocalized.KnownIssues)).AsString().Nullable();

        Create.Table(DbConstants.AppScreenshots)
            .WithColumn(nameof(AppScreenshot.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(AppScreenshot.AppId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.Apps), nameof(App.Id))
            .WithColumn(nameof(AppScreenshot.ScreentshotUri)).AsString().NotNullable();

        Create.Table(DbConstants.AppReleases)
            .WithColumn(nameof(AppRelease.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(AppRelease.AppId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.Apps), nameof(App.Id))
            .WithColumn(nameof(AppRelease.Version)).AsInt32().NotNullable()
            .WithColumn(nameof(AppRelease.MinimumOSVersion)).AsInt32().NotNullable()
            .WithColumn(nameof(AppRelease.PublishDate)).AsDate().NotNullable()
            .WithColumn(nameof(AppRelease.Notes)).AsString().Nullable()
            .WithColumn(nameof(AppRelease.Level)).AsInt16().NotNullable();

        Create.Table(DbConstants.AppReleasesLocalized)
            .WithColumn(nameof(AppReleaseLocalized.ReleaseId)).AsGuid().PrimaryKey().ForeignKey(nameof(DbConstants.AppReleases), nameof(AppRelease.Id))
            .WithColumn(nameof(AppReleaseLocalized.LanguageId)).AsInt32().PrimaryKey().ForeignKey(nameof(DbConstants.Languages), nameof(Language.Id))
            .WithColumn(nameof(AppReleaseLocalized.Notes)).AsString().Nullable();

        Create.Table(DbConstants.AppInstallers)
            .WithColumn(nameof(AppInstaller.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(AppInstaller.ReleaseId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.AppReleases), nameof(AppRelease.Id))
            .WithColumn(nameof(AppInstaller.Uri)).AsString().NotNullable()
            .WithColumn(nameof(AppInstaller.Title)).AsString().Nullable()
            .WithColumn(nameof(AppInstaller.Description)).AsString().Nullable()
            .WithColumn(nameof(AppInstaller.Size)).AsInt64().NotNullable()
            .WithColumn(nameof(AppInstaller.AdditionalSize)).AsInt64().Nullable();

        Create.Table(DbConstants.AppInstallersLocalized)
            .WithColumn(nameof(AppInstallerLocalized.InstallerId)).AsGuid().PrimaryKey().ForeignKey(nameof(DbConstants.AppInstallers), nameof(AppInstaller.Id))
            .WithColumn(nameof(AppInstallerLocalized.LanguageId)).AsInt32().PrimaryKey().ForeignKey(nameof(DbConstants.Languages), nameof(Language.Id))
            .WithColumn(nameof(AppInstallerLocalized.Title)).AsString().Nullable()
            .WithColumn(nameof(AppInstallerLocalized.Description)).AsString().Nullable();

        Create.Table(DbConstants.AppRuns)
            .WithColumn(nameof(AppRun.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(AppRun.ReleaseId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.AppReleases), nameof(AppRelease.Id))
            .WithColumn(nameof(AppRun.OSVersion)).AsInt32().Nullable()
            .WithColumn(nameof(AppRun.OSArhitecture)).AsInt16().Nullable()
            .WithColumn(nameof(AppRun.Date)).AsDate().NotNullable()
            .WithColumn(nameof(AppRun.Count)).AsInt32().NotNullable();

        Create.Table(DbConstants.AppErrors)
            .WithColumn(nameof(AppError.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(AppError.ReleaseId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.AppReleases), nameof(AppRelease.Id))
            .WithColumn(nameof(AppError.OSVersion)).AsInt32().NotNullable()
            .WithColumn(nameof(AppError.OSArhitecture)).AsInt16().Nullable()
            .WithColumn(nameof(AppError.Time)).AsDateTime2().NotNullable()
            .WithColumn(nameof(AppError.Message)).AsString().NotNullable()
            .WithColumn(nameof(AppError.Status)).AsInt16().NotNullable()
            .WithColumn(nameof(AppError.Count)).AsInt32().NotNullable();

        Create.UniqueConstraint(AppReleasesConstraintName)
            .OnTable(DbConstants.AppReleases)
            .Columns(nameof(AppRelease.AppId), nameof(AppRelease.Version));

        Create.UniqueConstraint(AppErrorsConstraintName)
            .OnTable(DbConstants.AppErrors)
            .Columns(nameof(AppError.ReleaseId), nameof(AppError.OSVersion), nameof(AppError.OSArhitecture), nameof(AppError.Message));

        Create.UniqueConstraint(AppRunsConstraintName)
            .OnTable(DbConstants.AppRuns)
            .Columns(nameof(AppRun.ReleaseId), nameof(AppRun.OSVersion), nameof(AppRun.OSArhitecture), nameof(AppRun.Date));
    }

    public override void Down()
    {
        Delete.UniqueConstraint(AppRunsConstraintName);
        Delete.UniqueConstraint(AppErrorsConstraintName);
        Delete.UniqueConstraint(AppReleasesConstraintName);

        Delete.Table(DbConstants.AppErrors);
        Delete.Table(DbConstants.AppRuns);
        Delete.Table(DbConstants.AppInstallers);
        Delete.Table(DbConstants.AppReleases);
        Delete.Table(DbConstants.AppScreenshots);
        Delete.Table(DbConstants.Apps);
        Delete.Table(DbConstants.AppFamilies);
    }
}
