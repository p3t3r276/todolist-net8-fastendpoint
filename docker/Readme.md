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
### 1. Setup hostnames in enviroments
```bash
# macOS / Linux
sudo sh -c 'printf "127.0.0.1 mongo1\n127.0.0.1 mongo2\n127.0.0.1 mongo3\n" >> /etc/hosts'
```

On `Windows`, edit C:\Windows\System32\drivers\etc\hosts as Administrator and add:
```
127.0.0.1   mongo1
127.0.0.1   mongo2
127.0.0.1   mongo3
```

#### MongoDB replica set
`components/mongo/mongo.yml` runs a 3-node replica set (`rs0`). Each node listens on its own port, and that port is published on the host with the same number. This means the member addresses work both inside the Docker network and from your machine through the hosts entries above:

| Node   | Port  | Replica set member | Priority |
|--------|-------|--------------------|----------|
| mongo1 | 27017 | `mongo1:27017`     | 2 (preferred primary) |
| mongo2 | 27018 | `mongo2:27018`     | 1 |
| mongo3 | 27019 | `mongo3:27019`     | 1 |

The `mongo-init` service waits for all three nodes to pass their healthchecks, then:
- runs `rs.initiate` only if the set reports `NotYetInitialized`, and fails on any other error;
- if the set already exists, checks that its members match the expected hosts, and **exits with an error** if they don't;
- waits up to 60s for a primary to be elected, so services that depend on `mongo-init` (`service_completed_successfully`) can write right away.

Ports are bound to `127.0.0.1` only, so the replica set can't be reached from other machines on your network.

Connection string (same for the API container and for local tools):
```
mongodb://<MONGO_ROOT_USER>:<MONGO_ROOT_PASSWORD>@mongo1:27017,mongo2:27018,mongo3:27019/?replicaSet=rs0&authSource=admin
```

> If you previously ran the replica set with all members on `27017`, `mongo-init` will fail with a "members do not match" error. Recreate the Mongo volumes (`docker compose -f <file> down -v`), which **deletes Mongo data**, or reconfigure the members with `rs.reconfig(cfg, { force: true })`.

### 2. Update .env file
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

MONGO_ROOT_USER=
MONGO_ROOT_PASSWORD=
# mongodb://<user>:<password>@mongo1:27017,mongo2:27018,mongo3:27019/?replicaSet=rs0&authSource=admin
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

docker compose -f dev-postgres.yml down --rmi local --volumes
```
