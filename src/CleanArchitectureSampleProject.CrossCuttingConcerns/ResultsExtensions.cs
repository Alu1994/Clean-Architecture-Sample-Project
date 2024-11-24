﻿namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class ResultsExtensions
{
    public static TSuccessResult ToSuccess<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : BaseError
    {
        return validationResult.Success;
    }

    public static Results<BaseError> ToErrorResult<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : BaseError
    {
        return new Results<BaseError>(validationResult.State, validationResult.Error);
    }

    public static BaseError ToError<TSuccessResult, TError>(this Results<TSuccessResult, TError> validationResult)
        where TError : BaseError
    {
        return validationResult.Error;
    }

    public static Task<A> AsTask<A>(this A self)
    {
        return Task.FromResult(self);
    }
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