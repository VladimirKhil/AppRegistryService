using LinqToDB;
using LinqToDB.Mapping;
using System.Runtime.InteropServices;

namespace AppRegistry.Database.Models;

/// <summary>
/// Contains information about application error.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppErrors)]
public sealed class AppError
{
    /// <summary>
    /// Unique error id.
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
    [Column(DataType = DataType.Int32), NotNull]
    public int OSVersion { get; set; }

    /// <summary>
    /// Operating system architecture.
    /// </summary>
    [Column(DataType = DataType.Int16), Nullable]
    public Architecture? OSArchitecture { get; set; }

    /// <summary>
    /// Error first appearance time.
    /// </summary>
    [Column(DataType = DataType.DateTimeOffset), NotNull]
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// Error text.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string Message { get; set; } = "";

    /// <summary>
    /// User notes for error.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? UserNotes { get; set; }

    /// <summary>
    /// Current error fix status.
    /// </summary>
    [Column(DataType = DataType.Int16), NotNull]
    public ErrorStatus Status { get; set; }

    /// <summary>
    /// Error counter.
    /// </summary>
    [Column(DataType = DataType.Int32), NotNull]
    public int Count { get; set; }
}
