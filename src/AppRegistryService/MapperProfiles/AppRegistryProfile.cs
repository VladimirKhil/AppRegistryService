using AppRegistry.Database.Models;
using AppRegistryService.Contract.Responses;
using AppRegistryService.Helpers;
using AppRegistryService.Models;
using AutoMapper;

namespace AppRegistryService.MapperProfiles;

/// <summary>
/// Defines an AppRegistry mapper profile.
/// </summary>
internal sealed class AppRegistryProfile : Profile
{
    public AppRegistryProfile()
    {
        CreateMap<AppFamily, AppFamilyInfo>();
        CreateMap<App, AppInfo>();

        CreateMap<AppRelease, AppReleaseInfo>()
            .ForMember(dest => dest.Version, act => act.MapFrom(src => VersionHelper.CreateVersion(src.Version)))
            .ForMember(dest => dest.MinimumOSVersion, act => act.MapFrom(src => VersionHelper.CreateOSVersion(src.MinimumOSVersion)));

        CreateMap<AppInstaller, AppInstallerInfo>();
    }
}
