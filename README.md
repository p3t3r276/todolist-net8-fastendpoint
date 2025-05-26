# todolist-net8-fastendpoint
A project to test the capacity of EF Core and integrating to ASP.NET Core project

### Technologies
- Databases
    - SQL Server
    - SQLite
    - [Coming] MongoDB

- BackEnd
    - .NET `8.0`
    - Minimal API
    - FastEndpoints `5.34.0`
    - Entity Framework Core `9.0.0`
    - MediatR `12.4.1`
    - [Coming] FluentValidation `12.0.0`
    - [Coming] Docker
    - [Coming] ASP.NET Core Identity
- Frontend
    - Angular

## Features Checklist
1. Basic todo list functions 
    - New item
    - Edit item by id
    - Get all items
    - Get item by id
    - Delete item by id
2. Add validations
3. Console App: Todo data seeding
3. Dockerization
4. Implement generic repotory pattern
5. Feature: Implement User function
    - User table
    - Link Users with TodoItems
6. Todo item assignments
7. Implement ASP.NET Core Identity
8. [Testing microcservice] move Identity tables to another database

## Getting started
### Set up environments 
#### ```Docker```:
```
docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=MyPass@word' \
    -p 1433:1433 \
    -d mcr.microsoft.com/mssql/server:2022-latest

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyPass@word" \
   -p 1433:1433 --name sql2022 --hostname sql2022 --platform linux/amd64 \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

#### ```Podman```:
```
podman run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=MyPass@word' \
    -p 1433:1433 \
    -d mcr.microsoft.com/mssql/server:2022-latest

podman run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyPass@word" \
   -p 1433:1433 --name sql2022 --hostname sql2022 --platform linux/amd64 \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

### Run Migrations
#### SQLite
```
dotnet ef migrations add Initial 
    --project .\backend\Persistence\SQLite\FastTodo.Persistence.SQLite.csproj \ 
    --startup-project .\backend\FastTodo.API\FastTodo.API.csproj \
    --context FastTodoSqliteDbContext -o "Data/Todo/Migrations" 
```

#### SQL Server
```
dotnet ef migrations add Initial 
    --project .\backend\Persistence\SQLite\FastTodo.Persistence.SQL.csproj \ 
    --startup-project .\backend\FastTodo.API\FastTodo.API.csproj \
    --context FastTodoSqliteDbContext -o "Data/Todo/Migrations" 
```

### Update database
```
dotnet ef database update \
    --project ./Persistent/EF/EFCoreTest.Persistent.EF.csproj \ 
    --startup-project ./EFCoreTest.API/EFCoreTest.API.csproj  \
    --context TodoDbContext
```
