using ERPLite.Data.Interceptors;
using ERPLite.Services.Interfaces.Auth;
using ERPLite.Services.Interfaces.Dashboard;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Services.Interfaces.Reports;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Services.Interfaces.System;
using ERPLite.Services.Reports.Services;
using ERPLite.Services.Services.Auth;
using ERPLite.Services.Services.Dashboard;
using ERPLite.Services.Services.HR;
using ERPLite.Services.Services.Inventory;
using ERPLite.Services.Services.Reports;
using ERPLite.Services.Services.Sales;
using ERPLite.Services.Services.System;
using ERPLite.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ERPLite.Services.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServicesLayer(this IServiceCollection services)
        {
            // register automapper
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            // register validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Auth
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            // HR
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAttendanceService, AttendanceService>();

            // Inventory
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IProductService, ProductService>();

            // Sales
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // Dashboard
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();

            // System
            services.AddScoped<IActivityLogService, ActivityLogService>();
            services.AddScoped<INotificationService, EmailNotificationService>();

            // Reports
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IPdfGenerator, PdfGenerator>();
            services.AddScoped<IReportService, ReportService>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            return services;
        }
    }
}