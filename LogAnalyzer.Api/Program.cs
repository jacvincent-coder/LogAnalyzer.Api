using LogAnalyzer.Api.Middleware;
using LogAnalyzer.Api.Services;
using LogAnalyzer.Api.Utils;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load API Key from config
var apiKey = builder.Configuration["ApiKey"];
Console.WriteLine("Loaded API Key: " + apiKey);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("X-API-Key");
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with API Key Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Log Analyzer API", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Use: X-API-Key: {your key}",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-API-Key"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// App services
builder.Services.AddSingleton<ILogParser, LogParser>();
builder.Services.AddScoped<ILogAnalyzerService, LogAnalyzerService>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowUI");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// API Key Middleware (GLOBAL)
app.UseMiddleware<ApiKeyMiddleware>();

// Keep your existing exception handling middleware 
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();

public partial class Program { }
