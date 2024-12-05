using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class ProblemDetailsExtensions
{
    public static IResult ToProblemDetails(this BaseError error, string title = "Error")
    {
        return Results.Problem(error.ToProblems());
    }

    public static ProblemDetails ToProblems(this BaseError error, string title = "Error")
    {
        return new ProblemDetails
        {
            Detail = error.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = HttpStatusCode.BadRequest.ToString(),
            Title = title
        };
    }

    public static IResult ToProblemDetails(this ErrorList errors, string title = "Error")
    {
        return Results.Problem(errors.ToProblems());
    }

    private const byte SpaceCount = 1;
    public static ProblemDetails ToProblems(this ErrorList errors, string title = "Error")
    {
        // === Using 'Span' returned the same result as using 'string.Join()' ===
        //var errorMessage = string.Create(errors.Errors.Select(x => x.Message.Length + SpaceCount).Sum(), errors.Errors, (span, state) =>
        //{
        //    int position = 0;
        //    Span<char> space = stackalloc char[SpaceCount] { ' ' };
        //    foreach (var str in state)
        //    {
        //        str.Message.AsSpan().CopyTo(span.Slice(position));
        //        position += str.Message.Length;
        //        space.CopyTo(span.Slice(position));
        //        position += 1;
        //    }
        //});

        return new ProblemDetails
        {
            Detail = string.Join(" ", errors.Errors.Select(x => x.Message)),
            Status = StatusCodes.Status400BadRequest,
            Type = HttpStatusCode.BadRequest.ToString(),
            Title = title
        };
    }
}