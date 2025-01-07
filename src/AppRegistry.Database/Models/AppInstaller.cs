using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines an application release installer.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppInstallers)]
public sealed class AppInstaller
{
    /// <summary>
    /// Unique installer identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid Id { get; set; }

    /// <summary>
    /// Release identifier link.
    /// </summary>
    [Column(DataType = DataType.Guid), NotNull]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Application installer order.
    /// </summary>
    [Column(DataType = DataType.Int32), NotNull]
    public int Order { get; set; }

    /// <summary>
    /// Installer file uri.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? Uri { get; set; }

    /// <summary>
    /// Installer title.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Title { get; set; }

    /// <summary>
    /// Installer description.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Description { get; set; }

    /// <summary>
    /// Installer file size in bytes.
    /// </summary>
    [Column(DataType = DataType.Int64), NotNull]
    public long? Size { get; set; }

    /// <summary>
    /// Installer additional (optional) files size in bytes.
    /// </summary>
    [Column(DataType = DataType.Int64), Nullable]
    public long? AdditionalSize { get; set; }
}
