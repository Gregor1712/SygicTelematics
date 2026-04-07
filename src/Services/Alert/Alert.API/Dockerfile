FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/Shared.Kernel/Shared.Kernel.csproj src/Shared/Shared.Kernel/
COPY src/Shared/Shared.Grpc/Shared.Grpc.csproj src/Shared/Shared.Grpc/
COPY src/Services/Alert/Alert.Application/Alert.Application.csproj src/Services/Alert/Alert.Application/
COPY src/Services/Alert/Alert.Infrastructure/Alert.Infrastructure.csproj src/Services/Alert/Alert.Infrastructure/
COPY src/Services/Alert/Alert.API/Alert.API.csproj src/Services/Alert/Alert.API/
RUN dotnet restore src/Services/Alert/Alert.API/Alert.API.csproj

COPY src/Shared/ src/Shared/
COPY src/Services/Alert/ src/Services/Alert/
RUN dotnet publish src/Services/Alert/Alert.API/Alert.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/src/Services/Alert/Alert.API/Csv ./Csv
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Alert.API.dll"]
