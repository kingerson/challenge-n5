namespace MsClean.Application;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
{
    private readonly IHubContext<SignalrHub> _hubContext;
    public SendNotificationCommandHandler(IHubContext<SignalrHub> hubContext) => _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    public async Task<bool> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Client(request.ConnectionId).SendAsync("messageReceived", request.User, request.Message, cancellationToken);
        return true;
    }
}
