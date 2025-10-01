#!/bin/bash
set -e

echo "Environment: $ASPNETCORE_ENVIRONMENT"

echo "Setup FastTodo database..."
/root/.dotnet/tools/dotnet-ef database update --verbose \
    --project $DB_PROJECT \
    --startup-project ./Host/FastTodo.API/FastTodo.API.csproj \
    --context $CONTEXT \
    --connection "$DB_CONNECTION_STRING"

echo "Setup Identity database..."
/root/.dotnet/tools/dotnet-ef database update --verbose \
    --project $IDENTITY_PROJECT \
    --startup-project ./Host/FastTodo.API/FastTodo.API.csproj \
    --context $IDENTITY_CONTEXT \
    --connection "$IDENTITY_CONNECTION_STRING"

# This command ensures that the script's exit code is correct
exec "$@"