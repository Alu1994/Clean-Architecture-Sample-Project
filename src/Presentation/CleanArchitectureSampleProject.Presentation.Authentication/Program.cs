using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication;
using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;
using CleanArchitectureSampleProject.Presentation.Authentication.Setups;

var builder = WebApplication.CreateBuilder(args);

builder.BuildAuthRepository();

builder.Services.AddOpenApi(OpenApiSetup.SetupOpenApiOptions);

builder.Services.AddAuthRepositoryLayer();

builder.Services.AddValidation();

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(OpenApiSetup.SetupSwaggerOptions);

app.UseHttpsRedirection();

app.MapEndpoints();

app.UseStaticFiles();

app.Run();
