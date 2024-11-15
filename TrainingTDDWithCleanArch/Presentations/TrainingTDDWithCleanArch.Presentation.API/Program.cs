using TrainingTDDWithCleanArch.Application;
using TrainingTDDWithCleanArch.Domain;
using TrainingTDDWithCleanArch.Repository;
using TrainingTDDWithCleanArch.Presentation.API;

var builder = WebApplication.CreateBuilder(args);

builder.BuildPresentation();

// =========== Add service defaults & Aspire client integrations. ===========
builder.AddServiceDefaults();
// =========== Add service defaults & Aspire client integrations. ===========

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========== Add Layer Dependency Injection ===========
builder.Services.AddPresentation();
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

app.UsePresentation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
