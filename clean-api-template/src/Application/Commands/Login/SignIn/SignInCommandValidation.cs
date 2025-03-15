namespace MsClean.Application;
using FluentValidation;

public class SignInCommandValidation : AbstractValidator<SignInCommand>
{
    public SignInCommandValidation()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage(BusinessExceptionMessages.PropertyCannotBeNullOrEmpty);
        RuleFor(x => x.Password).NotEmpty().WithMessage(BusinessExceptionMessages.PropertyCannotBeNullOrEmpty);
    }
}

