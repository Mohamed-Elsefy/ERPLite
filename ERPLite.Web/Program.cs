using ERPLite.Data.DependencyInjection;
using ERPLite.Repositories.DependencyInjection;
using ERPLite.Services.DependencyInjection;
using ERPLite.Web.Extensions;
using QuestPDF;
using QuestPDF.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add QuestPDF
Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Persistence (DbContext) - registered via ERPLite.Data extension, no DbContext usage here
builder.Services.AddPersistence(builder.Configuration);

// Add Identity Services
// Identity entity types (ApplicationUser/ApplicationRole) live in ERPLite.Data,
// referenced explicitly by ERPLite.Web for AddIdentity only (see .csproj comment).
builder.Services.AddIdentityServices(builder.Environment);

// Add Repositories
builder.Services.AddRepositories();

// Add Service layer
builder.Services.AddServicesLayer();

// Add AutoMapper for web layer
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));


var app = builder.Build();

// Seed roles and demo users in Development only - never in production.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.SeedDatabaseAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapStaticAssets();

app.UseRouting();

app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");


app.Run();
