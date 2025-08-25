#!/bin/bash
set -e

echo "🚀 Starting TeamWorkFlow application..."

# Wait for SQL Server to be ready
echo "⏳ Waiting for SQL Server to be ready..."
until /opt/mssql-tools/bin/sqlcmd -S teamworkflow-db -U sa -P YourStrong@Passw0rd -Q "SELECT 1" > /dev/null 2>&1; do
    echo "SQL Server is unavailable - sleeping for 2 seconds"
    sleep 2
done

echo "✅ SQL Server is ready!"

# Run the application
echo "🎯 Starting .NET application..."
exec dotnet TeamWorkFlow.dll
