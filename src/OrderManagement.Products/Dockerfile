FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderManagement.Products.API/OrderManagement.Products.API.csproj", "OrderManagement.Products.API/"]
RUN dotnet restore "OrderManagement.Products.API/OrderManagement.Products.API.csproj"
COPY . .
WORKDIR "/src/OrderManagement.Products.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.Products.API.dll"]
