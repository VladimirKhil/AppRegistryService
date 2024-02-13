using AppRegistryService.Contract.Models;

namespace AppRegistryService.Contract.Responses;

public sealed record SendAppErrorResponse(ErrorStatus ErrorStatus);
