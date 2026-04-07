FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Shared/Shared.Grpc/Shared.Grpc.csproj src/Shared/Shared.Grpc/
COPY src/Services/Vehicle/Vehicle.Application/Vehicle.Application.csproj src/Services/Vehicle/Vehicle.Application/
COPY src/Services/Vehicle/Vehicle.Infrastructure/Vehicle.Infrastructure.csproj src/Services/Vehicle/Vehicle.Infrastructure/
COPY src/Services/Vehicle/Vehicle.API/Vehicle.API.csproj src/Services/Vehicle/Vehicle.API/
RUN dotnet restore src/Services/Vehicle/Vehicle.API/Vehicle.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Vehicle/ src/Services/Vehicle/
RUN dotnet publish src/Services/Vehicle/Vehicle.API/Vehicle.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Vehicle/Vehicle.API/Csv ./Csv
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Vehicle.API.dll"]
