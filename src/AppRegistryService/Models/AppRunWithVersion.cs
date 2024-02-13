using AppRegistry.Database.Models;

namespace AppRegistryService.Models;

public readonly record struct AppRunWithVersion(AppRun Run, int Version);
