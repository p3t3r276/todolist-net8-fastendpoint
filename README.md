# todolist-net8-fastendpoint
A project demonstrating Clean Architecture with EF Core and ASP.NET Core using FastEndpoints

Table of Contents
=======
* [Technologies & Patterns](#technologies--patterns)
* [Features](#features)
* [API Endpoints](#api-endpoints)
* [Getting Started](#getting-started)
* [Configuration](#configuration)

## Technologies & Patterns
### Database Providers
- SQL Server
- SQLite
- Postgres
- [Upcoming] MongoDB

### Backend Stack
- .NET `8.0`
- FastEndpoints `5.34.0`
- Entity Framework Core `9.0.0`
- MediatR `12.4.1`
- FluentValidation `12.0.0`
- Generic Repository Pattern with Unit of Work
- CQRS Pattern with MediatR

## Features
### Completed ✅
1. Basic Todo Operations
    - Create, Read, Update, Delete (CRUD)
    - List all items with filtering
    - Get item by ID
2. Input Validation using FluentValidation
3. Data Seeding Console Application
4. Generic Repository Pattern Implementation
5. Multiple Database Provider Support (SQL Server, SQLite)
6. API Versioning
7. Unit of Work Pattern

### In Progress 🚧
1. Dockerization

### Planning 📋
1. User Management
    - User Authentication
    - Todo Item Ownership
2. Todo Item Assignments
3. ASP.NET Core Identity Integration
4. Logging

## API Endpoints

| Method | Endpoint        | Description                           |
|--------|----------------|---------------------------------------|
| GET    | /api/todos     | Get all todos with optional filtering |
| GET    | /api/todos/{id}| Get a specific todo by ID            |
| POST   | /api/todos     | Create a new todo                    |
| PUT    | /api/todos/{id}| Update an existing todo             |
| PATCH  | /api/todos/{id}| Update todo status             |
| DELETE | /api/todos/{id}| Delete a todo                       |

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Docker or Podman
- Visual Studio Code or Visual Studio 2022

### Database Setup
#### Option 1: SQL Server with Docker
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyPass@word" \
   -p 1433:1433 --name sql2022 --hostname sql2022 --platform linux/amd64 \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

#### Option 2: SQLite
No additional setup required. The database file will be created automatically.

### Database Migrations

#### For SQLite
```bash
dotnet ef migrations add Initial \
    --project ./src/Persistence/SQLite/FastTodo.Persistence.SQLite.csproj \
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSqliteDbContext -o "Data/Todo/Migrations"

dotnet ef database update \
    --project ./src/Persistence/SQLite/FastTodo.Persistence.SQLite.csproj \
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSqliteDbContext
```

#### For SQL Server
```bash
dotnet ef migrations add Initial \
    --project ./src/Persistence/EF/FastTodo.Persistence.EF.csproj \
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSQLDbContext -o "Data/Todo/Migrations"

dotnet ef database update \
    --project ./src/Persistence/EF/FastTodo.Persistence.EF.csproj \
    --startup-project ./src/FastTodo.API/FastTodo.API.csproj \
    --context FastTodoSQLDbContext
```

### Running the Application
```bash
dotnet run --project src/FastTodo.API/FastTodo.API.csproj
```

### Configuration
The database provider can be configured in `appsettings.json`:
```json
{
  "DatabaseProvider": "Sqlite", // or "SqlServer"
  "ConnectionStrings": {
    "Sqlite": "Data Source=FastTodo.db",
    "SqlServer": ""
  }
}
```