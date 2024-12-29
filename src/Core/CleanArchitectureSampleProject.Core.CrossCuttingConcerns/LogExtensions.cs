using Microsoft.Extensions.Logging;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class LogExtensions
{
    /// <summary>
    ///     Generate Log based on BaseError
    /// </summary>
    /// <typeparam name="TLogger">(TLogger)Class that is being used by ILogger</typeparam>
    /// <param name="logger">The logger that will be logging the BaseError</param>
    /// <param name="error">BaseError that will be used to generate the Error log.</param>
    /// <returns>Error Message</returns>
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

    public static TError LogErrorList<TLogger, TError>(this ILogger<TLogger> logger, TError error) where TError : ErrorList
    {
        foreach (var item in error.Errors)
        {
            var errorMessage = item.Message;
            var exception = item.Exception;

            if (exception is not null)
            {
                logger.LogError(exception, errorMessage);
                continue;
            }
            logger.LogError(errorMessage);
        }

        return error;
    }
}