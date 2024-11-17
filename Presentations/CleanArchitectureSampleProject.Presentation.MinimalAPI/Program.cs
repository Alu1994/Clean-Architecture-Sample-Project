using CleanArchitectureSampleProject.Presentation.MinimalAPI;

var builder = WebApplication.CreateBuilder(args);
builder.BuildPresentation();

builder.Services.AddPresentation(builder);

builder.Build()
    .UsePresentation()
    .Run();