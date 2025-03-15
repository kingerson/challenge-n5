namespace MsClean.Application;
using MediatR;

public record ModifyPermissionCommand(int Id,string EmployeeName, string EmployeeLastName, int PermissionTypeId) : IRequest<bool>;
