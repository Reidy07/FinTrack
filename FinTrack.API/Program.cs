using FinTrack.API.Services;
using FinTrack.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar los controladores
builder.Services.AddControllers();

// 2. Registrar la capa de Infraestructura (Base de datos, UnitOfWork, Servicios)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        // Aquí luego pondrás el localhost de tu FinTrack.Web, ej: "https://localhost:7123"
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 4. Configurar Swagger para documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<DailyReminderService>();

var app = builder.Build();

// 5. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
