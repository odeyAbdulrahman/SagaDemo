# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Stage 2: Build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["InventoryService/InventoryService.csproj", "InventoryService/"]
COPY ["SharedContracts/SharedContracts.csproj", "SharedContracts/"]
RUN dotnet restore "InventoryService/InventoryService.csproj"

COPY . .
WORKDIR "/src/InventoryService"
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
WORKDIR "/src/InventoryService"
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]