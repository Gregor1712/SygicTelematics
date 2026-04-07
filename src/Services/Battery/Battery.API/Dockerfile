FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Shared/Shared.Grpc/Shared.Grpc.csproj src/Shared/Shared.Grpc/
COPY src/Services/Battery/Battery.Application/Battery.Application.csproj src/Services/Battery/Battery.Application/
COPY src/Services/Battery/Battery.Infrastructure/Battery.Infrastructure.csproj src/Services/Battery/Battery.Infrastructure/
COPY src/Services/Battery/Battery.API/Battery.API.csproj src/Services/Battery/Battery.API/
RUN dotnet restore src/Services/Battery/Battery.API/Battery.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Battery/ src/Services/Battery/
RUN dotnet publish src/Services/Battery/Battery.API/Battery.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Battery/Battery.API/Csv ./Csv
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Battery.API.dll"]
