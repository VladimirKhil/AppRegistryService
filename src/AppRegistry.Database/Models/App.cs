using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines an application.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.Apps)]
public sealed class App
{
    /// <summary>
    /// Application unique identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid Id { get; set; }

    /// <summary>
    /// Application family identifier link.
    /// </summary>
    [Column(DataType = DataType.Guid), NotNull]
    public Guid AppFamilyId { get; set; }

    /// <summary>
    /// Application name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? Name { get; set; }

    /// <summary>
    /// Application known issues info.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? KnownIssues { get; set; }
}
