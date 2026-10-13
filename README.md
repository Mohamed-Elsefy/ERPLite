# ERPLite — Lightweight Enterprise Resource Planning

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?style=flat-square&logo=dotnet)
![EF Core](https://img.shields.io/badge/Entity_Framework_Core-10-512BD4?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019%2B-CC2927?style=flat-square&logo=microsoftsqlserver)
![FluentValidation](https://img.shields.io/badge/FluentValidation-12-7A86B8?style=flat-square)
![QuestPDF](https://img.shields.io/badge/QuestPDF-PDF_Reports-2E7D32?style=flat-square)
![License](https://img.shields.io/badge/License-All_Rights_Reserved-lightgrey?style=flat-square)

**ERPLite** is a modular, lightweight ERP system covering HR, Inventory, Sales & Finance, and Reporting — built with **ASP.NET Core MVC** on a clean 5-layer architecture with **Entity Framework Core**, **ASP.NET Core Identity** (role-based access), **FluentValidation**, **AutoMapper**, and **QuestPDF** report generation.

---

## ✨ Key Features

### 👥 Human Resources
- Departments and employee profiles with department-scoped manager visibility
- Daily attendance (check-in / check-out) with duplicate prevention (unique `EmployeeId + Date` index)
- Employee ⇄ Identity account linking, role promotion, lock / unlock

### 📦 Inventory & Procurement
- Categories, suppliers, and products with minimum-stock alerts
- Server-side search (name / category / supplier) and paginated listings
- Deletion protection for products referenced by historic orders

### 💰 Sales & Finance
- Customers, orders with line items, and installment-friendly partial payments
- Automatic order payment-status transitions (`Unpaid → PartiallyPaid → Paid`) and remaining-balance enforcement
- Per-order financial summaries and a global receipts ledger

### 📊 Reports & Dashboards
- Role-based dashboards (Admin / Manager / Employee) with sales analytics and attendance hubs
- PDF export for employees, inventory, sales, financial, and attendance reports (async rendering)
- Executive financial summary (revenue, collected, outstanding)

### 🛡️ System & Security
- ASP.NET Core Identity with three roles — `Admin`, `Manager`, `Employee` — enforced via authorization policies
- Full audit trail (`ActivityLogs`) and user notifications with centralized action/type constants
- Friendly error pages for browsers, JSON errors for API/AJAX callers (correlation via `RequestId`)

---

## 🏗️ Architecture

Strict separation of concerns across five layers. Dependencies flow **one way**:

```text
ERPLite.slnx
│
├── ERPLite.Web            Presentation (MVC, Areas: Admin / Manager / Employee)
│   ├── Controllers / Areas / Views / Models (ViewModels)
│   ├── Mapping (DTO ↔ ViewModel), Middlewares, Extensions, Helpers
│   └── wwwroot (Bootstrap, custom CSS/JS)
│
├── ERPLite.Services       Business logic + validation + orchestration
│   ├── Interfaces / Services (per module) / DTOs / Validators / Mapping
│   └── Helpers (ServiceResult, PaginationMapper, ValidationResultExtensions)
│
├── ERPLite.Repositories   Data access (EF Core)
│   ├── GenericRepository + specific repositories + UnitOfWork
│   └── Extensions (server-side pagination / search / sorting)
│
├── ERPLite.Data           Persistence model
│   ├── DbContext, Entities, Fluent-API Configurations, Seeders, Migrations
│   └── DependencyInjection (persistence composition root)
│
└── ERPLite.Shared         Cross-cutting primitives
    ├── Constants (Roles, SystemModules, AuditActions, NotificationTypes, UiKeys, …)
    ├── Enums, Helpers
    └── (reserved) Exceptions / Results / Pagination / Responses
```

**Engineering principles applied**

| Principle | How |
|---|---|
| Layered Clean Architecture | `Web → Services → Repositories → Data`; `Web` never touches repositories directly |
| Explicit persistence boundary | `AddPersistence()` / `SeedDatabaseAsync()` extensions — no `DbContext` leakage into `Web` |
| Unit of Work + transactions | Single commit per transaction, async dispose, `CancellationToken` support |
| Validation pipeline | `FluentValidation` validators for every Create/Update DTO, surfaced through `ServiceResult` (no exception-driven flow) |
| Read-path performance | `AsNoTracking` reads, server-side paging (`Skip/Take` + `Count`), bounded role lookups, async PDF rendering |
| No magic strings | Audit actions, notification types/priorities, TempData/ViewData keys, and display fallbacks live in `Shared.Constants` |
| Safe diagnostics | Structured `ILogger<T>` everywhere; user-facing messages are generic, details stay in logs with correlation IDs |

---

## 🛠️ Tech Stack

| Area | Technology |
|---|---|
| Framework | .NET 10, ASP.NET Core MVC (Areas + TagHelpers + ViewComponents) |
| Data | Entity Framework Core 10, SQL Server |
| Auth | ASP.NET Core Identity (cookies, lockout, seeded roles/users in Development) |
| Validation | FluentValidation 12 (service layer) + DataAnnotations (view models) |
| Mapping | AutoMapper 16 (layer-separated profiles) |
| Reporting | QuestPDF (Community license) |
| UI | Bootstrap 5, Bootstrap Icons, jQuery Validation |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full — any edition reachable via connection string)

### 1. Clone & Configure
```bash
git clone <your-repo-url>
cd ERPLite
```

Edit the connection string in `ERPLite.Web/appsettings.json`
(or override it without touching files):
```bash
dotnet user-secrets init --project ERPLite.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=.;Initial Catalog=ERPLite;Integrated Security=True;Encrypt=False;Trust Server Certificate=True" --project ERPLite.Web
```

### 2. Create the Database
```bash
dotnet ef database update --project ERPLite.Data --startup-project ERPLite.Web
```
Or from Visual Studio's Package Manager Console (Default Project: `ERPLite.Data`):
```powershell
Update-Database
```

### 3. Run
```bash
dotnet run --project ERPLite.Web
# http://localhost:5288  (Development)
```

On first **Development** run, demo roles and users are seeded automatically
(production startup never seeds — see [Configuration](#-configuration)):

| Role | Email | Password |
|---|---|---|
| Admin | `admin@erplite.com` | `Admin@123` |
| Manager | `manager@erplite.com` | `Manager@123` |
| Employee | `employee@erplite.com` | `Employee@123` |

---

## ⚙️ Configuration

| Concern | How |
|---|---|
| Environments | `appsettings.json` (base) + `appsettings.Development.json` (local). `ASPNETCORE_ENVIRONMENT` selects the profile |
| Secrets | `appsettings.Development.json` is git-ignored — put real secrets in User Secrets or env vars, e.g. `SeedUsers__Admin__Password` |
| Seed users | `SeedUsers:{Admin,Manager,Employee}:{FullName,Email,Password}` — Development only |
| Auth cookie | `SecurePolicy.SameAsRequest` in Development, `Always` otherwise; 7-day sliding expiration |
| Pagination | `PagedRequestDto.PageSize` is clamped to 1–50 (`MaxPageSize`) |
| Error handling | Browsers are redirected to `/Error?code=&requestId=`; API/AJAX callers receive JSON |

---

## 🗺️ Roadmap

- [x] Phase 0 — Solution scaffolding and architecture
- [x] Phase 1 — Database layer (Identity, domain entities, Fluent-API safeguards)
- [x] Phase 2 — Repository layer + Unit of Work
- [x] Phase 3 — Service layer, DTOs, FluentValidation
- [x] Phase 4 — MVC presentation, Identity wiring, dashboards
- [ ] Phase 5 — AI analytical engine (`AIReports` / `AILogs` tables are already modeled)
- [ ] Backlog — automated tests, Docker support, toast-notification component, PDF caching

---

## 🤝 Contributing

Direct pushes to `main` are blocked. For every change:

1. Create a feature branch: `feature/<short-description>`
2. Keep layers clean — business rules go in `Services` (+ a validator), queries in `Repositories`
3. Use `Shared.Constants` instead of inline strings for actions, notification types, and UI keys
4. Run a full solution build with **0 warnings** before opening a Pull Request:
   ```bash
   dotnet build ERPLite.slnx
   ```
