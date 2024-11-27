using CleanArchitectureSampleProject.Presentation.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(x => 
{
    x.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

builder.Services.AddSingleton<TokenGenerator>();

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Test");
    options.InjectStylesheet("/css/swagger-dark-theme.css");
});

app.UseHttpsRedirection();

app.MapPost("/login", (LoginRequest request, TokenGenerator tokenGenerator) =>
{
    return new
    {
        access_token = tokenGenerator.GenerateToken(request.Email)
    };
});

app.UseStaticFiles();

app.Run();
