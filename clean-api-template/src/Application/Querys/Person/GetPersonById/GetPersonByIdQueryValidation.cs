namespace MsClean.Application;
using FluentValidation;

public class GetPersonByIdQueryValidation : AbstractValidator<GetPersonByIdQuery>
{
    public GetPersonByIdQueryValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(BusinessExceptionMessages.IdCannotBeNullOrEmpty);
    }
}
