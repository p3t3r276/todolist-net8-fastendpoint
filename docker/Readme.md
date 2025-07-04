# Run Docker Containers

## For developement
### Setup SQLite
```bash
dotnet ef migrations add Remove_StartEndDate \
    --project ./src/Persistence/SQLite/FastTodo.Persistence.SQLite.csproj \
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSqliteDbContext -o "Data/Todo/Migrations"
  
dotnet ef database update \
    --project ./src/Persistence/SQLite/FastTodo.Persistence.SQLite.csproj \
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSqliteDbContext
```

## Deploy database with API
1. Update .env file
```env
DB_CONNECTION_STRING=
PROJECT=
CONTEXT=
SqlProvider=
ASPNETCORE_ENVIRONMENT=
```
or run docker-compose file based on database
### For MSSQL
```bash
docker compose -f dev-mssql.yml up -d --build
```

### For Postgres
```bash
docker compose -f dev-postgres.yml up -d --build
```
