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

ASP.NET Core 8 MVC application (Razor Views, EF Core 8, ASP.NET Identity). No Areas — all controllers are flat in `Controllers/`.

### Layer overview

```
Controllers/          → MVC controllers
Service/
  Interfaces/         → ICategoryService, IBookService, IAboutService, IAuthService
  Implements/         → concrete implementations
Dtos/
  Auth/               → LoginDto, RegisterDto
  Common/             → shared DTOs used by controllers (BookDto, CategoryDto, AboutDto)
  Admin/ Staff/ Manager/ Customer/   → role-scoped DTOs (reserved, currently empty)
Models/               → EF entities + BookStoreDbContext
Views/                → Razor views per controller
Migrations/           → EF Core migrations
```

### Key patterns

**Service / DTO pattern** — Controllers call services; services return DTOs. Services throw `ArgumentException` for bad input and `InvalidOperationException` for not-found. Controllers catch these and add `ModelState` errors.

**Author auto-create** — `BookService.ResolveAuthorIdAsync` looks up an author by name (case-insensitive) and creates one if missing. Entering a new author name in the Book form is enough to register them.

**Controller accessing DbContext directly** — `BookController` injects `BookStoreDbContext` alongside `IBookService` solely to populate `<select>` dropdowns via `PopulateDropdownsAsync`. This is intentional; don't refactor it to the service unless populating dropdowns becomes reusable.

**Client-side vs server-side filtering** — `BookController.Index` loads all books and delegates search/filter/pagination to JavaScript. `CategoryController.Index` does server-side filtering with query parameters (`search`, `status`, `page`). New list pages should follow the Category pattern (server-side).

**Pagination (server-side)** — `CategoryController` uses `const int PageSize = 10`, applies `.Skip().Take()` after filtering, and passes `Page`, `TotalPages`, `TotalCount`, `PageSize` through `ViewData`. Views read these variables at the top of the file.

**Auditing** — Most entities have `UpdatedByUserId` (FK → `IdentityUser`). Always capture the current user id with `User.FindFirstValue(ClaimTypes.NameIdentifier)` in the controller and pass it to the service.

**Soft delete** — Use `IsActive = false` rather than deleting rows. For status toggling, services expose `ToggleStatusAsync` / `ChangeStatusAsync` which flip the boolean and update `UpdatedByUserId`.

**TempData messaging** — Controllers set `TempData["Success"]` or `TempData["Error"]`; views render dismissible Bootstrap alerts.

**Vietnamese error messages** — All user-facing validation and service error messages must be in Vietnamese.

### Database

- Connection string key: `"MyCnn"` (SQL Server, database `BookStoreDB`)
- Each developer overrides `"MyCnn"` in `appsettings.Development.json` (not committed) to point at their local instance.
- DbContext: `BookStoreDbContext` (inherits `IdentityDbContext`)
- All EF mapping uses Fluent API in `OnModelCreating`; no Data Annotations on entity classes.

### Identity & roles

Four roles seeded at startup: `Customer`, `Admin`, `Staff`, `Manager`.  
Default role for new registrations: `Customer`.

Password policy (in `Program.cs`): MinimumLength 6, RequireDigit, RequireLowercase; no uppercase or non-alphanumeric requirement.

> **Dev note:** During active development, controllers are temporarily set to `[Authorize(Roles = "Customer")]` (with the real role commented out) so any logged-in account can access them. Remember to restore the correct role before merging.

### Adding a new feature (checklist)

1. Add/update entity in `Models/` and configure with Fluent API in `BookStoreDbContext.OnModelCreating`.
2. Create migration: `dotnet ef migrations add <Name>`.
3. Add DTOs in `Dtos/Common/` (or the appropriate role folder).
4. Add interface in `Service/Interfaces/` and implementation in `Service/Implements/`.
5. Register the service in `Program.cs` as `Scoped`.
6. Add controller in `Controllers/`; protect with `[Authorize(Roles = "...")]`.
7. Add Razor views under `Views/<ControllerName>/`.
8. Add a sidebar entry in `Views/Shared/_Layout.cshtml`.

### Frontend

Bootstrap 5 + Bootstrap Icons (CDN). jQuery Unobtrusive Validation via `_ValidationScriptsPartial.cshtml`.

Two layouts:
- `_Layout.cshtml` — admin/management shell with collapsible sidebar
- `_LayoutAuth.cshtml` — login/register pages

Sidebar active state is driven by `ViewContext.RouteData.Values["controller"]` and `["action"]`. A submenu auto-opens when `ctrl` matches any of its child controllers (see the Book/Category submenu for the pattern).
