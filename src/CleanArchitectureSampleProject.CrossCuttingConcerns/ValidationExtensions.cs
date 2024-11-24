using LanguageExt;
using LanguageExt.Common;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class ValidationExtensions
{
    public static TSuccessResult ToSuccess<TSuccessResult>(this Validation<Error, TSuccessResult> validation)
    {
        return validation.ToSeq().First()!;
    }

    public static Seq<Error> ToError<TSuccessResult>(this Validation<Error, TSuccessResult> validation)
    {
        return (Seq<Error>)validation;
    }

    public static Results<Error> ToResultError<TSuccessResult>(this Validation<Error, TSuccessResult> validation)
    {
        return new Results<Error>(validation.ToError().ToArr().First());
    }
}