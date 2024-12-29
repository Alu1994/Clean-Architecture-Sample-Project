using CleanArchitectureSampleProject.Infrastructure.Repository.Authentication;
using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;
using CleanArchitectureSampleProject.Presentation.Authentication.Setups;

namespace CleanArchitectureSampleProject.Presentation.Authentication;

public sealed class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.BuildAuthRepository();

        builder.Services.AddOpenApi(OpenApiSetup.SetupOpenApiOptions);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                // Allow requests from your frontend's origin (e.g., localhost:3000)
                policy.WithOrigins("http://127.0.0.1:3000")  // Replace with your frontend URL
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddAuthRepositoryLayer();

        builder.Services.AddValidation();

        var app = builder.Build();

        app.MapOpenApi();

        app.UseCors("AllowFrontend");

        app.UseSwaggerUI(OpenApiSetup.SetupSwaggerOptions);

        app.UseHttpsRedirection();

        app.MapEndpoints();

        app.UseStaticFiles();

        app.Run();
    }
}