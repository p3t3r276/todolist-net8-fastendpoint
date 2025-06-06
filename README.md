# todolist-net8-fastendpoint
A project to test the capacity of EF Core and integrating to ASP.NET Core project

### Technologies
- Databases
    - SQL Server
    - SQLite
    - [Upcoming] MongoDB

- Backend
    - .NET `8.0`
    - Minimal API
    - FastEndpoints `5.34.0`
    - Entity Framework Core `9.0.0`
    - MediatR `12.4.1`
    - FluentValidation `12.0.0`
    - [Upcoming] Docker
    - [Upcoming] ASP.NET Core Identity\

- Frontend
    - [Upcoming] Angular

## Features Checklist
1. Basic todo list functions ✅
    - New item
    - Edit item by id
    - Get all items
    - Get item by id
    - Delete item by id
2. Add validations ✅
3. Console App: Todo data seeding ✅
3. Dockerization
4. Implement generic repotory pattern ✅
5. Verionsing
6. Feature: Implement User function
    - User table
    - Link Users with TodoItems
7. Todo item assignments
8. Implement ASP.NET Core Identity
9. [Testing microcservice] move Identity tables to another database

## API Endpoints

| Method | Endpoint        | Description                                  |
|--------|-----------------|----------------------------------------------|
| GET    | /api/todos      | Get all todos with optional filtering        |
| GET    | /api/todos/{id} | Get a specific todo by ID                    |
| POST   | /api/todos      | Create a new todo                            |
| PUT    | /api/todos/{id} | Update an existing todo                      |
| DELETE | /api/todos/{id} | Delete a todo                                |

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
dotnet ef migrations add Initial \
    --project ./src/Persistence/SQLite/FastTodo.Persistence.SQLite.csproj \
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSqliteDbContext -o "Data/Todo/Migrations"
```

#### SQL Server
```
dotnet ef migrations add Initial \
    --project ./src/Persistence/EF/FastTodo.Persistence.EF.csproj \ 
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSQLDbContext -o "Data/Todo/Migrations" 
```

### Update database
#### SQLite
```
dotnet ef database update \
    --project ./src/Persistence/SQLite/FastTodo.Persistence.SQLite.csproj \
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSqliteDbContext
```

#### SQL Server
```
dotnet ef database update \
    --project ./src/Persistence/EF/FastTodo.Persistence.EF.csproj \
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSQLDbContext
```

### Run
```
dotnet run --project FastTodo.API/FastTodo.API.csproj
```