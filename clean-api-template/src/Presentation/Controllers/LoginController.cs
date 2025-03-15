namespace MsClean.Presentation.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsClean.Application;
using System.Net;

[AllowAnonymous]
public class LoginController : BaseApiController
{
    public LoginController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignIn(SignInCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
