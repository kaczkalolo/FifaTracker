# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["backend/src/FifaTracker.Domain/FifaTracker.Domain.csproj", "backend/src/FifaTracker.Domain/"]
COPY ["backend/src/FifaTracker.Application/FifaTracker.Application.csproj", "backend/src/FifaTracker.Application/"]
COPY ["backend/src/FifaTracker.Infrastructure/FifaTracker.Infrastructure.csproj", "backend/src/FifaTracker.Infrastructure/"]
COPY ["backend/src/FifaTracker.WebApi/FifaTracker.WebApi.csproj", "backend/src/FifaTracker.WebApi/"]

RUN dotnet restore "backend/src/FifaTracker.WebApi/FifaTracker.WebApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/backend/src/FifaTracker.WebApi"
RUN dotnet build "FifaTracker.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FifaTracker.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FifaTracker.WebApi.dll"]
