namespace MsClean.Application;
using FluentValidation;

public class RequestPermissionCommandValidation : AbstractValidator<RequestPermissionCommand>
{
    public RequestPermissionCommandValidation()
    {
        RuleFor(x => x.EmployeeName).NotEmpty().WithMessage(BusinessExceptionMessages.NameCannotBeNullOrEmpty);
        RuleFor(x => x.EmployeeLastName).NotEmpty().WithMessage(BusinessExceptionMessages.LastNameCannotBeNullOrEmpty);
        RuleFor(x => x.PermissionTypeId).NotEmpty().WithMessage(BusinessExceptionMessages.PermissionTypeCannotBeNullOrEmpty);
    }
}
