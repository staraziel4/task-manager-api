# Task Manager API

A REST API for managing personal tasks, built with **ASP.NET Core 8**, **Entity Framework Core**, **PostgreSQL** and **JWT authentication**.

Built as a portfolio project to demonstrate backend fundamentals: clean architecture, authentication, database modeling, validation, pagination, and automated testing.

## Features

- User registration and login with JWT authentication
- Password hashing with BCrypt (passwords are never stored in plain text)
- Full CRUD for tasks, scoped to the authenticated user (no user can see another user's tasks)
- Pagination and filtering by status on the tasks list
- Input validation with Data Annotations
- Interactive API documentation with Swagger/OpenAPI, including JWT support
- Unit tests for the business logic layer (xUnit + EF Core InMemory)

## Tech Stack

| Layer          | Technology                          |
|----------------|--------------------------------------|
| API            | ASP.NET Core 8 (Web API, Controllers) |
| Database       | PostgreSQL + Entity Framework Core    |
| Auth           | JWT Bearer + BCrypt                   |
| Docs           | Swashbuckle (Swagger)                 |
| Testing        | xUnit + EF Core InMemory              |

## Project Structure

```
TaskManagerApi/
├── src/
│   └── TaskManagerApi.Api/
│       ├── Controllers/      # HTTP endpoints
│       ├── Services/         # Business logic (testable, independent of HTTP)
│       ├── Data/             # EF Core DbContext
│       ├── Models/           # Database entities
│       ├── DTOs/             # Request/response contracts
│       └── Migrations/       # EF Core migrations
├── tests/
│   └── TaskManagerApi.Tests/ # Unit tests
└── docker-compose.yml        # Local PostgreSQL for development
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (to run PostgreSQL locally) — or a local PostgreSQL install

### 1. Start the database

```bash
docker compose up -d
```

### 2. Configure secrets (local development)

This project keeps secrets out of source control using [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets).

```bash
cd src/TaskManagerApi.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=taskmanager;Username=postgres;Password=postgres"
dotnet user-secrets set "Jwt:Key" "<a random base64 string, at least 32 bytes>"
```

In production, these values should come from environment variables or a secrets manager (Azure Key Vault, AWS Secrets Manager, etc.), never from a committed file.

### 3. Run the API

```bash
dotnet run --project src/TaskManagerApi.Api
```

The database schema is created automatically on startup by applying EF Core migrations. Swagger UI is available at `https://localhost:<port>/swagger`.

### 4. Run the tests

```bash
dotnet test
```

## API Overview

| Method | Endpoint             | Description                        | Auth required |
|--------|-----------------------|-------------------------------------|----------------|
| POST   | `/api/auth/register`  | Create a new user                   | No             |
| POST   | `/api/auth/login`     | Log in and receive a JWT            | No             |
| GET    | `/api/tasks`          | List tasks (paginated, filterable)  | Yes            |
| GET    | `/api/tasks/{id}`     | Get a single task                   | Yes            |
| POST   | `/api/tasks`          | Create a task                       | Yes            |
| PUT    | `/api/tasks/{id}`     | Update a task                       | Yes            |
| DELETE | `/api/tasks/{id}`     | Delete a task                       | Yes            |

To call the protected endpoints, register/login, copy the returned token, and click "Authorize" in Swagger UI (or send `Authorization: Bearer <token>` header).

## Design Decisions

- **Service layer separated from controllers**: business logic lives in `Services/`, independent of HTTP concerns, which is what makes it unit-testable without spinning up a web server.
- **Tasks are scoped by `UserId` at the query level**, not just hidden in the UI — a user can never fetch, edit or delete another user's task, even by guessing an ID.
- **Passwords are hashed with BCrypt**, never stored or logged in plain text.

## Roadmap

This is the first of a 3-project portfolio series. Next steps:
- Full-stack version with Angular frontend, Dockerized end-to-end
- CI/CD pipeline, rate limiting, refresh tokens, and cloud deployment
