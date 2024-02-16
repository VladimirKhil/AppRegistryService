using AppRegistry.Database.Models;
using LinqToDB;
using LinqToDB.Data;

namespace AppRegistry.Database;

/// <summary>
/// Defines a database context.
/// </summary>
public sealed class AppRegistryDbConnection : DataConnection
{
    /// <summary>
    /// Languages.
    /// </summary>
    public ITable<Language> Languages => this.GetTable<Language>();

    /// <summary>
    /// Application families.
    /// </summary>
    public ITable<AppFamily> Families => this.GetTable<AppFamily>();

    /// <summary>
    /// Application localized families.
    /// </summary>
    public ITable<AppFamilyLocalized> FamiliesLocalized => this.GetTable<AppFamilyLocalized>();

    /// <summary>
    /// Applications.
    /// </summary>
    public ITable<App> Apps => this.GetTable<App>();

    /// <summary>
    /// Localized applications.
    /// </summary>
    public ITable<AppLocalized> AppsLocalized => this.GetTable<AppLocalized>();

    /// <summary>
    /// Application screenshots.
    /// </summary>
    public ITable<AppScreenshot> Screenshots => this.GetTable<AppScreenshot>();

    /// <summary>
    /// Application releases.
    /// </summary>
    public ITable<AppRelease> Releases => this.GetTable<AppRelease>();

    /// <summary>
    /// Localized application releases.
    /// </summary>
    public ITable<AppReleaseLocalized> ReleasesLocalized => this.GetTable<AppReleaseLocalized>();

    /// <summary>
    /// Applications installers.
    /// </summary>
    public ITable<AppInstaller> Installers => this.GetTable<AppInstaller>();

    /// <summary>
    /// Localized applications installers.
    /// </summary>
    public ITable<AppInstallerLocalized> InstallersLocalized => this.GetTable<AppInstallerLocalized>();

    /// <summary>
    /// Applications run statistic.
    /// </summary>
    public ITable<AppRun> Runs => this.GetTable<AppRun>();

    /// <summary>
    /// Application errors.
    /// </summary>
    public ITable<AppError> Errors => this.GetTable<AppError>();

    public AppRegistryDbConnection(DataOptions options) : base(options) { }
}
