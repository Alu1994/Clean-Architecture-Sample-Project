using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class ResultsExtensions
{
    public static TSuccessResult ToSuccess<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : IError
    {
        return validationResult.Success;
    }

    public static Results<IError> ToErrorResult<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : IError
    {
        return new Results<IError>(validationResult.State, validationResult.Error);
    }

    public static IError ToError<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : IError
    {
        return validationResult.Error;
    }

    public static BaseError ToBaseError<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : BaseError
    {
        return validationResult.Error;
    }

    public static ErrorList ToErrorList<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : ErrorList
    {
        return validationResult.Error;
    }

    public static Task<A> AsTask<A>(this A self)
    {
        return Task.FromResult(self);
    }

    #region [IResult Extensions]
    public static IResult ToOkOrErrorResult<TSuccess, TError, LoggingClass>(this Results<TSuccess, TError> result, ILogger<LoggingClass> logger, string errorTitle) where TError : BaseError
    {
        return result.ToIResultBase(logger, errorTitle, (success) => Results.Ok(success));
    }

    public static IResult ToCreatedOrErrorResult<TSuccess, TError, LoggingClass>(this Results<TSuccess, TError> result, ILogger<LoggingClass> logger, string errorTitle) where TError : BaseError
    {
        return result.ToIResultBase(logger, errorTitle, (success) => Results.Created("", success));
    }

    public static IResult ToOkOrErrorsResult<TSuccess, TError, LoggingClass>(this Results<TSuccess, TError> result, ILogger<LoggingClass> logger, string errorTitle) where TError : ErrorList
    {
        return result.ToIResultList(logger, errorTitle, (success) => Results.Ok(success));
    }

    public static IResult ToCreatedOrErrorsResult<TSuccess, TError, LoggingClass>(this Results<TSuccess, TError> result, ILogger<LoggingClass> logger, string errorTitle) where TError : ErrorList
    {
        return result.ToIResultList(logger, errorTitle, (success) => Results.Created("", success));
    }

    private static IResult ToIResultBase<TSuccess, TError, LoggingClass>(this Results<TSuccess, TError> result, ILogger<LoggingClass> logger, string errorTitle, Func<TSuccess, IResult> successResult) where TError : BaseError
    {
        return result.Match(success => successResult(success),
            error =>
            {
                var errorMessage = logger.LogBaseError(error);
                return GetBadRequestResult(errorTitle, errorMessage);
            }
        );
    }

    private static IResult ToIResultList<TSuccess, TError, LoggingClass>(this Results<TSuccess, TError> result, ILogger<LoggingClass> logger, string errorTitle, Func<TSuccess, IResult> successResult) where TError : ErrorList
    {
        return result.Match<IResult>(success => successResult(success),
            error =>
            {
                var errorList = logger.LogErrorList(error);
                return GetBadRequestResult(errorTitle, errorList);
            }
        );
    }

    private static IResult GetBadRequestResult(string errorTitle, string errorMessage)
    {
        return Results.Problem(
            type: HttpStatusCode.BadRequest.ToString(),
            title: errorTitle,
            detail: errorMessage,
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    private static IResult GetBadRequestResult<TError>(string errorTitle, TError error) where TError : ErrorList
    {
        return Results.BadRequest(new
        {
            @Type = HttpStatusCode.BadRequest.ToString(),
            Title = errorTitle,
            Detail = errorTitle,
            Errors = error.Errors.Select(x => x.Message),
            StatusCode = StatusCodes.Status400BadRequest
        });
    }
    #endregion [IResult Extensions]
}

public static class ResultStatesExtensions
{
    public static bool IsSuccessStatus(this ResultStates resultStates)
    {
        return resultStates switch
        {
            ResultStates.NotFound or ResultStates.Error => false,
            _ => true
        };
    }

    public static bool IsFailStatus(this ResultStates resultStates)
    {
        return IsSuccessStatus(resultStates) is false;
    }
}