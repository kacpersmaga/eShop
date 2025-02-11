FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["eShop.IntegrationTests/eShop.IntegrationTests.csproj", "eShop.IntegrationTests/"]
COPY ["eShop/eShop.csproj", "eShop/"]
RUN dotnet restore "eShop.IntegrationTests/eShop.IntegrationTests.csproj"
COPY . .
WORKDIR "/src/eShop.IntegrationTests"
RUN dotnet build "eShop.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "eShop.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eShop.IntegrationTests.dll"]
