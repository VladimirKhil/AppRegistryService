using AppRegistry.Database;
using AppRegistry.Database.Models;
using AppRegistryService.Contract.Models;
using AppRegistryService.Contracts;
using AppRegistryService.Exceptions;
using LinqToDB;
using System.Net;

namespace AppRegistryService.Services;

public sealed class FamiliesService : IFamiliesService
{
    private readonly AppRegistryDbConnection _connection;

    public FamiliesService(AppRegistryDbConnection connection) => _connection = connection;

    public async Task<AppFamily[]> GetFamiliesAsync(CancellationToken cancellationToken) =>
        await _connection.Families.ToArrayAsync(cancellationToken);

    public async Task<App[]> GetFamilyAppsAsync(Guid appFamilyId, CancellationToken cancellationToken)
    {
        var apps = await _connection.Apps.Where(app => app.AppFamilyId == appFamilyId).ToArrayAsync(cancellationToken);

        if (apps.Length == 0)
        {
            var familyExists = await _connection.Families.AnyAsync(family => family.Id == appFamilyId, cancellationToken);

            if (!familyExists)
            {
                throw new ServiceException(WellKnownAppRegistryServiceErrorCode.AppFamilyNotFound, HttpStatusCode.NotFound);
            }
        }

        return apps;
    }
}
