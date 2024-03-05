using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines an application release information.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppReleases)]
public sealed class AppRelease
{
    /// <summary>
    /// Unique release identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid Id { get; set; }

    /// <summary>
    /// Application identifier link.
    /// </summary>
    [Column(DataType = DataType.Guid), NotNull]
    public Guid AppId { get; set; }

    /// <summary>
    /// Release version.
    /// </summary>
    [Column(DataType = DataType.Int32), NotNull]
    public int Version { get; set; }

    /// <summary>
    /// Minimum supported operating system version.
    /// </summary>
    [Column(DataType = DataType.Int32), NotNull]
    public int MinimumOSVersion { get; set; }

    /// <summary>
    /// Release publish date.
    /// </summary>
    [Column(DataType = DataType.Date), NotNull]
    public DateOnly? PublishDate { get; set; }

    /// <summary>
    /// Release notes.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Notes { get; set; }

    /// <summary>
    /// Release level.
    /// </summary>
    [Column(DataType = DataType.Int16), NotNull]
    public ReleaseLevel Level { get; set; }

    /// <summary>
    /// Is this release mandatory.
    /// </summary>
    [Column(DataType = DataType.Boolean), NotNull]
    public bool IsMandatory { get; set; }
}
