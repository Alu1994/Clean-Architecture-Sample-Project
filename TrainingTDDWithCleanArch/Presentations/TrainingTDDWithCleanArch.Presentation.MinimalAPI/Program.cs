using NLog.Web;
using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Domain;
using TrainingTDDWithCleanArch.Presentation.MinimalAPI;
using TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints;
using TrainingTDDWithCleanArch.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.BuildPresentation();

builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========== Add NLog ===========
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();
// =========== Add NLog ===========

// =========== Add service defaults & Aspire client integrations. ===========
// (This must be after 'builder.Logging.ClearProviders()' to re-add the LoggingProviders
builder.AddServiceDefaults();
// =========== Add service defaults & Aspire client integrations. ===========

// =========== Add Layer Dependency Injection ===========
builder.Services.AddPresentation();
builder.Services.AddDomainLayer();
builder.Services.AddApplicationLayer();
builder.Services.AddRepositoryLayer();
// =========== Add Layer Dependency Injection ===========

var app = builder.Build();

app.UsePresentation();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// =========== Map Endpoints ===========
app.MapDefaultEndpoints();
app.MapEndpoints();
// =========== Map Endpoints ===========

app.Run();