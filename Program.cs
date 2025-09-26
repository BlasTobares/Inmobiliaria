
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();


// Cache + Sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Importante: Session antes de Auth
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// ---- Seeder de usuarios iniciales ----
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var repo = new RepositorioUsuario(config.GetConnectionString("DefaultConnection")!);

    if (repo.ObtenerPorEmail("admin@inmo.local") == null)
    {
        var admin = new Usuario
        {
            Email = "admin@inmo.local",
            Nombre = "Admin",
            Apellido = "Principal",
            Rol = "Admin"
        };
        repo.Crear(admin, "Admin123!");
        Console.WriteLine("Usuario admin creado: admin@inmo.local / Admin123!");
    }

    if (repo.ObtenerPorEmail("empleado@inmo.local") == null)
    {
        var emp = new Usuario
        {
            Email = "empleado@inmo.local",
            Nombre = "Empleado",
            Apellido = "Demo",
            Rol = "Empleado"
        };
        repo.Crear(emp, "Empleado123!");
        Console.WriteLine("Usuario empleado creado: empleado@inmo.local / Empleado123!");
    }
}

app.Run();
