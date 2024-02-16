using AppRegistryService.Contract.Responses;
using AppRegistryService.Contracts;
using AppRegistryService.Helpers;
using AppRegistryService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AppRegistryService.Controllers;

/// <summary>
/// Provides API to work with application families.
/// </summary>
[Route("api/v1/families")]
[ApiController]
public sealed class FamiliesController : ControllerBase
{
    private readonly IFamiliesService _familiesService;
    private readonly IMapper _mapper;

    public FamiliesController(IFamiliesService familiesService, IMapper mapper)
    {
        _familiesService = familiesService;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all application families.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<AppFamilyInfo[]> GetFamiliesAsync(
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default)
    {
        var families = await _familiesService.GetFamiliesAsync(CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);
        return _mapper.Map<AppFamilyInfo[]>(families);
    }

    /// <summary>
    /// Gets application for an app family.
    /// </summary>
    /// <param name="appFamilyId">Application family identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Applications of the family.</returns>
    [HttpGet("{appFamilyId}/apps")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppInfo[]>> GetFamilyAppsAsync(
        Guid appFamilyId,
        [FromHeader(Name = "Accept-Language")] string acceptLanguage = Constants.DefaultLanguageCode,
        CancellationToken cancellationToken = default)
    {
        var apps = await _familiesService.GetFamilyAppsAsync(appFamilyId, CultureHelper.GetLanguageFromAcceptLanguageHeader(acceptLanguage), cancellationToken);
        var mappedApps = _mapper.Map<AppInfo[]>(apps);

        return Ok(mappedApps);
    }
}
