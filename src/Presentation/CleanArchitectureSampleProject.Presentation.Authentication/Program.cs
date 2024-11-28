using CleanArchitectureSampleProject.Infrastructure.Repository;
using CleanArchitectureSampleProject.Presentation.Authentication.Endpoints;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.BuildAuthRepository();

builder.Services.AddOpenApi(x => 
{
    x.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddAuthRepositoryLayer();

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Test");
    options.InjectStylesheet("/css/swagger-dark-theme.css");
});

app.UseHttpsRedirection();

app.MapPost("/login", async (LoginRequest request, TokenGenerator tokenGenerator, CancellationToken cancellation) =>
{
    var result = await tokenGenerator.GenerateToken(request, cancellation);
    if (result.IsSuccess)
    {
        return Results.Ok(new { access_token = result.Success });
    }
    return Results.Problem(
        detail: $"User '{request.Email}' not found.",
        statusCode: StatusCodes.Status400BadRequest,
        title: "Error while generating token.",
        type: HttpStatusCode.BadRequest.ToString());
});

app.MapPost("/create-user", async (CreateUserRequest request, TokenGenerator tokenGenerator, CancellationToken cancellation) =>
{
    var user = await tokenGenerator.CreateUser(request, cancellation);

    return user;
});

app.MapPost("/create-resource", async (CreateResourceRequest request, TokenGenerator tokenGenerator, CancellationToken cancellation) =>
{
    var resource = await tokenGenerator.CreateResource(request, cancellation);

    return resource;
});

app.MapPost("/create-user-resource", async (CreateUserResourceRequest request, TokenGenerator tokenGenerator, CancellationToken cancellation) =>
{
    var userResource = await tokenGenerator.CreateUserResource(request, cancellation);

    return userResource;
});

app.UseStaticFiles();

app.Run();
