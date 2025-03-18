namespace MsClean.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MsClean.Domain;

public class GetPermissionQueryHandler : IRequestHandler<GetPermissionQuery,PermissionViewModel>
{
    private readonly IPermissionRepository _permissionQueryRepository;
    private readonly IMediator _mediator;
    public GetPermissionQueryHandler(
            IPermissionRepository permissionQueryRepository,
            IMediator mediator
        )
    {
        _permissionQueryRepository = permissionQueryRepository ?? throw new ArgumentNullException(nameof(permissionQueryRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    public async Task<PermissionViewModel> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
    {
        var result = await _permissionQueryRepository.GetById(request.id, cancellationToken) ?? throw new MsCleanException($"Permission with id : {request.id} not found");

        var permission = new Permission
        {
            Id = result.Id,
            UserRegister = result.UserRegister,
            DateTimeRegister = result.DateTimeRegister
        };

        permission.Register(result.EmployeeForename, result.EmployeeLastName, result.PermissionTypeId, result.PermissionDate);

        await _mediator.Publish(new GetPermissionNotification(permission), cancellationToken);

        return result;
    }
}
