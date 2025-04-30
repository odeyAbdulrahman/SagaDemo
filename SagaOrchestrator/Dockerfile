# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Stage 2: Build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SagaOrchestrator/SagaOrchestrator.csproj", "SagaOrchestrator/"]
COPY ["SharedContracts/SharedContracts.csproj", "SharedContracts/"]
RUN dotnet restore "SagaOrchestrator/SagaOrchestrator.csproj"

COPY . .
WORKDIR "/src/SagaOrchestrator"
RUN dotnet build "SagaOrchestrator.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
WORKDIR "/src/SagaOrchestrator"
RUN dotnet publish "SagaOrchestrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SagaOrchestrator.dll"]