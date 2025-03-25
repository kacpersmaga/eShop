FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["eShop.Host/eShop.Host.csproj", "eShop.Host/"]
COPY ["eShop.Infrastructure/eShop.Infrastructure.csproj", "eShop.Infrastructure/"]
COPY ["eShop.Modules.Catalog/eShop.Modules.Catalog.csproj", "eShop.Modules.Catalog/"]
COPY ["eShop.Shared/eShop.Shared.csproj", "eShop.Shared/"]
COPY ["eShop.Modules.Auth/eShop.Modules.Auth.csproj", "eShop.Modules.Auth/"]
RUN dotnet restore "eShop.Host/eShop.Host.csproj"
COPY . .
WORKDIR "/src/eShop.Host"
RUN dotnet build "eShop.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "eShop.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eShop.Host.dll"]
