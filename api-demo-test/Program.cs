using api_demo_test.Application;
using api_demo_test.Data;
using api_demo_test.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // La URL exacta de tu app Angular
                  .AllowAnyHeader()                     // Permitir headers como Content-Type, Authorization
                  .AllowAnyMethod();                    // Permitir GET, POST, PUT, DELETE, OPTIONS
        });
});

// Configurar Swagger/OpenAPI
// http://localhost:5237/swagger/index.html
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Survey API", Version = "v1", Description = "API REST Survey docs" });
});


// BD
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? "Host=localhost;Database=survey_app;Username=postgres;Password=admin2023";
builder.Services.AddDbContext<SurveyDbContext>(options => options.UseNpgsql(connectionString));

// Inyección de Dependencias (Servicios)
builder.Services.AddScoped<ISurveyService, SurveyService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapeo de rutas (desde el archivo SurveyEndpoints.cs)
app.MapSurveyRoutes();
app.UseCors("AllowAngularDev");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
