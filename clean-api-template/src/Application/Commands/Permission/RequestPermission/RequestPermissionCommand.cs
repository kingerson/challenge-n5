namespace MsClean.Application;
using MediatR;

public record RequestPermissionCommand(string EmployeeName,string EmployeeLastName,int PermissionTypeId) : IRequest<int>;
