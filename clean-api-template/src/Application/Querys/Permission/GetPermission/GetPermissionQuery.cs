namespace MsClean.Application;
using MediatR;

public sealed record GetPermissionQuery(int id) : IRequest<PermissionViewModel>;
