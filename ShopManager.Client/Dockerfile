# Build stage for the client
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS client-build
WORKDIR /src
COPY ShopManager.Client/ShopManager.Client.csproj .
RUN dotnet restore ShopManager.Client.csproj
COPY ShopManager.Client/ .
RUN dotnet build ShopManager.Client.csproj -c Release -o /app/build

# Publish stage for the client
FROM client-build AS client-publish
RUN dotnet publish ShopManager.Client.csproj -c Release -o /app/publish

# Build stage for the web
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS web-build
WORKDIR /src
COPY ShopManager.Web/ShopManager.Web.csproj .
RUN dotnet restore ShopManager.Web.csproj
COPY ShopManager.Web/ .
RUN dotnet build ShopManager.Web.csproj -c Release -o /app/build

# Publish stage for the web
FROM web-build AS web-publish
RUN dotnet publish ShopManager.Web.csproj -c Release -o /app/publish

# Final stage with nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy static files from client
COPY --from=client-publish /app/publish/wwwroot .

# Copy nginx.conf
COPY ShopManager.Client/nginx.conf /etc/nginx/nginx.conf

# Expose the necessary port(s) for nginx
EXPOSE 80

# Command to start nginx
CMD ["nginx", "-g", "daemon off;"]
