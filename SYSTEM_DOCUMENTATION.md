# Portfol.io — Complete System Documentation

> A multi-user, self-hosted developer portfolio platform built with **ASP.NET Core 10 MVC** and **Entity Framework Core 10** backed by **SQL Server**.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Technology Stack](#2-technology-stack)
3. [Project Structure](#3-project-structure)
4. [Data Layer](#4-data-layer)
   - [Entity Model Reference](#41-entity-model-reference)
   - [Entity Relationship Diagram](#42-entity-relationship-diagram)
   - [Database Configuration](#43-database-configuration)
   - [Audit Timestamps](#44-audit-timestamps)
   - [Migrations](#45-migrations)
5. [Authentication & Authorization](#5-authentication--authorization)
   - [Identity Configuration](#51-identity-configuration)
   - [Cookie Settings](#52-cookie-settings)
   - [Database Seeding](#53-database-seeding)
6. [Routing Architecture](#6-routing-architecture)
7. [Public Portfolio (HomeController)](#7-public-portfolio-homecontroller)
8. [Admin Area](#8-admin-area)
   - [AuthController](#81-authcontroller)
   - [DashboardController](#82-dashboardcontroller)
   - [ProfileController](#83-profilecontroller)
   - [ProjectsController](#84-projectscontroller)
   - [CredentialsController](#85-credentialscontroller)
   - [SkillsController](#86-skillscontroller)
   - [TimelineEventsController](#87-timelineeventscontroller)
   - [PlatformProfilesController](#88-platformprofilescontroller)
   - [ContactMessagesController](#89-contactmessagescontroller)
9. [ViewModels](#9-viewmodels)
10. [Views & Layouts](#10-views--layouts)
11. [Static Assets & Styling](#11-static-assets--styling)
12. [Performance Optimizations](#12-performance-optimizations)
13. [Security Model](#13-security-model)
14. [Enumerations Reference](#14-enumerations-reference)
15. [Complete Route Reference](#15-complete-route-reference)

---

## 1. Project Overview

**Portfol.io** is a multi-user developer portfolio system where each registered `ApplicationUser` gets a public-facing portfolio page accessible via their unique username in the URL (e.g., `https://yoursite.com/john`). The site simultaneously serves as a CMS (Content Management System) via a protected `/Admin` area where users manage their own portfolio content independently.

**Key design decisions:**
- Each `ApplicationUser` owns all their data — data isolation is enforced at the controller and query levels via `ApplicationUserId` filtering.
- No separate registration flow exists on the public side. New admin users register via `/Admin/Auth/Register`.
- The public homepage with no username defaults to displaying the first user in the `Admin` role.
- All admin routes are gated by `[Authorize]` — only authenticated users can access them.

---

## 2. Technology Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core 10 MVC |
| **ORM** | Entity Framework Core 10 |
| **Database** | SQL Server (via `Microsoft.EntityFrameworkCore.SqlServer`) |
| **Authentication** | ASP.NET Core Identity (`IdentityUser<int>`, integer primary keys) |
| **Frontend CSS** | Bootstrap 5 + custom `site.css` / `admin.css` |
| **Icon Fonts** | [Devicon](https://devicon.dev/) (CDN) + [FontAwesome 6](https://fontawesome.com/) (CDN) |
| **Google Fonts** | Geist (400, 600), JetBrains Mono (400, 600, 700), Material Symbols Outlined |
| **Target Runtime** | .NET 10 |
| **Nullable Reference Types** | Enabled |
| **Implicit Usings** | Enabled |

**NuGet packages:**
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` v10.0.8
- `Microsoft.EntityFrameworkCore.Design` v10.0.8
- `Microsoft.EntityFrameworkCore.SqlServer` v10.0.8
- `Microsoft.EntityFrameworkCore.Tools` v10.0.8
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` v10.0.2

---

## 3. Project Structure

```
Portfol.io/
├── Areas/
│   └── Admin/                         # Protected admin area
│       ├── Controllers/
│       │   ├── AuthController.cs      # Login, Register, Logout, Access Denied
│       │   ├── ContactMessagesController.cs
│       │   ├── CredentialsController.cs
│       │   ├── DashboardController.cs
│       │   ├── PlatformProfilesController.cs
│       │   ├── ProfileController.cs   # Edit own profile info
│       │   ├── ProjectsController.cs
│       │   ├── SkillsController.cs
│       │   └── TimelineEventsController.cs
│       ├── ViewModels/
│       │   ├── EditProfileViewModel.cs
│       │   ├── LoginViewModel.cs
│       │   └── RegisterViewModel.cs
│       └── Views/
│           ├── Auth/                  # Login, Register, AccessDenied
│           ├── ContactMessages/       # Index, Details, Delete
│           ├── Credentials/           # Index, Details, Create, Edit, Delete
│           ├── Dashboard/             # Main admin overview
│           ├── PlatformProfiles/      # Index, Details, Create, Edit, Delete
│           ├── Profile/               # Edit own profile
│           ├── Projects/              # Index, Details, Create, Edit, Delete
│           ├── Shared/
│           │   ├── _AdminLayout.cshtml
│           │   └── _AdminAuthLayout.cshtml
│           ├── Skills/                # Index, Details, Create, Edit, Delete
│           ├── TimelineEvents/        # Index, Details, Create, Edit, Delete
│           ├── _ViewImports.cshtml
│           └── _ViewStart.cshtml
├── Controllers/
│   └── HomeController.cs              # Public portfolio display
├── Data/
│   └── ApplicationDbContext.cs        # EF Core DbContext
├── Migrations/
│   └── 20260604224828_InitialIdentityCreate.cs  # Single migration for all tables
├── Models/
│   ├── ApplicationUser.cs             # Extends IdentityUser<int>
│   ├── BaseEntity.cs                  # Abstract base with Id, CreatedAt, UpdatedAt
│   ├── ContactMessage.cs
│   ├── Credential.cs
│   ├── CredentialSkill.cs             # Join table: Credential ↔ Skill
│   ├── ErrorViewModel.cs
│   ├── PlatformProfile.cs
│   ├── Project.cs
│   ├── ProjectSkill.cs                # Join table: Project ↔ Skill
│   ├── Skill.cs
│   ├── TimelineEvent.cs
│   └── Enums/
│       ├── EAvailabilityStatus.cs
│       ├── ESkillCategory.cs
│       └── ETimelineEventType.cs
├── ViewModels/
│   └── HomeViewModel.cs               # Public portfolio view model
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml               # Public portfolio page
│   │   └── Privacy.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml             # Public layout (navbar, footer)
│   │   └── Error.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/
│   │   ├── site.css                   # Public site styles
│   │   ├── admin.css                  # Admin panel styles
│   │   └── admin-auth.css             # Login / Register page styles
│   ├── js/
│   │   └── site.js
│   └── lib/                           # Bootstrap 5, jQuery (bundled locally)
├── appsettings.json                   # Connection string + seed config
├── appsettings.Development.json
├── Program.cs                         # Application entry point & middleware
└── Portfol.io.csproj                  # .NET 10 project file
```

---

## 4. Data Layer

### 4.1 Entity Model Reference

#### `BaseEntity` (Abstract)
All domain entities (except `ApplicationUser` which inherits `IdentityUser<int>`) extend `BaseEntity`.

| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | Primary key (`[Key]`) |
| `CreatedAt` | `DateTime` | Auto-set on insert via `ApplyAuditTimestamps()` |
| `UpdatedAt` | `DateTime` | Auto-updated on every save |

---

#### `ApplicationUser`
Inherits `IdentityUser<int>`. Adds portfolio-specific fields.

| Property | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK (inherited) | Integer identity PK |
| `UserName` | `string` | Unique (Identity) | Used as the public URL slug, e.g. `/john` |
| `Email` | `string` | Unique (Identity) | Used for login |
| `FullName` | `string?` | Max 100 chars | Display name |
| `JobTitle` | `string?` | Max 100 chars | e.g. "Full-Stack Engineer" |
| `Bio` | `string` | Required, Max 2000 chars | Multi-line bio |
| `ProfileImageUrl` | `string?` | URL, Max 500 chars | Avatar/profile photo link |
| `GitHubUrl` | `string?` | URL, Max 500 chars | GitHub profile link |
| `LinkedInUrl` | `string?` | URL, Max 500 chars | LinkedIn profile link |
| `AvailabilityStatus` | `EAvailabilityStatus` | Enum | Defaults to `Unavailable` |
| `Projects` | `ICollection<Project>` | Navigation | One-to-many |
| `Skills` | `ICollection<Skill>` | Navigation | One-to-many |
| `TimelineEvents` | `ICollection<TimelineEvent>` | Navigation | One-to-many |
| `Credentials` | `ICollection<Credential>` | Navigation | One-to-many |
| `PlatformProfiles` | `ICollection<PlatformProfile>` | Navigation | One-to-many |
| `ContactMessages` | `ICollection<ContactMessage>` | Navigation | One-to-many |

---

#### `Project`
Table: `PortfolioProjects` (explicit via `[Table]` attribute)

| Property | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK (BaseEntity) | |
| `Title` | `string` | Required, Max 100 | |
| `Description` | `string` | Required, Max 2000 | Multi-line |
| `RepositoryUrl` | `string?` | URL, Max 500 | GitHub repo link |
| `LiveDemoUrl` | `string?` | URL, Max 500 | Live site link |
| `ImageUrl` | `string?` | URL, Max 500 | Cover image (lazy loaded in public view) |
| `IsFeatured` | `bool` | Default `false` | Highlight in portfolio |
| `DisplayOrder` | `int` | Required, `Range(1, MaxValue)` | Unique per user (enforced in controller) |
| `CompletionDate` | `DateTime?` | Date only | |
| `ApplicationUserId` | `int` | FK → `ApplicationUser` | Data isolation key |
| `ProjectSkills` | `ICollection<ProjectSkill>` | Navigation | Many-to-many via join table |

---

#### `Skill`

| Property | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK | |
| `Name` | `string` | Required, Max 50 | e.g. "TypeScript" |
| `Category` | `ESkillCategory` | Required, Enum | Groups the skill in UI |
| `ProficiencyLevel` | `int` | `Range(1, 100)` | Percentage proficiency |
| `IconClass` | `string?` | Max 50 | Devicon or FontAwesome CSS class, e.g. `devicon-react-original` |
| `DisplayOrder` | `int` | Required, `Range(1, MaxValue)` | Unique per user (enforced in controller) |
| `ApplicationUserId` | `int` | FK → `ApplicationUser` | |
| `ProjectSkills` | `ICollection<ProjectSkill>` | Navigation | Reverse navigation |
| `CredentialSkills` | `ICollection<CredentialSkill>` | Navigation | Reverse navigation |

---

#### `Credential`

| Property | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK | |
| `Name` | `string` | Required, Max 200 | e.g. "AWS Solutions Architect" |
| `IssuingAuthority` | `string` | Required, Max 200 | e.g. "Amazon Web Services" |
| `IssueDate` | `DateTime` | Required | |
| `ExpiryDate` | `DateTime?` | Nullable | `null` = no expiry |
| `VerificationUrl` | `string?` | URL, Max 500 | Link to verify credential |
| `BadgeUrl` | `string?` | URL, Max 500 | Badge image URL |
| `ApplicationUserId` | `int` | FK → `ApplicationUser` | |
| `CredentialSkills` | `ICollection<CredentialSkill>` | Navigation | Many-to-many via join table |

---

#### `TimelineEvent`

| Property | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK | |
| `Title` | `string` | Required, Max 200 | e.g. "Senior Engineer at Acme Corp" |
| `Organization` | `string` | Required, Max 200 | Company, school, etc. |
| `Location` | `string?` | Max 100 | City or "Remote" |
| `Description` | `string?` | Max 1000 | Multi-line |
| `EventType` | `ETimelineEventType` | Enum | Defaults to `Work` |
| `StartDate` | `DateTime` | Required | |
| `EndDate` | `DateTime?` | Nullable | `null` = "Present" |
| `ApplicationUserId` | `int` | FK → `ApplicationUser` | |

---

#### `PlatformProfile`

| Property | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK | |
| `PlatformName` | `string` | Required, `varchar(25)` | e.g. "Codeforces", "LeetCode" |
| `UserHandle` | `string` | Required, Max 50, alphanumeric + `_` | e.g. "john_doe" |
| `Rank` | `string?` | Max 20 | e.g. "Candidate Master" |
| `ProfileUrl` | `string?` | URL, Max 500 | Direct profile link |
| `DisplayOrder` | `int` | Required | |
| `ApplicationUserId` | `int` | FK → `ApplicationUser` | |

---

#### `ContactMessage`

| Property | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK | |
| `SenderName` | `string` | Required, Max 100 | |
| `SenderEmail` | `string` | Required, Email, Max 254 (RFC 5321) | |
| `Subject` | `string` | Required, Max 100 | |
| `Body` | `string` | Required, Max 2000 | Multi-line |
| `SentDate` | `DateTime` | Default `DateTime.UtcNow` | |
| `IsRead` | `bool` | Default `false` | |
| `IsArchived` | `bool` | Default `false` | |
| `RepliedAt` | `DateTime?` | Nullable | |
| `ApplicationUserId` | `int` | FK → `ApplicationUser` | Routes message to correct user |

---

#### `ProjectSkill` (Join Table)

| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK (inherited from BaseEntity) |
| `ProjectId` | `int` | FK → `Project` (Cascade delete) |
| `SkillId` | `int` | FK → `Skill` (**Restrict** delete — prevents orphan cascade) |
| `VersionUsed` | `string?` | Max 20, e.g. `v8.0` |

> **Unique Index**: `(ProjectId, SkillId)` — prevents duplicate associations.

---

#### `CredentialSkill` (Join Table)

| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | PK (inherited from BaseEntity) |
| `CredentialId` | `int` | FK → `Credential` (Cascade delete) |
| `SkillId` | `int` | FK → `Skill` (**Restrict** delete) |
| `IsCoreFocus` | `bool` | Marks a skill as the primary focus of the credential (default `false`) |

> **Unique Index**: `(CredentialId, SkillId)` — prevents duplicate associations.

---

### 4.2 Entity Relationship Diagram

```
ApplicationUser (IdentityUser<int>)
│
├─── 1:N ──► Project
│               └─── N:M via ProjectSkill ──► Skill
│
├─── 1:N ──► Skill
│
├─── 1:N ──► Credential
│               └─── N:M via CredentialSkill ──► Skill
│
├─── 1:N ──► TimelineEvent
│
├─── 1:N ──► PlatformProfile
│
└─── 1:N ──► ContactMessage
```

**Cascade rules:**
- Deleting a `Project` → cascades to its `ProjectSkill` rows.
- Deleting a `Credential` → cascades to its `CredentialSkill` rows.
- Deleting a `Skill` → **blocked** (Restrict) if it is linked to any `ProjectSkill` or `CredentialSkill`. The admin must remove skill associations first.
- Deleting an `ApplicationUser` → cascades to all their owned entities.

---

### 4.3 Database Configuration

Connection string is defined in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=PortfolioDb;..."
  },
  "AdminSeed": {
    "Email": "admin@portfol.io",
    "Password": "Admin!Portfol.io2026"
  }
}
```

Registered in `Program.cs`:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

### 4.4 Audit Timestamps

`ApplicationDbContext` overrides both `SaveChanges()` and `SaveChangesAsync()` to call `ApplyAuditTimestamps()`:

- **On `EntityState.Added`**: Sets both `CreatedAt` and `UpdatedAt` to `DateTime.UtcNow`.
- **On `EntityState.Modified`**: Updates only `UpdatedAt`. Explicitly marks `CreatedAt` as not modified to prevent overwrite.

This is transparent to all controllers — no manual timestamp handling is needed anywhere.

---

### 4.5 Migrations

A single EF Core migration file exists:
- **`20260604224828_InitialIdentityCreate`** — Creates all tables including ASP.NET Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, etc.) plus all custom domain tables.

To apply or re-apply migrations:
```bash
dotnet ef database update
```

To add a new migration after model changes:
```bash
dotnet ef migrations add <MigrationName>
```

---

## 5. Authentication & Authorization

### 5.1 Identity Configuration

The app uses `AddIdentity<ApplicationUser, IdentityRole<int>>()` (not `AddDefaultIdentity`) to avoid the pre-built Microsoft UI and use integer primary keys.

**Password policy:**
| Setting | Value |
|---|---|
| Minimum length | **16 characters** |
| Require digit | ✅ |
| Require non-alphanumeric | ✅ |
| Require uppercase | ✅ |
| Unique email required | ✅ |

**Lockout policy:**
| Setting | Value |
|---|---|
| Max failed attempts | **5** |
| Lockout duration | **15 minutes** |
| Applied to new users | ✅ |

---

### 5.2 Cookie Settings

```
Cookie Name:     Portfolio.AdminAuth
HttpOnly:        true
SameSite:        Strict
Secure:          Always (HTTPS in production) / SameAsRequest (dev)
Expiry:          7 days (sliding)
Login path:      /Admin/Auth/Login
Access denied:   /Admin/Auth/AccessDenied
```

---

### 5.3 Database Seeding

On every application startup, `Program.cs` runs a seed block that:

1. Creates the `Admin` role if it doesn't already exist.
2. Looks up the configured seed email (`AdminSeed:Email` from `appsettings.json`, default `admin@portfol.io`).
3. If no user with that email exists, creates an `ApplicationUser` with:
   - `UserName`: `"admin"`
   - `FullName`: `"Portfolio Admin"`
   - `Bio`: placeholder text
   - `AvailabilityStatus`: `Building`
   - `EmailConfirmed`: `true` (no email verification flow)
4. Assigns the user to the `Admin` role.

> **⚠️ Important**: Change the seed password (`Admin!Portfol.io2026`) after first login. Override it via `appsettings.json` or environment variables.

---

## 6. Routing Architecture

Three named routes are registered in `Program.cs`, evaluated in order:

| Priority | Route Name | Pattern | Defaults | Description |
|---|---|---|---|---|
| 1 | `areas` | `{area:exists}/{controller=Dashboard}/{action=Index}/{id?}` | — | Admin area routes, e.g. `/Admin/Projects/Create` |
| 2 | `user_portfolio` | `{username}` | `controller=Home, action=Index` | Public portfolio by username, e.g. `/john` |
| 3 | `default` | `{controller=Home}/{action=Index}/{id?}` | — | Fallback MVC routes |

**Username-based routing logic (in `HomeController.Index`):**
- No username or username `"Home"` (case-insensitive) → `404 Not Found` (username in route path is strictly required).
- Valid username → load that user's portfolio data.
- Unknown username → `404 Not Found`.

---

## 7. Public Portfolio (HomeController)

**File:** [`Controllers/HomeController.cs`](Controllers/HomeController.cs)

### `GET /{username}` → `Index(string? username)`

Loads and renders the full public portfolio for the target user:

| Data Loaded | Ordering | Includes |
|---|---|---|
| Projects | `DisplayOrder` ASC, `CompletionDate` DESC | `ProjectSkills → Skill` |
| Skills | `Category` ASC, `DisplayOrder` ASC | — |
| Credentials | `IssueDate` DESC | `CredentialSkills → Skill` |
| TimelineEvents | `StartDate` DESC | — |
| PlatformProfiles | `DisplayOrder` ASC | — |

All queries use `.AsNoTracking()` for read performance.

### `POST /Home/SubmitMessage` → `SubmitMessage(HomeViewModel model)`

Receives the contact form submission from the public portfolio page.

- Validates `ModelState` with `[ValidateAntiForgeryToken]`.
- Looks up the target user by `model.TargetUserId`.
- Creates a `ContactMessage` linked to that user.
- On success: redirects to `/{username}` with a `TempData["SuccessMessage"]`.
- On failure: re-fetches all portfolio data and re-renders the `Index` view with validation errors.

---

## 8. Admin Area

All controllers in the Admin area share:
- `[Area("Admin")]` attribute
- `[Authorize]` attribute (all actions require authentication)
- Access to `ApplicationDbContext` and `UserManager<ApplicationUser>`
- A `GetCurrentUserId()` helper that reads the current user's integer ID from Identity claims

### 8.1 AuthController

**File:** [`Areas/Admin/Controllers/AuthController.cs`](Areas/Admin/Controllers/AuthController.cs)

Route prefix: `Admin/Auth/[action]`

| Action | Method | Route | Access | Description |
|---|---|---|---|---|
| `Login` | GET | `/Admin/Auth/Login` | `[AllowAnonymous]` | Shows login form. Redirects to dashboard if already authenticated |
| `Login` | POST | `/Admin/Auth/Login` | `[AllowAnonymous]` | Signs in via `PasswordSignInAsync` with lockout enabled |
| `Register` | GET | `/Admin/Auth/Register` | `[AllowAnonymous]` | Shows registration form |
| `Register` | POST | `/Admin/Auth/Register` | `[AllowAnonymous]` | Creates user, assigns `Admin` role, auto-signs in |
| `Logout` | POST | `/Admin/Auth/Logout` | Authorized | Signs out, redirects to public home |
| `AccessDenied` | GET | `/Admin/Auth/AccessDenied` | `[AllowAnonymous]` | Rendered when unauthenticated user hits a protected route |

**Open Redirect protection:** Login success uses `RedirectToSafeUrl()` which checks `Url.IsLocalUrl(returnUrl)` before redirecting.

**Generic error messages:** Login failures use a vague "Invalid login attempt" message to prevent email enumeration attacks.

---

### 8.2 DashboardController

**File:** [`Areas/Admin/Controllers/DashboardController.cs`](Areas/Admin/Controllers/DashboardController.cs)

**`GET /Admin/Dashboard`**

The main landing page after login. Passes the following to `ViewBag`:

| ViewBag Key | Description |
|---|---|
| `ProjectCount` | Total projects for this user |
| `SkillCount` | Total skills |
| `CredentialCount` | Total credentials |
| `TimelineCount` | Total timeline events |
| `ProfileCount` | Total platform profiles |
| `TotalMessages` | All contact messages |
| `UnreadMessages` | Unread contact messages |
| `RecentProjects` | Last 5 projects by `CreatedAt` |
| `RecentMessages` | Last 5 non-archived messages by `SentDate` |

---

### 8.3 ProfileController

**File:** [`Areas/Admin/Controllers/ProfileController.cs`](Areas/Admin/Controllers/ProfileController.cs)

Allows the logged-in user to edit their own `ApplicationUser` record.

| Action | Method | Route | Description |
|---|---|---|---|
| `Index` | GET | `/Admin/Profile` | Loads current user fields into `EditProfileViewModel` |
| `Index` | POST | `/Admin/Profile` | Updates user fields, calls `UserManager.UpdateAsync`, refreshes sign-in cookies |

**Fields editable:**
- Full Name, Job Title, Bio, Profile Image URL, GitHub URL, LinkedIn URL, Availability Status

After a successful update, calls `SignInManager.RefreshSignInAsync()` to keep the auth cookie in sync with updated user data.

---

### 8.4 ProjectsController

**File:** [`Areas/Admin/Controllers/ProjectsController.cs`](Areas/Admin/Controllers/ProjectsController.cs)

Full CRUD for `Project` entities. All operations are scoped to the current user's `ApplicationUserId`.

| Action | Method | Route | Description |
|---|---|---|---|
| `Index` | GET | `/Admin/Projects` | List all user's projects |
| `Details` | GET | `/Admin/Projects/Details/{id}` | View project + linked skills |
| `Create` | GET | `/Admin/Projects/Create` | Show create form with skill checkboxes |
| `Create` | POST | `/Admin/Projects/Create` | Save project + create `ProjectSkill` rows |
| `Edit` | GET | `/Admin/Projects/Edit/{id}` | Show edit form with pre-selected skills |
| `Edit` | POST | `/Admin/Projects/Edit/{id}` | Update project, replace all `ProjectSkill` rows |
| `Delete` | GET | `/Admin/Projects/Delete/{id}` | Confirm delete view |
| `Delete` | POST | `/Admin/Projects/Delete/{id}` | Delete project (cascades `ProjectSkill`) |

**Skill association strategy (Create/Edit):**
- On **Create**: Save the project first, then insert each selected skill into `ProjectSkills`.
- On **Edit**: Delete all existing `ProjectSkill` rows for the project, then re-insert from the submitted `selectedSkillIds` array (replace-all strategy).

**Validation:** `DisplayOrder` uniqueness is checked via an `AnyAsync` call before save.

---

### 8.5 CredentialsController

**File:** [`Areas/Admin/Controllers/CredentialsController.cs`](Areas/Admin/Controllers/CredentialsController.cs)

Full CRUD for `Credential` entities. Follows the same structure as `ProjectsController`.

| Action | Method | Route | Description |
|---|---|---|---|
| `Index` | GET | `/Admin/Credentials` | List all user's credentials |
| `Details` | GET | `/Admin/Credentials/Details/{id}` | View credential + linked skills |
| `Create` | GET | `/Admin/Credentials/Create` | Form with skill checkboxes |
| `Create` | POST | `/Admin/Credentials/Create` | Save credential + `CredentialSkill` rows |
| `Edit` | GET | `/Admin/Credentials/Edit/{id}` | Pre-selected skill checkboxes |
| `Edit` | POST | `/Admin/Credentials/Edit/{id}` | Update credential, replace `CredentialSkill` rows |
| `Delete` | GET | `/Admin/Credentials/Delete/{id}` | Confirm delete |
| `Delete` | POST | `/Admin/Credentials/Delete/{id}` | Delete credential |

---

### 8.6 SkillsController

**File:** [`Areas/Admin/Controllers/SkillsController.cs`](Areas/Admin/Controllers/SkillsController.cs)

Full CRUD for `Skill` entities.

| Action | Method | Route | Description |
|---|---|---|---|
| `Index` | GET | `/Admin/Skills` | List skills grouped by category |
| `Details` | GET | `/Admin/Skills/Details/{id}` | View skill details |
| `Create` | GET | `/Admin/Skills/Create` | Skill creation form |
| `Create` | POST | `/Admin/Skills/Create` | Save skill (validates `DisplayOrder` uniqueness) |
| `Edit` | GET | `/Admin/Skills/Edit/{id}` | Pre-filled edit form |
| `Edit` | POST | `/Admin/Skills/Edit/{id}` | Update skill (validates `DisplayOrder` uniqueness) |
| `Delete` | GET | `/Admin/Skills/Delete/{id}` | Confirm delete |
| `Delete` | POST | `/Admin/Skills/Delete/{id}` | Delete skill |

> **Note:** Deleting a skill that is currently linked to any Project or Credential will fail at the database level due to the `Restrict` cascade policy. The admin must remove those associations from the Project/Credential edit pages first.

---

### 8.7 TimelineEventsController

**File:** [`Areas/Admin/Controllers/TimelineEventsController.cs`](Areas/Admin/Controllers/TimelineEventsController.cs)

Full CRUD for `TimelineEvent` entities (work history, education, awards, etc.).

| Action | Method | Route | Description |
|---|---|---|---|
| `Index` | GET | `/Admin/TimelineEvents` | List all events |
| `Details` | GET | `/Admin/TimelineEvents/Details/{id}` | View event details |
| `Create` | GET/POST | `/Admin/TimelineEvents/Create` | Create new event |
| `Edit` | GET/POST | `/Admin/TimelineEvents/Edit/{id}` | Update event |
| `Delete` | GET/POST | `/Admin/TimelineEvents/Delete/{id}` | Delete event |

---

### 8.8 PlatformProfilesController

**File:** [`Areas/Admin/Controllers/PlatformProfilesController.cs`](Areas/Admin/Controllers/PlatformProfilesController.cs)

Full CRUD for `PlatformProfile` entities (Codeforces, LeetCode, GitHub, etc.).

| Action | Method | Route | Description |
|---|---|---|---|
| `Index` | GET | `/Admin/PlatformProfiles` | List all profiles |
| `Details` | GET | `/Admin/PlatformProfiles/Details/{id}` | View profile |
| `Create` | GET/POST | `/Admin/PlatformProfiles/Create` | Create new profile |
| `Edit` | GET/POST | `/Admin/PlatformProfiles/Edit/{id}` | Update profile |
| `Delete` | GET/POST | `/Admin/PlatformProfiles/Delete/{id}` | Delete profile |

---

### 8.9 ContactMessagesController

**File:** [`Areas/Admin/Controllers/ContactMessagesController.cs`](Areas/Admin/Controllers/ContactMessagesController.cs)

Read-only inbox for messages submitted via the public portfolio contact form.

| Action | Method | Route | Description |
|---|---|---|---|
| `Index` | GET | `/Admin/ContactMessages` | List all messages (with unread indicator) |
| `Details` | GET | `/Admin/ContactMessages/Details/{id}` | View message, marks as read automatically |
| `Delete` | GET/POST | `/Admin/ContactMessages/Delete/{id}` | Confirm and delete message |

> Messages cannot be edited; they are submitted by site visitors and are read-only for the admin.

---

## 9. ViewModels

### `HomeViewModel` (Public)
**File:** [`ViewModels/HomeViewModel.cs`](ViewModels/HomeViewModel.cs)

Aggregates all data for the public portfolio page plus the contact form fields.

| Property | Type | Description |
|---|---|---|
| `AdminUser` | `ApplicationUser?` | The portfolio owner's user record |
| `TargetUserId` | `int` | Hidden field for message routing |
| `TargetUsername` | `string` | The username in the URL |
| `Projects` | `List<Project>` | With `ProjectSkills → Skill` eagerly loaded |
| `Skills` | `List<Skill>` | Ordered by category then display order |
| `Credentials` | `List<Credential>` | With `CredentialSkills → Skill` eagerly loaded |
| `TimelineEvents` | `List<TimelineEvent>` | |
| `PlatformProfiles` | `List<PlatformProfile>` | |
| `SenderName` | `string` | Contact form — Required, Max 100 |
| `SenderEmail` | `string` | Contact form — Required, Email format |
| `Subject` | `string` | Contact form — Required, Max 100 |
| `Body` | `string` | Contact form — Required, Max 2000 |

---

### `LoginViewModel`
**File:** [`Areas/Admin/ViewModels/LoginViewModel.cs`](Areas/Admin/ViewModels/LoginViewModel.cs)

| Property | Validation |
|---|---|
| `Email` | Required, Email format |
| `Password` | Required |
| `RememberMe` | `bool` |
| `ReturnUrl` | `string?` |

---

### `RegisterViewModel`
**File:** [`Areas/Admin/ViewModels/RegisterViewModel.cs`](Areas/Admin/ViewModels/RegisterViewModel.cs)

| Property | Validation |
|---|---|
| `Username` | Required, Max 50, alphanumeric + `_` |
| `Email` | Required, Email format |
| `FullName` | Required, Max 100 |
| `Password` | Required, Min 16, must meet Identity policy |
| `ConfirmPassword` | Must match `Password` |

---

### `EditProfileViewModel`
**File:** [`Areas/Admin/ViewModels/EditProfileViewModel.cs`](Areas/Admin/ViewModels/EditProfileViewModel.cs)

| Property | Validation |
|---|---|
| `FullName` | Required, Max 100 |
| `JobTitle` | Optional, Max 100 |
| `Bio` | Required, Max 2000 |
| `ProfileImageUrl` | Optional, URL format, Max 500 |
| `GitHubUrl` | Optional, URL format, Max 500 |
| `LinkedInUrl` | Optional, URL format, Max 500 |
| `AvailabilityStatus` | Required, `EAvailabilityStatus` enum |

---

## 10. Views & Layouts

### Public Layouts

| File | Used By | Description |
|---|---|---|
| `Views/Shared/_Layout.cshtml` | All public views | Fixed navbar (Projects, Skills, Credentials, Timeline, Profiles, Contact), footer, Bootstrap + CDN icons |
| `Views/Home/Index.cshtml` | `HomeController.Index` | Full single-page portfolio with all sections |

### Admin Layouts

| File | Used By | Description |
|---|---|---|
| `Areas/Admin/Views/Shared/_AdminLayout.cshtml` | All admin CRUD views | Sidebar navigation, top bar, admin-specific styling |
| `Areas/Admin/Views/Shared/_AdminAuthLayout.cshtml` | Login, Register, AccessDenied | Minimal centered layout for auth screens |

### Admin Views Structure

Each resource has the standard 5 views:
```
{Resource}/
├── Index.cshtml    — Data table with action buttons
├── Details.cshtml  — Read-only record view
├── Create.cshtml   — New record form
├── Edit.cshtml     — Edit record form
└── Delete.cshtml   — Delete confirmation
```

**Skill checkboxes** on Create/Edit views for `Projects` and `Credentials`:
- Skills are fetched and passed via `ViewBag.Skills`.
- Currently selected IDs are passed via `ViewBag.SelectedSkillIds`.
- Checkboxes are rendered grouped by skill category.
- On form submit, selected skill IDs are bound via `int[] selectedSkillIds`.

---

## 11. Static Assets & Styling

### CSS Files

| File | Purpose |
|---|---|
| `wwwroot/css/site.css` | Public portfolio styles: dark theme, grid background, navbar, hero, cards, sections |
| `wwwroot/css/admin.css` | Admin panel styles: sidebar, top bar, data tables, stat cards |
| `wwwroot/css/admin-auth.css` | Login/Register page: centered card, gradient background |

### External CDN Resources

All loaded via `<link>` tags in the respective layout files:

| Resource | URL | Purpose |
|---|---|---|
| Devicon | `cdn.jsdelivr.net/gh/devicons/devicon@latest/devicon.min.css` | Skill icons (language/framework logos) |
| FontAwesome 6 | `cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css` | General UI icons |
| Google Fonts — Geist | `fonts.googleapis.com` | Primary body font (400, 600) |
| Google Fonts — JetBrains Mono | `fonts.googleapis.com` | Code/mono font (400, 600, 700) |
| Google Fonts — Material Symbols | `fonts.googleapis.com` | Material icon font |

**Resource hints** (`preconnect`/`dns-prefetch`) are declared for `fonts.googleapis.com`, `fonts.gstatic.com`, `cdn.jsdelivr.net`, and `cdnjs.cloudflare.com` to reduce connection setup latency.

### Local Libraries (`wwwroot/lib/`)

- **Bootstrap 5** — CSS framework + JS bundle
- **jQuery** — DOM utilities (used by Bootstrap and ASP.NET validation scripts)

---

## 12. Performance Optimizations

The following optimizations are applied in `Program.cs` and across controllers:

### Response Compression
```csharp
builder.Services.AddResponseCompression(options => { options.EnableForHttps = true; });
app.UseResponseCompression();
```
Compresses HTTP responses (Gzip/Brotli) for all routes including HTTPS.

### Static File Caching
```csharp
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 365; // 1 year
        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
    }
});
```
All static files (CSS, JS, images) are served with a **1-year `Cache-Control`** header. The `asp-append-version="true"` tag helper appends a content hash to local asset URLs, ensuring cache busting when files change.

### No-Tracking Queries
All read-only database queries use `.AsNoTracking()` across the entire application:
- `HomeController` — all public portfolio queries
- All Admin CRUD `Index`, `Details`, and GET `Edit` actions

This prevents EF Core from attaching returned entities to the change tracker, significantly reducing memory usage and CPU time for large datasets.

### Lazy Image Loading
Project cover images in the public `Index.cshtml` view use `loading="lazy"` to defer offscreen image loading.

---

## 13. Security Model

| Concern | Implementation |
|---|---|
| **Authentication** | ASP.NET Core Identity cookie auth |
| **Authorization** | `[Authorize]` on all admin controllers/actions |
| **CSRF Protection** | `[ValidateAntiForgeryToken]` on all POST actions |
| **Data Isolation** | Every query filters by `ApplicationUserId == currentUserId` — users cannot see or modify each other's data |
| **Open Redirect** | Login success uses `Url.IsLocalUrl()` check before redirecting |
| **Email Enumeration** | Generic "Invalid login attempt" message regardless of whether email or password is wrong |
| **Brute-Force** | Account lockout after 5 failed attempts for 15 minutes |
| **Password Strength** | Minimum 16 chars, requires digit, uppercase, and non-alphanumeric |
| **Cookie Security** | `HttpOnly`, `SameSite=Strict`, `Secure=Always` in production |
| **XSS** | Razor's `@` expression output is HTML-encoded by default |
| **SQL Injection** | Parameterized queries via Entity Framework Core |

---

## 14. Enumerations Reference

### `EAvailabilityStatus`

| Enum Value | Display Name |
|---|---|
| `OpenForWork` | Open to Opportunities |
| `Employed` | Currently Employed |
| `Freelancing` | Accepting Freelance/Contracts |
| `Building` | In Stealth Mode / Building |
| `Unavailable` | Unavailable |

---

### `ESkillCategory`

| Enum Value | Display Name |
|---|---|
| `Backend` | Backend Development |
| `Frontend` | Frontend Development |
| `Mobile` | Mobile Development |
| `ArtificialIntelligence` | Artificial Intelligence & LLMs |
| `SoftwareDesign` | Software Design & Architecture |
| `Cybersecurity` | Cybersecurity & InfoSec |
| `CloudComputing` | Cloud Computing |
| `DevOps` | DevOps & CI/CD |
| `Database` | Database Management |
| `SystemsAdministration` | Systems Administration |
| `Hardware` | Hardware & Infrastructure |
| `CompetitiveProgramming` | Competitive Programming |
| `Networking` | Networking |
| `DataScience` | Data Science & Analytics |
| `GameDevelopment` | Game Development |
| `QualityAssurance` | Quality Assurance & Testing |
| `UIUXDesign` | UI/UX Design |
| `ProjectManagement` | Project Management |

---

### `ETimelineEventType`

| Enum Value | Display Name |
|---|---|
| `Work` | Work Experience |
| `Education` | Education |
| `Award` | Award / Recognition |
| `Volunteer` | Volunteer / Open Source |
| `Other` | Other |

---

## 15. Complete Route Reference

### Public Routes

| Method | Pattern | Controller | Action | Description |
|---|---|---|---|---|
| GET | `/` | Home | Index | Default portfolio (first Admin user) |
| GET | `/{username}` | Home | Index | Portfolio for specific username |
| POST | `/Home/SubmitMessage` | Home | SubmitMessage | Contact form submission |
| GET | `/Home/Error` | Home | Error | Error page |

### Admin Auth Routes

| Method | Pattern | Controller | Action |
|---|---|---|---|
| GET | `/Admin/Auth/Login` | Auth | Login (GET) |
| POST | `/Admin/Auth/Login` | Auth | Login (POST) |
| GET | `/Admin/Auth/Register` | Auth | Register (GET) |
| POST | `/Admin/Auth/Register` | Auth | Register (POST) |
| POST | `/Admin/Auth/Logout` | Auth | Logout |
| GET | `/Admin/Auth/AccessDenied` | Auth | AccessDenied |

### Admin CRUD Routes (pattern: `/Admin/{Resource}/{Action}/{id?}`)

| Resource | Index | Details | Create | Edit | Delete |
|---|---|---|---|---|---|
| Dashboard | `/Admin/Dashboard` | — | — | — | — |
| Profile | `/Admin/Profile` | — | — | — | — |
| Projects | `/Admin/Projects` | `/Admin/Projects/Details/{id}` | `/Admin/Projects/Create` | `/Admin/Projects/Edit/{id}` | `/Admin/Projects/Delete/{id}` |
| Skills | `/Admin/Skills` | `/Admin/Skills/Details/{id}` | `/Admin/Skills/Create` | `/Admin/Skills/Edit/{id}` | `/Admin/Skills/Delete/{id}` |
| Credentials | `/Admin/Credentials` | `/Admin/Credentials/Details/{id}` | `/Admin/Credentials/Create` | `/Admin/Credentials/Edit/{id}` | `/Admin/Credentials/Delete/{id}` |
| TimelineEvents | `/Admin/TimelineEvents` | `/Admin/TimelineEvents/Details/{id}` | `/Admin/TimelineEvents/Create` | `/Admin/TimelineEvents/Edit/{id}` | `/Admin/TimelineEvents/Delete/{id}` |
| PlatformProfiles | `/Admin/PlatformProfiles` | `/Admin/PlatformProfiles/Details/{id}` | `/Admin/PlatformProfiles/Create` | `/Admin/PlatformProfiles/Edit/{id}` | `/Admin/PlatformProfiles/Delete/{id}` |
| ContactMessages | `/Admin/ContactMessages` | `/Admin/ContactMessages/Details/{id}` | — | — | `/Admin/ContactMessages/Delete/{id}` |

---

*Documentation generated for commit state as of June 2026.*
