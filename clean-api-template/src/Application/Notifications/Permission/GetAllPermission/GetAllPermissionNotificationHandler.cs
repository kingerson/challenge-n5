namespace MsClean.Application;
using System;
using System.Threading.Tasks;
using MediatR;
using MsClean.Domain;
using MsClean.Infrastructure;

public class GetAllPermissionNotificationHandler : INotificationHandler<GetAllPermissionNotification>
{
    private readonly IKakfaService _kakfaService;
    private readonly IElasticSearchService<Permission> _elasticSearchService;
    public GetAllPermissionNotificationHandler(IKakfaService kakfaService, IElasticSearchService<Permission> elasticSearchService)
    {
        _kakfaService = kakfaService ?? throw new ArgumentNullException(nameof(kakfaService));
        _elasticSearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(elasticSearchService));
    }
    public async Task Handle(GetAllPermissionNotification notification, CancellationToken cancellationToken)
    {
        _ = await _kakfaService.ProduceAsync("test-topic", "get");
        _ = await _elasticSearchService.IndexBulkAsync(notification.permissions, "permissions");
    }
}
