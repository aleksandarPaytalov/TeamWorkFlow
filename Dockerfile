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

# Install SQL Server tools for health checks
USER root
RUN apt-get update && apt-get install -y curl gnupg2 \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Copy entrypoint script
COPY docker-entrypoint.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

# Start the application with entrypoint script
ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
