FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Shared/Shared.Grpc/Shared.Grpc.csproj src/Shared/Shared.Grpc/
COPY src/ApiGateway/Gateway.API/Gateway.API.csproj src/ApiGateway/Gateway.API/
RUN dotnet restore src/ApiGateway/Gateway.API/Gateway.API.csproj

COPY src/Shared/ src/Shared/
COPY src/ApiGateway/Gateway.API/ src/ApiGateway/Gateway.API/
RUN dotnet publish src/ApiGateway/Gateway.API/Gateway.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Gateway.API.dll"]
