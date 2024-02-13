using AppRegistry.Database.Models;

namespace AppRegistryService.Models;

public readonly record struct AppErrorWithVersion(AppError Error, int Version);
