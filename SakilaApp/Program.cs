using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
using SakilaApp.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SECCIÓN DE SERVICIOS (BUILDER) ---

// AGREGAR SERVICIOS PARA CONTROLADORES CON VISTAS Y RAZOR PAGES
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// CONFIGURAR EL DBCONTEXT PARA SQL SERVER
builder.Services.AddDbContext<SakilaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CONFIGURAR IDENTITY CON IDENTITYUSER E IDENTITYROLE
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // NO REQUIERE CONFIRMACIÓN DE EMAIL
    options.Password.RequireDigit = true;           // REQUIERE AL MENOS UN NÚMERO
    options.Password.RequiredLength = 6;            // MÍNIMO 6 CARACTERES
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<SakilaContext>()
.AddDefaultTokenProviders();

// CONFIGURAR COOKIES PARA EL REDIRECCIONAMIENTO DE SEGURIDAD
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// REGISTRAR EL SERVICIO DE ENVÍO DE CORREOS (CONSOLA)
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, ConsoleEmailSender>();

var app = builder.Build();

// --- 2. SECCIÓN DE MIDDLEWARE (PIPELINE DE SOLICITUDES) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// EL ORDEN AQUÍ ES CRÍTICO: AUTENTICACIÓN ANTES QUE AUTORIZACIÓN
app.UseAuthentication();
app.UseAuthorization();

// CONFIGURAR LA RUTA POR DEFECTO (HOME/INDEX)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// MAPEO DE RAZOR PAGES (NECESARIO PARA IDENTITY)
app.MapRazorPages();

app.Run();