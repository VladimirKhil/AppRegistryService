using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines a localized application family.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.AppFamiliesLocalized)]
public sealed class AppFamilyLocalized
{
    /// <summary>
    /// Unique application family identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid FamilyId { get; set; }

    /// <summary>
    /// Language id.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int LanguageId { get; set; }

    /// <summary>
    /// Application family localized description.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? Description { get; set; }
}
