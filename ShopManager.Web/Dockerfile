FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ShopManager.Web/ShopManager.Web.csproj", "ShopManager.Web/"]
RUN dotnet restore "ShopManager.Web/ShopManager.Web.csproj"
COPY . .
WORKDIR "/src/ShopManager.Web"
RUN dotnet build "ShopManager.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShopManager.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopManager.Web.dll"]
