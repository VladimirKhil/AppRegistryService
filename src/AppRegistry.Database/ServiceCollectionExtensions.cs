using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data.RetryPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppRegistry.Database;

/// <summary>
/// Provides a <see cref="IServiceCollection" /> extension that allows to register a AppRegistry database.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppRegistry database to service collection.
    /// </summary>
    public static void AddAppRegistryDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "AppRegistry")
    {
        var dbConnectionString = configuration.GetConnectionString(connectionStringName);

        services.AddLinqToDBContext<AppRegistryDbConnection>((provider, options) =>
            options
                .UsePostgreSQL(dbConnectionString)
                .UseRetryPolicy(new TransientRetryPolicy())
                .UseDefaultLogging(provider));
    }
}
