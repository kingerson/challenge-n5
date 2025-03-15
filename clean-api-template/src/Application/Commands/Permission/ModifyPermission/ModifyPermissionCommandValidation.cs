namespace MsClean.Application;
using FluentValidation;

public class ModifyPermissionCommandValidation : AbstractValidator<ModifyPermissionCommand>
{
    public ModifyPermissionCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(BusinessExceptionMessages.IdCannotBeNullOrEmpty);
        RuleFor(x => x.EmployeeName).NotEmpty().WithMessage(BusinessExceptionMessages.NameCannotBeNullOrEmpty);
        RuleFor(x => x.EmployeeLastName).NotEmpty().WithMessage(BusinessExceptionMessages.LastNameCannotBeNullOrEmpty);
        RuleFor(x => x.PermissionTypeId).NotEmpty().WithMessage(BusinessExceptionMessages.PermissionTypeCannotBeNullOrEmpty);
    }
}
