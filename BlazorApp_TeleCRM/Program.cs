using BlazorApp_TeleCRM.Components;
using BlazorApp_TeleCRM.Models;
using BlazorApp_TeleCRM.Service;
using Blazored.LocalStorage;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Radzen;
using Radzen.Blazor;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register Radzen services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<ThemeService>(); // Register ThemeService
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<Radzen.ContextMenuService>();
builder.Services.AddScoped<Radzen.TooltipService>();
builder.Services.AddScoped<Radzen.SideDialogOptions>();
//builder.Services.AddScoped<ThemeServiceCustom>();
builder.Services.AddScoped<SharedStateService>();
builder.Services.AddHttpClient();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddControllers();


var connectionString_allseeddbPD = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext for EF Core with MySQL
builder.Services.AddDbContext<allseeddbPDContext>(options =>
    options.UseMySql(connectionString_allseeddbPD, ServerVersion.AutoDetect(connectionString_allseeddbPD)));


builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(connectionString_allseeddbPD));


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // ¢¹Ò´ä¿ÅìÊÙ§ÊØ´ 100 MB
});

// Add Authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied"; // Path to an access-denied page
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(180);
        options.SlidingExpiration = true;
    });

// Add Authorization services
builder.Services.AddAuthorizationCore();

// Add your custom authentication state provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();



app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // Ensure this matches your setup

app.MapControllers();

app.Run();
