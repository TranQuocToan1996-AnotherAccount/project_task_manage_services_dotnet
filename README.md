# Task Management API

A RESTful Task Management API built with .NET 10, following a Golang-style architecture (Controller → Service → Repository).

## Features

- **Authentication**: JWT-based authentication with refresh tokens
- **Authorization**: Protected endpoints with role-based access
- **CRUD Operations**: Full CRUD for Users, Projects, and Tasks
- **Pagination**: Paginated results for list endpoints
- **Filtering**: Filter tasks by status, priority, project, and keyword
- **Caching**: Redis caching with 5-minute TTL for read operations
- **Validation**: FluentValidation for request DTOs
- **Mapping**: Mapster for Entity-DTO mapping
- **Logging**: Serilog with console and file sinks
- **Exception Handling**: Global exception middleware
- **Database**: PostgreSQL with Entity Framework Core
- **Docker**: Complete Docker Compose setup

## Tech Stack

- .NET 10 (Preview)
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Redis
- JWT Authentication
- FluentValidation
- Mapster
- Serilog
- Swagger/OpenAPI
- Docker & Docker Compose

## Project Structure

```
TaskManagement/
├── Controllers/          # API endpoints
├── Services/             # Business logic layer
├── Repositories/         # Data access layer
├── DTO/                  # Data Transfer Objects
├── Models/               # EF Core entities
├── Data/                 # DbContext
├── Utility/              # Helper classes (JWT, Password, Redis)
├── Middleware/           # Global exception handling
├── Validators/           # FluentValidation validators
├── Mapping/              # Mapster configuration
├── Configurations/       # Options classes
├── Constants/            # Cache keys, etc.
├── Common/               # Shared classes (ApiResponse, Pagination)
├── Program.cs            # Application entry point
└── docker-compose.yml    # Docker orchestration
```

## Running the Application

### Using Docker Compose (Recommended)

```bash
docker-compose up
```

This will start:
- API on port 8080
- PostgreSQL on port 5432
- Redis on port 6379
- PgAdmin on port 5050

### Local Development

1. **Install dependencies**:
   ```bash
   dotnet restore
   ```

2. **Run database migrations**:
   ```bash
   export PATH="$PATH:/Users/at/.dotnet/tools"
   dotnet ef database update
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` or `http://localhost:5000`.

### Access Swagger UI

When running locally, access the Swagger UI at:
```
https://localhost:5001/swagger
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Logout (invalidate refresh token)

### Users (Protected)
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create a new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Projects (Protected)
- `GET /api/projects?page=1&pageSize=10` - Get all projects with pagination
- `GET /api/projects/{id}` - Get project by ID
- `GET /api/projects/owner/{ownerId}` - Get projects by owner
- `POST /api/projects` - Create a new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

### Tasks (Protected)
- `GET /api/tasks?page=1&pageSize=10&status=done&priority=high&keyword=test` - Get tasks with filters
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create a new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `POST /api/tasks/{id}/assign` - Assign task to a user
- `PATCH /api/tasks/{id}/status` - Update task status

## Database Schema

### Users
- `id` (GUID)
- `username` (string)
- `email` (string)
- `password_hash` (string)
- `created_at` (datetime)
- `updated_at` (datetime)

### Projects
- `id` (GUID)
- `name` (string)
- `description` (string)
- `owner_id` (GUID, FK to users)
- `created_at` (datetime)
- `updated_at` (datetime)

### Tasks
- `id` (GUID)
- `title` (string)
- `description` (string)
- `status` (enum: Todo, InProgress, Done)
- `priority` (enum: Low, Medium, High)
- `project_id` (GUID, FK to projects)
- `assignee_id` (GUID, FK to users, nullable)
- `created_at` (datetime)
- `updated_at` (datetime)

## Configuration

Edit `appsettings.json` to configure:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=postgres;Port=5432;Database=taskdb;Username=postgres;Password=123456"
  },
  "Jwt": {
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient",
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "ExpirationInMinutes": 60
  },
  "Redis": {
    "ConnectionString": "redis:6379"
  }
}
```

## Learning Notes for Golang Developers

This project follows a similar structure to Golang projects:

- **Controllers** ≈ Go Handlers
- **Services** ≈ Go Service Layer
- **Repositories** ≈ Go Repository/DAO Layer
- **Dependency Injection** is built-in (no manual wiring like Go)
- **Middleware** works similarly to Go middleware
- **LINQ** is different from SQL but powerful for data access
- **Entity Framework** is an ORM (vs raw SQL in Go)
- **Async/Await** patterns for asynchronous operations
- **Strong typing** throughout the codebase
