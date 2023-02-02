using Courses.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WatchedController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("{courseId:int}")]
    [HttpGet]
    public async Task<IActionResult> Get(int courseId)
    {
        var @return = await _mediator.Send(new GetWatchedRequest(courseId));
        return Ok(@return);
    }

    [Route("{courseId:int}/{entryId:int}")]
    [HttpPost]
    public async Task<IActionResult> Insert(int courseId, int entryId)
    {
        var @return = await _mediator.Send(new SetWatchedRequest(courseId, entryId));
        return Ok(@return);
    }

    [Route("{courseId:int}/{entryId:int}")]
    [HttpDelete]
    public async Task<IActionResult> Delete(int courseId, int entryId)
    {
        var @return = await _mediator.Send(new DeletedWatchedRequest(courseId, entryId));
        return Ok(@return);
    }

    // [Route("{courseId:int}")]
    // [HttpGet]
}