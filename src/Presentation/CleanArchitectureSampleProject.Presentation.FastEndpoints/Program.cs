using CleanArchitectureSampleProject.Presentation.FastEndpoints.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.BuildPresentation();

builder.Services.AddPresentation(builder);

var app = builder.Build();

app.UsePresentation();

app.Run();