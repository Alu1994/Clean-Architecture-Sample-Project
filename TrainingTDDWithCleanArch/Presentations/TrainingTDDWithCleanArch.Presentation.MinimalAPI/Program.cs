using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Domain;
using TrainingTDDWithCleanArch.Repository;
using TrainingTDDWithCleanArch.Presentation.MinimalAPI.Endpoints.Products;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// =========== Add service defaults & Aspire client integrations. ===========
builder.AddServiceDefaults();
// =========== Add service defaults & Aspire client integrations. ===========

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========== Add NLog ===========
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();
// =========== Add NLog ===========

// =========== Add Layer Dependency Injection ===========
builder.Services.AddDomainLayer();
builder.Services.AddApplicationLayer();
builder.Services.AddRepositoryLayer();
// =========== Add Layer Dependency Injection ===========

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// =========== Add Endpoints ===========
app.MapProducts().MapCategories();
// =========== Add Endpoints ===========

app.Run();