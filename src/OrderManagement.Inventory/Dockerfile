FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderManagement.Inventory.API/OrderManagement.Inventory.API.csproj", "OrderManagement.Inventory.API/"]
COPY ["OrderManagement.Inventory.Application/OrderManagement.Inventory.Application.csproj", "OrderManagement.Inventory.Application/"]
COPY ["OrderManagement.Inventory.Core/OrderManagement.Inventory.Core.csproj", "OrderManagement.Inventory.Core/"]
COPY ["OrderManagement.Inventory.Infrastructure/OrderManagement.Inventory.Infrastructure.csproj", "OrderManagement.Inventory.Infrastructure/"]
RUN dotnet restore "OrderManagement.Inventory.API/OrderManagement.Inventory.API.csproj"
COPY . .
WORKDIR "/src/OrderManagement.Inventory.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.Inventory.API.dll"]
