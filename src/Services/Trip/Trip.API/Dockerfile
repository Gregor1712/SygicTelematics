FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Shared/Shared.Grpc/Shared.Grpc.csproj src/Shared/Shared.Grpc/
COPY src/Services/Trip/Trip.Application/Trip.Application.csproj src/Services/Trip/Trip.Application/
COPY src/Services/Trip/Trip.Infrastructure/Trip.Infrastructure.csproj src/Services/Trip/Trip.Infrastructure/
COPY src/Services/Trip/Trip.API/Trip.API.csproj src/Services/Trip/Trip.API/
RUN dotnet restore src/Services/Trip/Trip.API/Trip.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Trip/ src/Services/Trip/
RUN dotnet publish src/Services/Trip/Trip.API/Trip.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Trip/Trip.API/Csv ./Csv
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Trip.API.dll"]
