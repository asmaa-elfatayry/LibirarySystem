# 📚 LibrarySystem — Library Management System

A full-featured **Library Management System** built with **ASP.NET Core MVC**, following **Clean Architecture** principles. Built as a learning project, focused on production-grade patterns rather than a simple CRUD demo.

---

## 🏗️ Architecture

The solution follows a **layered Clean Architecture** with strict dependency rules:

```
LibrarySystem.sln
│
├── LibrarySystem.Domain          → Entities, Enums, Repository Interfaces (no dependencies)
├── LibrarySystem.Application     → DTOs, Service Interfaces, Result pattern, Mapping profiles
├── LibrarySystem.Infrastructure  → EF Core, Repositories, Services, Identity, Background Jobs
└── LibrarySystem.Web (MVC)       → Controllers, Views, ViewModels, Dependency Injection wiring
```

**Dependency rule:** `Web → Application → Domain` and `Infrastructure → Application → Domain`. The Domain layer has zero external dependencies.

### Design Patterns Used

| Pattern | Where | Why |
|---|---|---|
| **Generic Repository** | `IGenericRepository<T>` | Shared CRUD logic across all entities, with support for `Include()` via `Expression<Func<T, object>>[]` |
| **Generic Service** | `IGenericService<T>` | Business-logic wrapper around repositories; avoids duplicating CRUD services per entity |
| **Unit of Work** | `IUnitOfWork` | Coordinates multi-repository transactions (`Loan` + `BookCopy` + `Reservation` + `Fine`) with a single `SaveChangesAsync()` |
| **Result Pattern** | `Result` / `Result<T>` | Explicit success/failure signaling from services instead of exceptions for expected business failures |
| **DTO Layer** | `Application.DTOs` | Decouples Entities from the Web layer; prevents over-posting and leaking EF navigation properties |
| **AutoMapper Profiles** | `MappingProfile.cs` | Entity ↔ ViewModel/DTO conversion without manual boilerplate |
| **Soft Delete** | `BaseEntity.IsDeleted` | Records are flagged, not physically removed, preserving historical/loan data integrity |

---

## 🧩 Core Domain Features

### Catalog Management
- **Categories, Authors, Publishers** — full CRUD with AJAX-based deletion (SweetAlert2 confirmation)
- **Books** — CRUD with cover image upload, ISBN, publication year, and dropdowns for Author/Category/Publisher
- **Book Copies** — each physical copy is tracked independently (`Available`, `Borrowed`, `UnderMaintenance`, `Lost`), managed from the Book Details page with inline status editing (select → confirm → save)

### Circulation
- **Loans (Borrowing/Returning)**
  - Staff select a member and a book; the system auto-assigns an available copy
  - Blocks borrowing if no copy is available or the member has unpaid fines
  - Returning a book auto-updates the copy status and checks the reservation queue
- **Reservations**
  - Members can reserve a book with zero available copies, from their own account
  - **FIFO auto-fulfillment**: when a reserved book is returned, the system automatically converts the oldest pending reservation into a new loan — all inside a single `UnitOfWork` transaction
- **Fines**
  - Automatically calculated on late returns (`daysLate × rate`)
  - Staff can mark fines as paid
  - Members with unpaid fines are blocked from new loans

---

## 🔐 Authentication & Authorization

Built on **ASP.NET Core Identity** with `Guid`-based keys and full custom flows (no scaffolded defaults):

- Register / Login / Logout with a custom `AccountController`
- **Email confirmation** — required before login is allowed (`SignIn.RequireConfirmedEmail`), sent via a real SMTP email service (`IEmailSender` abstraction, swappable implementation)
- **Forgot / Reset Password** — token-based flow, protected against user-enumeration (same response whether the email exists or not)
- **Remember Me**
- **Google OAuth login** (`Microsoft.AspNetCore.Authentication.Google`) — links to existing accounts by email or creates a new one, with `EmailConfirmed = true` since Google already verifies it
- **Role-based authorization**: `Admin`, `Librarian`, `Member`, seeded at startup
- **User management panel** (Admin-only) — create staff accounts, change roles inline, with safeguards against self-demotion and removing the last remaining Admin
- Custom **403 / 404 / 500** error pages matching the site's design system

---

## 📊 Admin Dashboard

Real-time statistics built with **Chart.js**, pulling live data through existing services (no duplicated queries):

- Stat cards: total books, members, active loans, overdue loans, pending reservations, unpaid fines total
- **Line chart** — loans over the last 14 days
- **Bar chart** — top 5 most borrowed books
- **Doughnut chart** — book distribution by category

Chart colors are read dynamically from CSS custom properties, so they stay in sync with the site's light/dark theme automatically.

