namespace MsClean.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MsClean.Domain;
using MsClean.Infrastructure;

public class GetPermissionQueryHandler : IRequestHandler<GetPermissionQuery,PermissionViewModel>
{
    private readonly IPermissionRepository _permissionQueryRepository;
    private readonly IKakfaService _kakfaService;
    private readonly IElasticSearchService<Permission> _elasticSearchService;
    public GetPermissionQueryHandler(
        IPermissionRepository permissionQueryRepository,
        IKakfaService kakfaService,
        IElasticSearchService<Permission> elasticSearchService
        )
    {
        _permissionQueryRepository = permissionQueryRepository ?? throw new ArgumentNullException(nameof(permissionQueryRepository));
        _kakfaService = kakfaService ?? throw new ArgumentNullException(nameof(kakfaService));
        _elasticSearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(elasticSearchService));
    }
    public async Task<PermissionViewModel> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
    {
        var result = await _permissionQueryRepository.GetById(request.id, cancellationToken) ?? throw new MsCleanException($"Permission with id : {request.id} not found");

        _ = await _kakfaService.ProduceAsync("test-topic", "get-id");

        var permission = new Permission
        {
            Id = result.Id,
            UserRegister = result.UserRegister,
            DateTimeRegister = result.DateTimeRegister
        };

        permission.Register(result.EmployeeForename, result.EmployeeLastName, result.PermissionTypeId, result.PermissionDate);

        _ = await _elasticSearchService.IndexAsync(permission, "permissions-id");

        return result;
    }
}
