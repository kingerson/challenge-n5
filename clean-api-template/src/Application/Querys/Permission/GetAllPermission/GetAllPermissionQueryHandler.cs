namespace MsClean.Application;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MsClean.Domain;
using MsClean.Infrastructure;

public class GetAllPermissionQueryHandler : IRequestHandler<GetAllPermissionQuery, IEnumerable<PermissionViewModel>>
{
    private readonly IPermissionRepository _permissionQueryRepository;
    private readonly IKakfaService _kakfaService;
    private readonly IElasticSearchService<Permission> _elasticSearchService;
    private readonly IMapper _mapper;
    public GetAllPermissionQueryHandler(
        IPermissionRepository permissionQueryRepository,
        IKakfaService kakfaService,
        IElasticSearchService<Permission> elasticSearchService,
        IMapper mapper
        )
    {
        _permissionQueryRepository = permissionQueryRepository ?? throw new ArgumentNullException(nameof(permissionQueryRepository));
        _kakfaService = kakfaService ?? throw new ArgumentNullException(nameof(kakfaService));
        _elasticSearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(elasticSearchService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<IEnumerable<PermissionViewModel>> Handle(GetAllPermissionQuery request, CancellationToken cancellationToken)
    {
        var result = await _permissionQueryRepository.GetAll(cancellationToken);
        _ = await _kakfaService.ProduceAsync("test-topic", "get");

        var permissions = _mapper.Map<IEnumerable<Permission>>(result);

        _ = await _elasticSearchService.IndexBulkAsync(permissions);

        return result;
    }

}

