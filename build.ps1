param (
    [string]$tag = "latest"
)

docker build . -f src\AppRegistryService\Dockerfile -t vladimirkhil/appregistryservice:$tag