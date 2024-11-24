using LanguageExt;
using LanguageExt.Common;
using System.Net.Http.Headers;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public enum ResultStates
{
    Success,
    Created,
    Updated,
    Executed,
    NotFound,
    Error
}

public record class DefiniedResults(bool IsSuccess, ResultStates State, string Message, Exception? Exception)
{
    public DefiniedResults(Exception exception) : this(ResultStates.Error, exception) { }

    public DefiniedResults(ResultStates state, Exception exception) : this(ResultStates.Error, exception.Message, exception) { }

    public DefiniedResults(ResultStates state) : this(state, string.Empty, null) { }

    public DefiniedResults(ResultStates state, string message) : this(state is not ResultStates.NotFound or ResultStates.Error, state, message, null) { }

    public DefiniedResults(ResultStates state, string message, Exception? exception) : this(state is not ResultStates.NotFound or ResultStates.Error, state, message, exception) { }

}

public record class Results<TError>(bool IsSuccess, ResultStates State, string Message, TError? Error) where TError : Error
{
    public Results(TError error) : this(ResultStates.Error, error) { }

    public Results(ResultStates state, TError error) : this(ResultStates.Error, error.Message, error) { }

    public Results(ResultStates state) : this(state, string.Empty, null) { }

    public Results(ResultStates state, string message) : this(state, message, null) { }

    public Results(ResultStates state, string message, TError? error) : this(state is not ResultStates.NotFound or ResultStates.Error, state, message, error) { }
}

public record class Results<TResult, TError>(bool IsSuccess, ResultStates State, string Message, TResult? Result, Error? Error) where TError : Error
{
    public Results(TError error) : this(ResultStates.Error, error) { }

    public Results(ResultStates state, TError error) : this(state, error.Message, error) { }

    public Results(ResultStates state) : this(state, string.Empty, null) { }

    public Results(ResultStates state, string message) : this(state, message, null) { }

    public Results(ResultStates state, string message, TError? error) : this(state is not ResultStates.NotFound or ResultStates.Error, state, message, default, error) { }

    public Results(TResult result) : this(ResultStates.Success, result) { }

    public Results(ResultStates state, TResult result) : this(state is not ResultStates.NotFound or ResultStates.Error, state, string.Empty, result, null) { }

    public static implicit operator Results<TResult, TError>(TResult result)
    {
        return new Results<TResult, TError>(result);
    }

    public static implicit operator Results<TResult, TError>(TError error)
    {
        return new Results<TResult, TError>(error);
    }

    public static implicit operator Results<TResult, TError>(Seq<Error> error)
    {
        return new Results<TResult, TError>((TError)error.Head);
    }

    public static implicit operator Results<TResult, TError>(ResultStates state)
    {
        return new Results<TResult, TError>(state);
    }
}
