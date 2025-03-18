namespace MsClean.Application;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MsClean.Domain;
using MsClean.Infrastructure;

public class RequestPermissionNotificationHandler : INotificationHandler<RequestPermissionNotification>
{
    private readonly IKakfaService _kakfaService;
    private readonly IElasticSearchService<Permission> _elasticSearchService;
    public RequestPermissionNotificationHandler(IKakfaService kakfaService, IElasticSearchService<Permission> elasticSearchService)
    {
        _kakfaService = kakfaService ?? throw new ArgumentNullException(nameof(kakfaService));
        _elasticSearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(elasticSearchService));
    }
    public async Task Handle(RequestPermissionNotification notification, CancellationToken cancellationToken)
    {
        _ = await _kakfaService.ProduceAsync("test-topic", "request");

        _ = await _elasticSearchService.IndexAsync(notification.permission, "request-permissions");
    }
}
