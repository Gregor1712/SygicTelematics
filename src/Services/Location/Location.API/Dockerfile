FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Shared/Shared.Grpc/Shared.Grpc.csproj src/Shared/Shared.Grpc/
COPY src/Services/Location/Location.Application/Location.Application.csproj src/Services/Location/Location.Application/
COPY src/Services/Location/Location.Infrastructure/Location.Infrastructure.csproj src/Services/Location/Location.Infrastructure/
COPY src/Services/Location/Location.API/Location.API.csproj src/Services/Location/Location.API/
RUN dotnet restore src/Services/Location/Location.API/Location.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Location/ src/Services/Location/
RUN dotnet publish src/Services/Location/Location.API/Location.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Location/Location.API/Csv ./Csv
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Location.API.dll"]
