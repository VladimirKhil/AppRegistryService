using LinqToDB;
using LinqToDB.Mapping;

namespace AppRegistry.Database.Models;

/// <summary>
/// Defines a language.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.Languages)]
public sealed class Language
{
    /// <summary>
    /// Unique language id.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull, Identity]
    public int Id { get; set; }

    /// <summary>
    /// Language code.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string Code { get; set; } = "";
}
