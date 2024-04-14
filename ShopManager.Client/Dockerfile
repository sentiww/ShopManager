FROM mcr.microsoft.com/dotnet/sdk:8.0 AS client-build
WORKDIR /src

COPY ShopManager.Common/ShopManager.Common.csproj .
RUN dotnet restore ShopManager.Common.csproj

COPY ShopManager.Common/ .

COPY ShopManager.Client/ShopManager.Client.csproj .
RUN dotnet restore ShopManager.Client.csproj

COPY ShopManager.Client/ .

RUN dotnet build ShopManager.Client.csproj -c Release -o /app/build

FROM client-build AS client-publish
RUN dotnet publish ShopManager.Client.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=client-publish /app/publish/wwwroot .

COPY ShopManager.Client/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
