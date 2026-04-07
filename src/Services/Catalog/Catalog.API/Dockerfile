FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Services/Catalog/Catalog.Application/Catalog.Application.csproj src/Services/Catalog/Catalog.Application/
COPY src/Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj src/Services/Catalog/Catalog.Infrastructure/
COPY src/Services/Catalog/Catalog.API/Catalog.API.csproj src/Services/Catalog/Catalog.API/
RUN dotnet restore src/Services/Catalog/Catalog.API/Catalog.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Catalog/ src/Services/Catalog/
RUN dotnet publish src/Services/Catalog/Catalog.API/Catalog.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Catalog/Catalog.API/Csv ./Csv
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
