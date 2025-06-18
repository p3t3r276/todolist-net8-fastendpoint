# Run Docker Containers

## For developement

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
