using Npgsql;

namespace AppRegistry.Database;

public static class DatabaseExtensions
{
    public static bool EnsureExists(string connectionString, string dbName)
    {
        using var dbConnection = new NpgsqlConnection(connectionString);

        dbConnection.Open();

        var existCmd = dbConnection.CreateCommand();
        existCmd.CommandText = "select count(*) from pg_database where datname = @name";
        existCmd.Parameters.Add(new NpgsqlParameter("name", dbName.ToLowerInvariant()));

        var existed = Convert.ToInt32(existCmd.ExecuteScalar());

        if (existed != 0)
        {
            return false;
        }

        var createCmd = dbConnection.CreateCommand();
        createCmd.CommandText = $"CREATE DATABASE \"{dbName.ToLowerInvariant()}\"";
        createCmd.ExecuteNonQuery();

        return true;
    }
}
