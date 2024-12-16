using CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;
using FluentValidation;

namespace CleanArchitectureSampleProject.Presentation.Authentication.Messages;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class CreateResourceRequestValidator : AbstractValidator<CreateResourceRequest>
{
    public CreateResourceRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public sealed class CreateUserResourceRequestValidator : AbstractValidator<CreateUserResourceRequest>
{
    public CreateUserResourceRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ResourceId).NotEmpty();
        RuleFor(x => x.CanRead).NotNull();
        RuleFor(x => x.CanWrite).NotNull();
        RuleFor(x => x.CanDelete).NotNull();
    }
}