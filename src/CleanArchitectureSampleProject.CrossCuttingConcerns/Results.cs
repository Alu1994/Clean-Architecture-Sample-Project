namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public enum ResultStates : short
{
    Success,
    Created,
    Updated,
    Executed,

    NotFound,
    Error
}

public record class Results<TError>(bool IsSuccess, bool IsFail, ResultStates State, TError? Error) where TError : BaseError
{
    public Results(TError error) : this(ResultStates.Error, error) { }

    public Results(ResultStates state) : this(state, null) { }

    public Results(ResultStates state, TError? error) : this(state.IsSuccessStatus(), state.IsFailStatus(), state, error) { }
}

public record class Results<TSuccessResult, TErrorResult>(bool IsSuccess, bool IsFail, ResultStates State, TSuccessResult? Success, TErrorResult? Error) where TErrorResult : BaseError
{
    public Results(ResultStates state) : this(state.IsSuccessStatus(), state.IsFailStatus(), state, default, null) { }

    public Results(TSuccessResult result) : this(ResultStates.Success, result) { }

    public Results(TErrorResult error) : this(ResultStates.Error, error) { }

    public Results(ResultStates state, TSuccessResult result) : this(true, false, state, result, null) { }

    public Results(ResultStates state, TErrorResult error) : this(false, true, state, default, error) { }



    public async Task<R2> MatchAsync<R2>(Func<TSuccessResult, Task<R2>> SuccAsync, Func<TErrorResult, R2> Fail)
    {
        return await Match(SuccAsync, (TErrorResult f) => Fail(f).AsTask());
    }

    public async Task<R2> MatchAsync<R2>(Func<TSuccessResult, Task<R2>> SuccAsync, Func<TErrorResult, Task<R2>> FailAsync)
    {
        return await Match(SuccAsync, FailAsync);
    }

    public Ret Match<Ret>(Func<TSuccessResult, Ret> Succ, Func<TErrorResult, Ret> Fail)
    {
        if (IsFail is false)
        {
            if (Succ is not null && Success is not null)
            {
                return Succ(Success);
            }
            throw new ArgumentNullException("Succ");
        }
        return Fail(Error!);
    }


    public static implicit operator Results<TSuccessResult, TErrorResult>(TSuccessResult result)
    {
        return new Results<TSuccessResult, TErrorResult>(result);
    }

    public static implicit operator Results<TSuccessResult, TErrorResult>(TErrorResult error)
    {
        return new Results<TSuccessResult, TErrorResult>(error);
    }

    public static implicit operator Results<TSuccessResult, TErrorResult>(ResultStates state)
    {
        return new Results<TSuccessResult, TErrorResult>(state);
    }

    public static implicit operator Results<TSuccessResult, TErrorResult>((ResultStates state, TErrorResult error) result)
    {
        return new Results<TSuccessResult, TErrorResult>(result.state, result.error);
    }

    public static implicit operator Results<TSuccessResult, TErrorResult>((ResultStates state, TSuccessResult success) result)
    {
        return new Results<TSuccessResult, TErrorResult>(result.state, result.success);
    }
}

public record class BaseError(string Message, Exception? Exception)
{
    public BaseError(string message) : this(message, null) { }
}