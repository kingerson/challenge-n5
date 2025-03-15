namespace MsClean.Presentation.Controllers;

using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MsClean.Application;

public class PermissionController : BaseApiController
{
    public PermissionController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Create(RequestPermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetById(int id)
    {
        var request = new GetPermissionQuery(id);
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PermissionViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Get()
    {
        var request = new GetAllPermissionQuery();
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Update(ModifyPermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
