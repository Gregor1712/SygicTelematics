FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Services/Identity/Identity.Application/Identity.Application.csproj src/Services/Identity/Identity.Application/
COPY src/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj src/Services/Identity/Identity.Infrastructure/
COPY src/Services/Identity/Identity.API/Identity.API.csproj src/Services/Identity/Identity.API/
RUN dotnet restore src/Services/Identity/Identity.API/Identity.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Identity/ src/Services/Identity/
RUN dotnet publish src/Services/Identity/Identity.API/Identity.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Identity/Identity.Infrastructure/Data/UserSeedData.json ./Data/UserSeedData.json
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Identity.API.dll"]
