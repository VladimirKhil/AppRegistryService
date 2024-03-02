using LinqToDB;
using LinqToDB.Mapping;
using System.Runtime.InteropServices;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines application run per day statistic.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppRuns)]
public sealed class AppRun
{
    /// <summary>
    /// Unique run id.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int Id { get; set; }

    /// <summary>
    /// Application release identifier.
    /// </summary>
    [Column(DataType = DataType.Guid), NotNull]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Operating system version.
    /// </summary>
    [Column(DataType = DataType.Int32), Nullable]
    public int OSVersion { get; set; }

    /// <summary>
    /// Operating system architecture.
    /// </summary>
    [Column(DataType = DataType.Int16), Nullable]
    public Architecture? OSArchitecture { get; set; }
    
    /// <summary>
    /// Run date.
    /// </summary>
    [Column(DataType = DataType.Date), NotNull]
    public DateOnly Date { get; set; }

    /// <summary>
    /// Run counter.
    /// </summary>
    [Column(DataType = DataType.Int32), NotNull]
    public int Count { get; set; }
}
