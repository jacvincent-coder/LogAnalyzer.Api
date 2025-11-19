using LogAnalyzer.Api.Services;
using LogAnalyzer.Api.Utils;

var builder = WebApplication.CreateBuilder(args);


// services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILogParser, LogParser>();
builder.Services.AddScoped<ILogAnalyzerService, LogAnalyzerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
