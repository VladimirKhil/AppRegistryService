using AppRegistry.Database.Models;
using AppRegistryService.Contract.Responses;

namespace AppRegistryService.Helpers;

internal static class Mapper
{
    public static AppFamilyInfo ToAppFamilyInfo(this AppFamily appFamily) => new()
    {
        Id = appFamily.Id,
        Name = appFamily.Name,
        Description = appFamily.Description,
        Details = appFamily.Details,
        LogoUri = appFamily.LogoUri == null ? null : new Uri(appFamily.LogoUri)
    };

    public static AppInfo ToAppInfo(this App app) => new()
    {
        Id = app.Id,
        FamilyId = app.FamilyId,
        Name = app.Name,
        KnownIssues = app.KnownIssues
    };

    public static AppReleaseInfo? ToAppReleaseInfo(this AppRelease? appRelease)
    {
        if (appRelease == null)
        {
            return null;
        }

        return new()
        {
            Id = appRelease.Id,
            AppId = appRelease.AppId,
            Version = VersionHelper.CreateVersion(appRelease.Version),
            MinimumOSVersion = VersionHelper.CreateOSVersion(appRelease.MinimumOSVersion),
            PublishDate = appRelease.PublishDate,
            Notes = appRelease.Notes,
            Level = appRelease.Level.ToContractReleaseLevel(),
            IsMandatory = appRelease.IsMandatory
        };
    }

    public static AppInstallerInfo ToAppInstallerInfo(this AppInstaller appInstaller) => new()
    {
        Id = appInstaller.Id,
        ReleaseId = appInstaller.ReleaseId,
        Order = appInstaller.Order,
        Uri = appInstaller.Uri == null ? null : new Uri(appInstaller.Uri),
        Title = appInstaller.Title,
        Description = appInstaller.Description,
        Size = appInstaller.Size,
        AdditionalSize = appInstaller.AdditionalSize
    };

    private static Contract.Models.ReleaseLevel ToContractReleaseLevel(this ReleaseLevel level) => level switch
    {
        ReleaseLevel.Minor => Contract.Models.ReleaseLevel.Minor,
        ReleaseLevel.Major => Contract.Models.ReleaseLevel.Major,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unknown release level")
    };

    public static ReleaseLevel ToReleaseLevel(this Contract.Models.ReleaseLevel releaseLevel) => releaseLevel switch
    {
        Contract.Models.ReleaseLevel.Minor => ReleaseLevel.Minor,
        Contract.Models.ReleaseLevel.Major => ReleaseLevel.Major,
        _ => throw new ArgumentOutOfRangeException(nameof(releaseLevel), releaseLevel, "Unknown release level")
    };

    public static Contract.Models.ErrorStatus ToContractErrorStatus(this ErrorStatus errorStatus) => errorStatus switch
    {
        ErrorStatus.NotFixed => Contract.Models.ErrorStatus.NotFixed,
        ErrorStatus.Fixed => Contract.Models.ErrorStatus.Fixed,
        ErrorStatus.CannotReproduce => Contract.Models.ErrorStatus.CannotReproduce,
        _ => throw new ArgumentOutOfRangeException(nameof(errorStatus), errorStatus, "Unknown error status")
    };
}
