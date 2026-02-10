using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TranscriptAnalyzer.Api;
using TranscriptAnalyzer.Api.Configuration;
using TranscriptAnalyzer.Api.Endpoints;
using TranscriptAnalyzer.Api.Middleware;
using TranscriptAnalyzer.Application;
using TranscriptAnalyzer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization to use string enums (PascalCase for enum values)
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Add services to the container
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// OpenAPI/Swagger
builder.Services.AddOpenApi();

// Authentication & Authorization
builder.Services.AddAuthenticationServices(builder.Configuration);

// CORS
builder.Services.AddCorsConfiguration(builder.Configuration, builder.Environment);

// Minimal API endpoint discovery
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCorsConfiguration();

app.UseAuthentication();
app.UseAuthorization();
app.UseTenantContext();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health")
    .AllowAnonymous();

// API version prefix group
var apiV1 = app.MapGroup("/api/v1");

// Register endpoints
apiV1.MapClientsEndpoints();
apiV1.MapDecisionTablesEndpoints();

// Placeholder endpoints (to be implemented in user story tasks)
apiV1.MapGet("/", () => Results.Ok(new { Version = "1.0", Status = "Ready" }))
    .WithName("ApiInfo")
    .WithTags("Info")
    .AllowAnonymous();

// Seed dev data in Development environment
if (app.Environment.IsDevelopment())
{
    await DevDataSeeder.SeedDevDataAsync(app.Services);
}

await app.RunAsync();
