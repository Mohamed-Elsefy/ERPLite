using ERPLite.Data.Seeders;
using ERPLite.Repositories.DependencyInjection;
using ERPLite.Web.Extensions;
using ERPLite.Services.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using QuestPDF;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add QuestPDF
Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity Services
// this is Extension method for Identity Services
// Identity Services are added in the ERPLite.Web.Extensions.IdentityServices.cs file 
builder.Services.AddIdentityServices();

// Add Repositories 
builder.Services.AddRepositories();

// Add Service layer
builder.Services.AddServicesLayer();


var app = builder.Build();

// Seed Roles and Users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await IdentitySeeder
        .SeedRolesAndUsersAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
