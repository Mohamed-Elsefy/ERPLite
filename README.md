# 🚀 ERPLite - Enterprise Resource Planning System

**ERPLite** is a lightweight, fully integrated Enterprise Resource Planning (ERP) system built on a multi-layered **Clean Architecture** utilizing **.NET 8** and **Entity Framework Core**. The system is architected for high scalability, financial precision, and enterprise-grade modularity, featuring native blueprints for AI integration.

---

## 🏗️ Solution Architecture

The solution is split into 5 distinct layers to enforce a strict **Separation of Concerns (SoC)**, ensuring independent deployability, easier testing, and clean maintainability:

```text
ERPLite.sln
│
├── ERPLite.Web              --> Presentation Layer (MVC Web App)
│   ├── Controllers/         --> Organized by modules and user roles
│   ├── Views/               --> UI Views for each sub-system
│   ├── ViewModels/          --> View-specific data transfer models
│   ├── Areas/               --> Role-based routing (Admin, Manager, Employee)
│   └── wwwroot/             --> Static assets (CSS, JS, Images, Libs)
│
├── ERPLite.Services         --> Business Logic Layer
│   ├── Interfaces/          --> Contract definitions for services per module
│   ├── Services/            --> Core business logic implementations
│   ├── DTOs/                --> Data Transfer Objects between layers
│   └── Validators/          --> Input data validation rules (FluentValidation)
│
├── ERPLite.Repositories     --> Data Access Layer
│   ├── Interfaces/          --> Repository interfaces (Generic & Custom)
│   ├── Repositories/        --> EF Core data access implementations
│   └── UnitOfWork/          --> Transaction management across repositories
│
├── ERPLite.Data             --> Database & Core EF Layer
│   ├── Context/             --> ApplicationDbContext
│   ├── Entities/            --> Core domain models & database tables
│   └── Configurations/      --> Fluent API entity mappings & relationships
│
└── ERPLite.Shared           --> Shared/Common Utilities
    ├── Responses/           --> Unified web response wrappers
    ├── Pagination/          --> Generic data pagination components
    └── Enums & Constants/   --> Solution-wide system constants & enums


🗺️ Project Roadmap & Execution Plan
The project follows a strict engineering timeline to ensure structural integrity and quality control:

[x] Phase 0: Architecture setup, project scaffolding, and folder structuring.

[x] Phase 1: Core Infrastructure & Database Layer Implementation (Current Status).

[ ] Phase 2: Repository Layer & Unit of Work implementation.

[ ] Phase 3: Service Layer, DTO mappings, and Core Business Validation.

[ ] Phase 4: Presentation Web UI, Controller logic, and Identity Authentication setup.

[ ] Phase 5: AI Analytical Engine integration and smart reporting.


💎 Core Infrastructure Features (Phase 1 Deliverables)
The foundation of the system has been successfully established within the ERPLite.Data project with the following architectural safeguards:

1. Identity & Security System
Custom integration of ASP.NET Core Identity utilizing ApplicationUser and ApplicationRole.

Enforced string-based Identity IDs adhering to secure framework defaults.

Built-in optional relationship between system users and physical employees (1 ↔ 0..1).

2. Core Domain Modules (Entities)
Human Resources (HR): Departments, Employees, and Attendance tracking tracking.

Inventory & Procurement: Product Categories, Suppliers, and Products with minimum stock level alert properties.

Sales & Finance: Customers, Orders, OrderItems, and multi-transaction Payment capabilities (supporting installments and partial payments).

AI Engine: Dedicated tables (AIReports, AILogs) tracking AI-generated insights and monitoring API token consumption.

System Utilities: System-wide directed Notifications and an enterprise ActivityLog engine for a complete audit trail.

3. Data Integrity & Database Safeguards
100% Fluent API Driven: Zero data annotation pollution in the domain models, separating database design from business entities.

Financial Precision: Strict Precision(18,2) constraints enforced on all financial metrics (Price, Salary, Amount, etc.). Floats/doubles are structurally banned for financial calculations.

Cascade Delete Protection: Configured DeleteBehavior.Restrict across sensitive transactional entities to protect structural history from accidental data cascades.

Double-Check In Prevention: Applied a Composite Unique Index combining (EmployeeId + Date) to structurally guarantee that an employee cannot log an attendance entry more than once per calendar day.


🚀 Getting Started for Developers
To configure and run this project locally on your development machine, follow these steps:

1. Clone the Repository
Bash
git clone
cd ERPLite
2. Configure the Connection String
Open the appsettings.json file inside the ERPLite.Web project and update the connection string for your local SQL Server instance:

JSON
"ConnectionStrings": {
  "DefaultConnection": "Data Source=.;Initial Catalog=ERPLite;Integrated Security=True;Encrypt=False;Trust Server Certificate=True"
}
3. Apply Database Migrations
Open the Package Manager Console in Visual Studio, set ERPLite.Data as your Default Project, and execute:

PowerShell
Update-Database
Alternatively, using the .NET CLI:

Bash
dotnet ef database update --project ERPLite.Data --startup-project ERPLite.Web
📜 Contribution & Branching Strategy
Direct pushes to the main branch are strictly blocked.

Create a dedicated feature branch for every architectural step (e.g., feature/repository-pattern).

Always perform a full solution Clean and Build to ensure there are no compilation compilation errors before submitting a Pull Request.