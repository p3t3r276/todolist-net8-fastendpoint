# Run Docker Containers

## For developement
### Setup SQLite
```bash
dotnet ef migrations add Remove_StartEndDate \
    --project ./src/Persistence/Postgres/FastTodo.Persistence.Postgres.csproj \
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoPostgresContext -o "Data/Todo/Migrations"
  
dotnet ef database update `
    --project ./src/Persistence/Postgres/FastTodo.Persistence.Postgres.csproj \
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoPostgresContext --connection "Server=127.0.0.1;Port=5432;Database=fasttodo;User Id=myuser;Password=mypassword;"

dotnet ef database update \
    --project ./src/Persistence/Postgres/FastTodo.Persistence.Postgres.csproj \
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoIdentityDbContext --connection "Server=127.0.0.1;Port=5432;Database=identity;User Id=myuser;Password=mypassword;"
```

```powershell 
dotnet ef migrations add Remove_StartEndDate `
    --project ./src/Persistence/Postgres/FastTodo.Persistence.Postgres.csproj `
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj `
    --context FastTodoPostgresContext -o "Data/Todo/Migrations"
  
dotnet ef database update `
    --project ./src/Persistence/Postgres/FastTodo.Persistence.Postgres.csproj `
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj `
    --context FastTodoPostgresContext --connection "Server=127.0.0.1;Port=5432;Database=fasttodo;User Id=myuser;Password=mypassword;"

dotnet ef database update `
    --project ./src/Persistence/Postgres/FastTodo.Persistence.Postgres.csproj `
    --startup-project ./src/Host/FastTodo.API/FastTodo.API.csproj `
    --context FastTodoIdentityDbContext --connection "Server=127.0.0.1;Port=5432;Database=identity;User Id=myuser;Password=mypassword;"
```

## Deploy database with API
1. Update .env file
```env
DB_CONNECTION_STRING=
DB_PROJECT=
CONTEXT=
IDENTITY_CONNECTION_STRING=
IDENTITY_PROJECT=
IDENTITY_CONTEXT=
SQLProvider=
ASPNETCORE_ENVIRONMENT=

POSTGRES_USER=
POSTGRES_PASSWORD=
POSTGRES_DATABSE=
PGADMIN_DEFAULT_EMAIL=
PGADMIN_DEFAULT_PASSWORD=

REDIS_PASSWORD=
REDIS_DATABASES=
REDIS_PORT=

MONGO_INITDB_ROOT_USERNAME=
MONGO_INITDB_ROOT_PASSWORD=
MONGODB_CONNECTION_STRING=
MONGODB_DATABASE=
MONGODB_PROJECT=
MONGODB_CONTEXT=

ME_CONFIG_MONGODB_SERVER=
ME_CONFIG_MONGODB_ENABLE_ADMIN=
ME_CONFIG_MONGODB_ADMINUSERNAME=
ME_CONFIG_MONGODB_ADMINPASSWORD=
ME_CONFIG_BASICAUTH_USERNAME=
ME_CONFIG_BASICAUTH_PASSWORD=

```
or run docker-compose file based on database

### For local envrironemnt
```bash
docker compose -f dev-mssql.yml up -d --build

docker compose -f dev-mssql.yml down -rmi local --volumes
```

### For Dev environment
```bash
docker compose -f dev-postgres.yml up -d --build

docker compose -f dev-postgres.yml down -rmi local --volumes
```
