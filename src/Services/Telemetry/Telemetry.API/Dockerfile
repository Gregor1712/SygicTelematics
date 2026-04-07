FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Shared/Shared.Grpc/Shared.Grpc.csproj src/Shared/Shared.Grpc/
COPY src/Services/Telemetry/Telemetry.Application/Telemetry.Application.csproj src/Services/Telemetry/Telemetry.Application/
COPY src/Services/Telemetry/Telemetry.Infrastructure/Telemetry.Infrastructure.csproj src/Services/Telemetry/Telemetry.Infrastructure/
COPY src/Services/Telemetry/Telemetry.API/Telemetry.API.csproj src/Services/Telemetry/Telemetry.API/
RUN dotnet restore src/Services/Telemetry/Telemetry.API/Telemetry.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Telemetry/ src/Services/Telemetry/
RUN dotnet publish src/Services/Telemetry/Telemetry.API/Telemetry.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Telemetry/Telemetry.API/Csv ./Csv
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Telemetry.API.dll"]
