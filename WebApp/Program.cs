using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Services;
using WebApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with connection string from appsettings.json
// This ensures credentials are not hardcoded and can vary by environment
builder.Services.AddDbContext<SsgtContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Allow AJAX JSON posts to send the anti-forgery token in a request header.
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

// Add session support for storing menus after login
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Allow HTTP/HTTPS based on request
    options.Cookie.SameSite = SameSiteMode.Lax; // More permissive for local testing
});

// Add memory cache for menu caching
builder.Services.AddMemoryCache();

// Add HttpContextAccessor for error logging
builder.Services.AddHttpContextAccessor();

// Add error logging service
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();

// Add menu service
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ItemQuantityConversionHelper>();

// Register template-specific PDF services
builder.Services.AddScoped<WebApp.Services.Pdf.EstimatePdfService>();
builder.Services.AddScoped<WebApp.Services.Pdf.InvoiceInStatePdfService>();
// Out-of-state invoice PDF service (renders IGST-only taxable summary)
builder.Services.AddScoped<WebApp.Services.Pdf.InvoiceOutOfStatePdfService>();
// Register router/facade as the single IPdfService implementation
builder.Services.AddScoped<WebApp.Services.Pdf.IPdfService, WebApp.Services.Pdf.PdfServiceRouter>();
// Removed transitional renderer factory and renderer registrations - using PdfServiceRouter facade instead

// Add authentication with cookie scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Allow HTTP/HTTPS based on request
        options.Cookie.SameSite = SameSiteMode.Lax; // More permissive for local testing
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

// Add session middleware before authentication
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();

