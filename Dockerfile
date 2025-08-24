# Use the official .NET 6.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET 6.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files
COPY ["TeamWorkFlow/TeamWorkFlow.csproj", "TeamWorkFlow/"]
COPY ["TeamWorkFlow.Core/TeamWorkFlow.Core.csproj", "TeamWorkFlow.Core/"]
COPY ["TeamWorkFlow.Infrastructure/TeamWorkFlow.Infrastructure.csproj", "TeamWorkFlow.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "TeamWorkFlow/TeamWorkFlow.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/TeamWorkFlow"
RUN dotnet build "TeamWorkFlow.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TeamWorkFlow.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage - runtime image
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "TeamWorkFlow.dll"]
