namespace MsClean.Presentation;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected readonly IMediator _mediator;
    public BaseApiController(IMediator mediator) => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
}