---

## ⚙️ Background Jobs

**Hangfire** (SQL Server storage) handles scheduled tasks:

- Daily recurring job (`Cron.Daily(9)`) that emails members whose loans are due the next day
- Hangfire Dashboard (`/hangfire`) is locked down with a custom `IDashboardAuthorizationFilter` — Admin-only access

---

## 📄 Reports

- **Excel export** (ClosedXML) for Books and Loans, RTL-formatted
- **PDF export** (QuestPDF) for Books, with a styled table (zebra striping, header background, page numbers), using a registered Arabic font (Cairo) for correct glyph shaping

---

## 🪵 Logging

**Serilog**, configured entirely through `appsettings.json`:

- Console + rolling daily file sink + SQL Server sink (auto-creates the `Logs` table)
- Structured logging (`{PropertyName}` placeholders, not string interpolation) for queryable log data
- Minimum level configurable per environment

---

## 🎨 Frontend

- Razor Views with a consistent design system: **Playfair Display** (headings), **Inter** (UI text), **Lora** (body copy), driven entirely by CSS custom properties (`--primary`, `--bg-primary`, `--text-secondary`, `--radius-*`, `--shadow-*`, ...)
- **Full dark mode** — synced with Bootstrap's native `data-bs-theme` alongside custom tokens, persisted in `localStorage`
- AJAX-driven interactions (SweetAlert2 confirmations) for delete actions and inline status updates — no full-page reloads for common actions
- Role-aware UI: navigation links and homepage content adapt based on the logged-in user's role

---

## 🔒 Security Practices

- Secrets (SMTP credentials, Google OAuth keys) are never committed — managed via **.NET User Secrets** in development
- `.gitignore` configured for `bin/`, `obj/`, `appsettings.*.json` (except the base file), uploaded files, and IDE artifacts
- Anti-forgery tokens on all state-changing POST requests
- Cascade delete behavior explicitly configured per relationship (`Restrict` for reference data, `Cascade` only for true dependent records like `BookCopy → Loan`)

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Auth | ASP.NET Core Identity + Google OAuth |
| Mapping | AutoMapper |
| Background Jobs | Hangfire |
| Logging | Serilog |
| Charts | Chart.js |
| Reports | ClosedXML (Excel), QuestPDF (PDF) |
| Frontend | Bootstrap 5, vanilla JS (modular per-feature files), SweetAlert2 |

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB or full instance)

### Setup

```bash
git clone <repo-url>
cd LibrarySystem

# Restore & build
dotnet restore
dotnet build
```

### Configure secrets (development)

```bash
cd LibrarySystem.Web
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:AppPassword" "your-gmail-app-password"
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"
```

Update the `DefaultConnection` string in `appsettings.json` to point to your SQL Server instance.

### Apply migrations & run

```bash
dotnet ef database update --project LibrarySystem.Infrastructure --startup-project LibrarySystem.Web
dotnet run --project LibrarySystem.Web
```

On first run, the app seeds:
- Roles: `Admin`, `Librarian`, `Member`
- A default Admin account (`admin@library.com`)

---

## 📌 Roadmap / Not Yet Implemented
- [ ] Localization (Arabic/English toggle)


---

## 📁 Project Structure Highlights

```
LibrarySystem.Domain/
  Entities/        → BaseEntity, Book, Author, Category, Publisher, BookCopy, Loan, Reservation, Fine
  Enums/           → CopyStatus, LoanStatus, ReservationStatus
  Interfaces/       → IGenericRepository<T>, IUnitOfWork

LibrarySystem.Application/
  DTOs/            → BookDto, LoanDto, ReservationDto, FineDto
  Interfaces/       → IGenericService<T>, ILoanService, IReservationService, IFineService, IEmailSender
  Common/          → Result, Result<T>

LibrarySystem.Infrastructure/
  Data/            → AppDbContext
  Repositories/     → GenericRepository<T>, UnitOfWork
  Services/        → GenericService<T>, LoanService, ReservationService, FineService, SmtpEmailSender
  Jobs/            → DueDateReminderJob
  Seed/            → DataSeeder

LibrarySystem.Web/
  Controllers/      → Category, Author, Publisher, Book, Loan, Reservation, Fine, User, Dashboard, Report, Account
  ViewModels/       → Per-entity view models + Account/Auth view models
  Mappings/         → MappingProfile
  wwwroot/css/      → Design-system CSS files (auth, cards, dashboard, error pages, admin forms)
  wwwroot/modules/  → Per-feature JavaScript (book, loan, user)
```

---

*Built as a hands-on learning project to practice Clean Architecture, EF Core, Identity, and real-world ASP.NET Core patterns beyond basic CRUD.*
