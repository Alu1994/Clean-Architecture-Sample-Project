namespace CleanArchitectureSampleProject.Presentation.MinimalAPI.Setups;

public sealed class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Permite a requisição continuar para o próximo middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Captura exceções e trata erros de autenticação
            //await HandleUnauthorizedResponse(context, ex);
            throw;
        }

        // Verifica se o status foi configurado como 401
        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            await WriteUnauthorizedResponse(context);
        }
    }

    private async Task HandleUnauthorizedResponse(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/json";

        // Mensagem de erro personalizada
        var response = new UnauthorizedResponse
        (
            (short)context.Response.StatusCode,
            context.Request.Path.Value,
            "Unauthorized. Your token might be missing or invalid.",
            ex.Message
        );

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task WriteUnauthorizedResponse(HttpContext context)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.ContentType = "application/json";

            var response = new UnauthorizedResponse
            (
                (short)context.Response.StatusCode,
                context.Request.Path.Value
            );

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

public record UnauthorizedResponse(short StatusCode, string? RequestedResourcePath, string Message = "You are not authorized to access this resource.", string? Details = null);