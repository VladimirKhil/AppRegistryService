using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines an localized application.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppsLocalized)]
public sealed class AppLocalized
{
    /// <summary>
    /// Application unique identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid AppId { get; set; }

    /// <summary>
    /// Language id.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int LanguageId { get; set; }

    /// <summary>
    /// Application known issues info.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? KnownIssues { get; set; }
}
