using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication;
using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.BuildAuthRepository();

builder.Services.AddOpenApi(x =>
{
    x.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

builder.Services.AddAuthRepositoryLayer();

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Test");
    options.InjectStylesheet("/css/swagger-dark-theme.css");
});

app.UseHttpsRedirection();

app.MapEndpoints();

app.UseStaticFiles();

app.Run();
