namespace MsClean.Application;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MsClean.Domain;

public class GetAllPermissionQueryHandler : IRequestHandler<GetAllPermissionQuery, IEnumerable<PermissionViewModel>>
{
    private readonly IPermissionRepository _permissionQueryRepository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public GetAllPermissionQueryHandler(
        IPermissionRepository permissionQueryRepository,
        IMediator mediator,
        IMapper mapper
        )
    {
        _permissionQueryRepository = permissionQueryRepository ?? throw new ArgumentNullException(nameof(permissionQueryRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<IEnumerable<PermissionViewModel>> Handle(GetAllPermissionQuery request, CancellationToken cancellationToken)
    {
        var result = await _permissionQueryRepository.GetAll(cancellationToken);
        var permissions = _mapper.Map<IEnumerable<Permission>>(result);

        await _mediator.Publish(new GetAllPermissionNotification(permissions), cancellationToken);

        return result;
    }

}

