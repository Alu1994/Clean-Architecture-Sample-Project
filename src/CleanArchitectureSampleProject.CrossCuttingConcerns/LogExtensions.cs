using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class LogExtensions
{
    /// <summary>
    ///     Generate Log based on LanguageExt.Common.Seq<Error>
    /// </summary>
    /// <typeparam name="TLogger">(TLogger)Class that is being used by ILogger</typeparam>
    /// <param name="logger">The logger that will be logging the Seq<Error></param>
    /// <param name="error">Seq<Error> that will be used to generate the Error log.</param>
    /// <returns>Error Message</returns>
    public static string LogSeqError<TLogger>(this ILogger<TLogger> logger, Seq<Error> error)
    {
        var errorMessage = error.ToSeq().Head.Message;
        var exceptions = error.ToSeq().Head.Exception;
        exceptions.Match(exp =>
            logger.LogError(exp, errorMessage),
            () => logger.LogError(errorMessage));

        return errorMessage;
    }

    public static string LogBaseError<TLogger, TError>(this ILogger<TLogger> logger, TError error) where TError : BaseError
    {
        var errorMessage = error.Message;
        var exception = error.Exception;

        if (exception is not null)
        {
            logger.LogError(exception, errorMessage);
            return errorMessage;
        }
        logger.LogError(errorMessage);
        return errorMessage;
    }
}