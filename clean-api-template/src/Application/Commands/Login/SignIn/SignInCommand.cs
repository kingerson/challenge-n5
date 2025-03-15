namespace MsClean.Application;
using MediatR;

public class SignInCommand : IRequest<string>
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
