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
    /// Application families.
    /// </summary>
    public ITable<AppFamily> Families => this.GetTable<AppFamily>();

    /// <summary>
    /// Applications.
    /// </summary>
    public ITable<App> Apps => this.GetTable<App>();

    /// <summary>
    /// Application screenshots.
    /// </summary>
    public ITable<AppScreenshot> Screenshots => this.GetTable<AppScreenshot>();

    /// <summary>
    /// Application releases.
    /// </summary>
    public ITable<AppRelease> Releases => this.GetTable<AppRelease>();

    /// <summary>
    /// Applications installers.
    /// </summary>
    public ITable<AppInstaller> Installers => this.GetTable<AppInstaller>();

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
