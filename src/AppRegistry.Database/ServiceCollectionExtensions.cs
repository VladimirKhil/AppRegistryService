using AppRegistry.Database.Migrations;
using AppRegistry.Database.Models;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data.RetryPolicy;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace AppRegistry.Database;

/// <summary>
/// Provides a <see cref="IServiceCollection" /> extension that allows to register a AppRegistry database.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppRegistry database to service collection.
    /// </summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(NpgsqlProviderAdapter.NpgsqlConnection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(NpgsqlDataReader))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Initial))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(AppFamilyLocalized))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(AppLocalized))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(AppInstallerLocalized))]
    public static void AddAppRegistryDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "AppRegistry")
    {
        var dbConnectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException("Database connection is undefined");

        services.AddLinqToDBContext<AppRegistryDbConnection>((provider, options) =>
            options
                .UsePostgreSQL(dbConnectionString)
                .UseRetryPolicy(new TransientRetryPolicy())
                .UseDefaultLogging(provider));
    }
}
