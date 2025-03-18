namespace MsClean.Application;
using System;
using System.Threading.Tasks;
using MediatR;
using MsClean.Domain;
using MsClean.Infrastructure;

public class GetPermissionNotificationHandler : INotificationHandler<GetPermissionNotification>
{
    private readonly IKakfaService _kakfaService;
    private readonly IElasticSearchService<Permission> _elasticSearchService;
    public GetPermissionNotificationHandler(IKakfaService kakfaService, IElasticSearchService<Permission> elasticSearchService)
    {
        _kakfaService = kakfaService ?? throw new ArgumentNullException(nameof(kakfaService));
        _elasticSearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(elasticSearchService));
    }
    public async Task Handle(GetPermissionNotification notification, CancellationToken cancellationToken)
    {
        _ = await _kakfaService.ProduceAsync("test-topic", "get-id");
        _ = await _elasticSearchService.IndexAsync(notification.permission, "permissions-id");
    }
}

