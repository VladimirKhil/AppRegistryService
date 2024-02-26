param (
    [string]$version = "1.0.0",
    [string]$apikey = ""
)

dotnet pack src\AppRegistryService.Contract\AppRegistryService.Contract.csproj -c Release /property:Version=$version
dotnet pack src\AppRegistryService.Client\AppRegistryService.Client.csproj -c Release /property:Version=$version
dotnet nuget push bin\.Release\AppRegistryService.Contract\VKhil.AppRegistry.Contract.$version.nupkg --api-key $apikey --source https://api.nuget.org/v3/index.json
dotnet nuget push bin\.Release\AppRegistryService.Client\VKhil.AppRegistry.Client.$version.nupkg --api-key $apikey --source https://api.nuget.org/v3/index.json