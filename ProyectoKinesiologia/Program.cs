using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ProyectoKine.BD.Contexto;
using ProyectoKine.Repositorio.Repositorios;
using ProyectoKine.Servicio.Servicios;
using ProyectoKinesiologia.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Base de datos: SQL Server (LocalDB en desarrollo) con Entity Framework Core.
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------------------------------------------------------------------------
// Inyección de dependencias: repositorios y servicios de negocio.
// ---------------------------------------------------------------------------
builder.Services.AddScoped<IPacienteRepositorio, PacienteRepositorio>();
builder.Services.AddScoped<ITurnoRepositorio, TurnoRepositorio>();
builder.Services.AddScoped<IPacienteServicio, PacienteServicio>();
builder.Services.AddScoped<ITurnoServicio, TurnoServicio>();

// ---------------------------------------------------------------------------
// Controladores y Swagger.
// ---------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KineTurnos API",
        Version = "v1",
        Description = "API para la gestión de turnos de una kinesióloga: pacientes, agenda, " +
                      "edición y cancelación de turnos."
    });

    // Incluye los comentarios XML de los controllers en la documentación.
    var nombreXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var rutaXml = Path.Combine(AppContext.BaseDirectory, nombreXml);
    if (File.Exists(rutaXml))
    {
        opciones.IncludeXmlComments(rutaXml);
    }
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// Middleware y archivos estáticos (frontend HTML/CSS/JS).
// ---------------------------------------------------------------------------
app.UseMiddleware<ManejadorExcepcionesMiddleware>();

app.UseDefaultFiles();   // Sirve index.html al entrar a la raíz.
app.UseStaticFiles();    // Sirve los archivos de wwwroot.

// Documentación interactiva de la API.
app.UseSwagger();
app.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "KineTurnos API v1");
    opciones.DocumentTitle = "KineTurnos API";
});

app.MapControllers();

// ---------------------------------------------------------------------------
// En desarrollo se aplican automáticamente las migraciones pendientes.
// ---------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    contexto.Database.Migrate();
    app.Logger.LogInformation("Base de datos verificada y migraciones aplicadas.");
}

app.Run();
