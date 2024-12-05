namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Configuration.Middlewares;

public sealed class JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly ILogger<JwtMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private const string ContentType = "application/json";
    private const string ErrorLog401 = "Error while Authenticating: {response}";
    private const string ErrorLog403 = "Error while Authorizing: {response}";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            throw;
        }
        await WriteForbiddenResponse(context);
    }

    private async Task WriteForbiddenResponse(HttpContext context)
    {
        if (context.Response.HasStarted) return;        
        var response = GetResponseType(context);
        if (response.IsFail) return;

        _logger.LogError(response.Success!.ErrorMessage, response.Success.Response);
        context.Response.ContentType = ContentType;
        await context.Response.WriteAsJsonAsync(response.Success.Response);
    }

    private static Results<AuthResponseType, BaseError> GetResponseType(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? string.Empty;

        return context.Response.StatusCode switch
        {
            StatusCodes.Status401Unauthorized => new AuthResponseType(ErrorLog401, new UnauthorizedResponse(method, path)),
            StatusCodes.Status403Forbidden => new AuthResponseType(ErrorLog403, new ForbiddenResponse(method, path)),
            _ => new BaseError("")
        };
    }
}

public record AuthResponseType(string ErrorMessage, BaseAuthenticationResponse Response);

public abstract record BaseAuthenticationResponse(
    string RequestedHttpMethod,
    string RequestedResourcePath,
    string Details,
    string Message,
    short? StatusCode);

public record UnauthorizedResponse(
    string RequestedHttpMethod = "",
    string RequestedResourcePath = "",
    string Details = "",
    string Message = "You are not authorized to access this app.",
    short? StatusCode = StatusCodes.Status401Unauthorized) : BaseAuthenticationResponse(RequestedHttpMethod, RequestedResourcePath, Details, Message, StatusCode);

public record ForbiddenResponse(
    string RequestedHttpMethod = "",
    string RequestedResourcePath = "",
    string Details = "",
    string Message = "You are not authorized to access this specific resource.",
    short? StatusCode = StatusCodes.Status403Forbidden) : BaseAuthenticationResponse(RequestedHttpMethod, RequestedResourcePath, Details, Message, StatusCode);
