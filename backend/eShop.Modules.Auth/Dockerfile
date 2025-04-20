FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["eShop.Modules.Auth/eShop.Modules.Auth.csproj", "eShop.Modules.Auth/"]
RUN dotnet restore "eShop.Modules.Auth/eShop.Modules.Auth.csproj"
COPY . .
WORKDIR "/src/eShop.Modules.Auth"
RUN dotnet build "eShop.Modules.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "eShop.Modules.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eShop.Modules.Auth.dll"]
