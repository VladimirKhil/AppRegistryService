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

    public async Task<AppFamily[]> GetFamiliesAsync(string language, CancellationToken cancellationToken)
    {
        var query = from f in _connection.Families
                    from l in _connection.Languages.Where(l => l.Code == language).DefaultIfEmpty()
                    from fl in _connection.FamiliesLocalized.Where(fl => fl.FamilyId == f.Id && fl.LanguageId == l.Id).DefaultIfEmpty()
                    select new { Family = f, Localization = fl };

        return await query.Select(r => r.Localization == null
            ? r.Family
            : new AppFamily { Id = r.Family.Id, Name = r.Family.Name, Description = r.Localization.Description, LogoUri = r.Family.LogoUri })
            .ToArrayAsync(cancellationToken);
    }

    public async Task<App[]> GetFamilyAppsAsync(Guid appFamilyId, string language, CancellationToken cancellationToken)
    {
        var query = from a in _connection.Apps.Where(app => app.FamilyId == appFamilyId)
                    from l in _connection.Languages.Where(l => l.Code == language).DefaultIfEmpty()
                    from al in _connection.AppsLocalized.Where(al => al.AppId == a.Id && al.LanguageId == l.Id).DefaultIfEmpty()
                    select new { App = a, Localization = al };

        var apps = await query.Select(r => r.Localization == null
            ? r.App
            : new App { Id = r.App.Id, Name = r.App.Name, FamilyId = r.App.FamilyId, KnownIssues = r.Localization.KnownIssues })
            .ToArrayAsync(cancellationToken);

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
