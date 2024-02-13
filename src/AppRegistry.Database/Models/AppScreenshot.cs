using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines an application screenshot.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppScreenshots)]
public sealed class AppScreenshot
{
    /// <summary>
    /// Screenshot unique identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int Id { get; set; }

    /// <summary>
    /// Application identifier link.
    /// </summary>
    [Column(DataType = DataType.Guid), NotNull]
    public Guid AppId { get; set; }

    /// <summary>
    /// Screenshot uri.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string ScreentshotUri { get; set; } = "";
}
