namespace MsClean.Application;
using Microsoft.AspNetCore.SignalR;

public class SignalrHub : Hub
{
    public async Task NewMessage(string user, string message)
    {
        await Clients.All.SendAsync("messageReceived", user, message);
    }

    public async Task SendMessageToClient(string connectionId, string user, string message)
    {
        await Clients.Client(connectionId).SendAsync("messageReceived", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
        await base.OnDisconnectedAsync(exception);
    }
}
