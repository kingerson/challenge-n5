namespace MsClean.Application;
using System.Collections.Generic;
using MediatR;

public sealed record GetAllPermissionQuery() : IRequest<IEnumerable<PermissionViewModel>>;
