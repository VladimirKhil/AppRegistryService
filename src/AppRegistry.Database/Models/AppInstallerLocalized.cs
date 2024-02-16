using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines an application release installer.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppInstallersLocalized)]
public sealed class AppInstallerLocalized
{
    /// <summary>
    /// Unique installer identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid InstallerId { get; set; }

    /// <summary>
    /// Language id.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int LanguageId { get; set; }

    /// <summary>
    /// Installer localized title.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Title { get; set; }

    /// <summary>
    /// Installer localized description.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Description { get; set; }
}
