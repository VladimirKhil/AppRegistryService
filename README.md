# AppRegistryService

Provides information about apps and their updates.

Collects apps run and error statistics.

# Build

    dotnet build

# Run

## Docker


    docker run -p 5000:5000 vladimirkhil/appregistryservice:1.0.10


## Helm


    dependencies:
    - name: appregistry
      version: "1.0.2"
      repository: "https://vladimirkhil.github.io/AppRegistryService/helm/repo"