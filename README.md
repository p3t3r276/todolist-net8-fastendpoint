# FastTodo
A project demonstrating Clean Architecture with EF Core and ASP.NET Core using FastEndpoints

Table of Contents
=======
* [Technologies & Patterns](#technologies--patterns)
* [Solution Structure](#solution--structure)
* [Features](#features)
* [API Endpoints](#api-endpoints)
* [Getting Started](#getting-started)
* [Configuration](#configuration)

## Technologies & Patterns

### Architecture

<p align='center'>
  <img src='./docs/Architecture.png' />
</p>

#### Component Diagram

<p align='center'>
  <img src='./docs/Architecture-Overview.svg' alt='FastTodo high-level architecture' />
</p>

Requests enter through Nginx and reach the FastTodo.API endpoints. The application layer writes todos to the primary tables of the configured SQL database and reads the user's todo list from MongoDB. Redis caches user data. Identity reads users from the master tables and links them to roles, claims and logins through the bridge tables. The Migrator and SeedData tools prepare the SQL schema and sample data.

The diagram source is [`docs/Architecture-Overview.drawio`](./docs/Architecture-Overview.drawio). Open it in [draw.io](https://app.diagrams.net/) or the VS Code *Draw.io Integration* extension to edit it, then export it over `docs/Architecture-Overview.svg`.

> Only one SQL provider is active at runtime, chosen by `fastTodoOption:SQLProvider` (`SQLServer`, `Postgres` or `SQLite`; the default is SQLite). Redis is used when a `Redis` connection string is set; otherwise an in-memory cache is used.

#### Request Flow

Writes and single-item reads use the SQL database. The "my todos" list is read from MongoDB, and user details come from the Redis cache.

```mermaid
sequenceDiagram
    autonumber
    actor C as Client
    participant N as Nginx
    participant E as Endpoints
    participant A as Application
    participant S as SQL Database
    participant M as MongoDB
    participant R as Redis

    C->>N: HTTP /api/todos
    N->>E: Forward request
    E->>A: Send command / query
    A->>A: Validate request
    alt Create, update, delete, get by id
        A->>S: Read / write todo
        S-->>A: Todo
    else List my todos
        A->>M: Read my todos
        M-->>A: Todo page
        A->>R: Get cached users
        R-->>A: Users
    end
    A-->>E: Result
    E-->>C: JSON response
```

#### Deployment (Docker Compose)

```mermaid
flowchart LR
    user([User]) -->|":80"| nginx[nginx<br/>api_gateway]
    user -->|":5050"| nginx
    nginx -->|":8080"| api[fasttodo-api]
    nginx --> admin[pgAdmin / Adminer]
    migrator[fasttodo-migrator<br/>schema setup] -->|run once| db[(Postgres / SQL Server)]
    api -->|depends on migrator| migrator
    api --> db
    api --> mongo[(MongoDB replica set)]
    api --> redis[(Redis)]
    admin --> db
```

### Database Providers
- SQL Server
- SQLite
- Postgres
- [Coming soon] MariaDB
- Redis
- MongoDB

### Backend Stack
- [.NET](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) `8.0`
- [FastEndpoints](https://fast-endpoints.com/) `5.34.0`
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) `9.0.0`
- [MediatR](https://github.com/jbogard/MediatR) `12.4.1`
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/) `12.0.0`
- [Nginx](https://nginx.org/)

### Patterns
- Generic Repository Pattern with Unit of Work
- CQRS Pattern with [MediatR](https://github.com/jbogard/MediatR)
- Option pattern

## Solution Structure
| Project | Responsibility |
|---------|---------------|
| FastTodo.API | Main entry point. Handles HTTP requests, configures services, and manages API endpoints using FastEndpoints |
| FastTodo.Application | Contains application business logic, CQRS handlers, validations, and mapping profiles |
| FastTodo.Domain | Core domain entities, interfaces, and business rules. No external dependencies |
| FastTodo.Domain.Shared | Shared constants, enums, and utility classes used across all layers |
| FastTodo.Infrastructure | Cross-cutting concerns: data access, logging, caching, authentication |
| FastTodo.Infrastructure.Domain | Interfaces and base classes for infrastructure services |
| FastTodo.Persistence.EF | Entity Framework Core implementations for SQL Server |
| FastTodo.Persistence.SQLite | SQLite specific database context and configurations |
| FastTodo.Persistence.Postgres | PostgreSQL specific database context and configurations |
| FastTodo.Persistence.Mongo | MongoDB EF Core provider context, query repository and unit of work |
| FastTodo.Persistence.Redis | Redis / in-memory distributed cache and `ICacheService` implementation |
| FastTodo.SeedData | Console application for seeding sample data into the database |

## Features
### ✅ Completed 
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
8. Dockerization
9. Logging
10. ASP.NET Core Identity Integration
11. Options Pattern
    - Allow choosing database providers
12. User Management
    - Implement ASP.NET Core Identity login using JWT
    - User Authentication
    - Todo Item Ownership

### 🚧 In Progress 
1. User Management 
    - Todo Item Assignment

### 📋 Planning 
1. Options Pattern
    - Swagger vs Scalar
2. Allow multiple database connection from multiple database providers
3. Caching with Redis
4. MongoDB Integration
5. Integrate CAP
6. RabbitMQ Integration
6. Unit Test
7. Azure Container integration

## API Endpoints

### Todo Operations

| Method | Endpoint        | Description                           |
|--------|----------------|---------------------------------------|
| GET    | /api/todos     | Get all todos with optional filtering |
| GET    | /api/todos/{id}| Get a specific todo by ID            |
| POST   | /api/todos     | Create a new todo                    |
| PUT    | /api/todos/{id}| Update an existing todo             |
| PATCH  | /api/todos/{id}| Update todo status                  |
| DELETE | /api/todos/{id}| Delete a todo                       |

### Authentication & User Management

| Method | Endpoint                    | Description                          |
|--------|----------------------------|--------------------------------------|
| POST   | /api/accounts/register     | Register a new user                 |
| POST   | /api/accounts/login        | Login and get JWT token             |
| POST   | /api/accounts/refresh      | Refresh JWT token                   |
| POST   | /api/accounts/logout       | Logout and invalidate token         |
| GET    | /api/accounts/info         | Get current user info               |
| PUT    | /api/accounts/info         | Update user profile                 |
| PUT    | /api/accounts/password     | Change password                     |
| POST   | /api/accounts/confirmemail | Confirm email address               |
| POST   | /api/accounts/resetpassword| Reset forgotten password            |

## Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/) or [Podman](https://podman.io/)

Setup `docker containers` in [Docker documentation](https://github.com/p3t3r276/todolist-net8-fastendpoint/blob/dev/docker/Readme.md)

### Configuration
The database provider can be configured in `appsettings.json`:

```json
{
"ConnectionStrings": {
    "Default": "",
    "Identity": ""
  },
  "fastTodoOption": {
    "SQLProvider": "", // Postgres, SQLite or SQLServer
    "noSql": "",
    "openapi": "swagger" // swagger or scalar
  },
}
```
