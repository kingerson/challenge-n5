namespace MsClean.Application;
using MediatR;

public class SendNotificationCommand : IRequest<bool>
{
    public string ConnectionId { get; set; }
    public string User { get; set; }
    public string Message { get; set; }
    public string Url { get; set; }
}
