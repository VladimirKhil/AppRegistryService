using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines a localized application release information.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppReleasesLocalized)]
public sealed class AppReleaseLocalized
{
    /// <summary>
    /// Unique release identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid ReleaseId { get; set; }

    /// <summary>
    /// Language id.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int LanguageId { get; set; }

    /// <summary>
    /// Release notes.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Notes { get; set; }
}
