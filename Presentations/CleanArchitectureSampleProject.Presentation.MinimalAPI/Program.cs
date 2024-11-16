using CleanArchitectureSampleProject.Presentation.MinimalAPI;

var builder = WebApplication.CreateBuilder(args);
builder.BuildPresentation();

builder.Services.AddPresentation();

var app = builder.Build();

app.UsePresentation();

app.Run();