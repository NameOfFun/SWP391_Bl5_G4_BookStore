# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run (HTTP on :5268, HTTPS on :7159)
dotnet run

# Watch mode
dotnet watch run

# EF Core migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update
dotnet ef database drop --force

# Run a specific migration target
dotnet ef database update <MigrationName>
```

## Architecture

ASP.NET Core 8 MVC application using Razor Views, Entity Framework Core 8, and ASP.NET Identity.

### Layer overview

```
Controllers/          → MVC controllers (flat, no Areas yet)
Service/
  Interfaces/         → IAuthService, IAboutService, …
  Implements/         → AuthService, AboutService, …
Dtos/
  Auth/               → LoginDto, RegisterDto, result DTOs
  Common/             → Shared DTOs (AboutDto, …)
  Admin/ Staff/ Manager/ Customer/   → Role-scoped DTOs (empty/growing)
Models/               → EF entities + BookStoreDbContext
Views/                → Razor views per controller
Migrations/           → EF Core migrations
```

### Key patterns

**Service / Result DTO pattern** — Services return structured result objects instead of throwing exceptions. Example:
```csharp
// Service returns a result DTO
var result = await _authService.LoginAsync(dto);
if (!result.Succeeded) { /* show errors */ }
```

**Auditing** — Most entities have `UpdatedByUserId` (FK to `IdentityUser`) and `UpdatedAt`. Capture the current user id from `User.FindFirstValue(ClaimTypes.NameIdentifier)` in the controller and pass it to the service.

**Soft delete** — Use `IsActive = false` rather than deleting rows. Filter queries with `.Where(x => x.IsActive)`.

**TempData messaging** — Controllers use `TempData["Success"]` / `TempData["Error"]` to pass flash messages to views.

**Vietnamese error messages** — All user-facing validation and service error messages are in Vietnamese.

### Database

- Connection string key: `"MyCnn"` (SQL Server, `BookStoreDB`)
- DbContext: `BookStoreDbContext` (inherits `IdentityDbContext`)
- All EF mapping is done with Fluent API inside `OnModelCreating`; no Data Annotations on entities.

### Identity & roles

Four roles seeded at startup: `Customer`, `Admin`, `Staff`, `Manager`.  
Default role for new registrations: `Customer`.

Password policy (configured in `Program.cs`):
- MinimumLength: 6, RequireDigit: true, RequireLowercase: true
- RequireUppercase: false, RequireNonAlphanumeric: false

### Adding a new feature (typical checklist)

1. Add/update model in `Models/` and configure in `BookStoreDbContext.OnModelCreating`.
2. Create a migration: `dotnet ef migrations add <Name>`.
3. Add DTOs in the matching role folder under `Dtos/`.
4. Add interface + implementation under `Service/`.
5. Register the service in `Program.cs` as Scoped.
6. Add controller in `Controllers/`; protect with `[Authorize(Roles = "...")]` as needed.
7. Add Razor views under `Views/<ControllerName>/`.

### Frontend

Bootstrap 5 + Bootstrap Icons loaded from CDN. jQuery Unobtrusive Validation is included via `_ValidationScriptsPartial.cshtml`. Two layouts exist:
- `_Layout.cshtml` — main site layout
- `_LayoutAuth.cshtml` — login/register pages
