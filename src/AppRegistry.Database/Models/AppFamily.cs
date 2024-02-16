using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines an application family.
/// Applications in a family share the same name and logo but have separate codebases, releases and supported platforms.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppFamilies)]
public sealed class AppFamily
{
    /// <summary>
    /// Unique application family identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid Id { get; set; }

    /// <summary>
    /// Application family name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string Name { get; set; } = "";

    /// <summary>
    /// Application family description.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Description { get; set; }

    /// <summary>
    /// Application family logo uri.
    /// </summary>
    [Column(DataType = DataType.Image), Nullable]
    public string? LogoUri { get; set; }
}
