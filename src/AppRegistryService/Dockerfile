FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/AppRegistryService/AppRegistryService.csproj", "src/AppRegistryService/"]
COPY ["src/AppRegistry.Database/AppRegistry.Database.csproj", "src/AppRegistry.Database/"]
COPY ["src/AppRegistryService.Contract/AppRegistryService.Contract.csproj", "src/AppRegistryService.Contract/"]
RUN dotnet restore "./src/AppRegistryService/AppRegistryService.csproj" -r linux-musl-x64
COPY ["src/", "src/"]
WORKDIR "/src/src/AppRegistryService"
RUN dotnet publish "./AppRegistryService.csproj" -p:PublishSingleFile=true -r linux-musl-x64 --self-contained true -p:PublishTrimmed=true -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./AppRegistryService", "--urls", "http://*:5000"]