using Courses.API.Notes.Commands;
using Courses.API.Notes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> InsertOrUpdate(Upsert.UpsertRequest request)
    {
        var @return = await _mediator.Send(request);
        return Ok(@return);
    }

    [HttpGet("{courseId:int}")]
    public async Task<IActionResult> FetchEntriesWithNotes(int courseId)
    {
        var @return = await _mediator.Send(new FetchEntriesWithNotes.Request(courseId));
        return Ok(@return);
    }

    [HttpGet("{courseId:int}/{entryId:int}")]
    public async Task<IActionResult> FetchNotes(int courseId, int entryId)
    {
        var response = await _mediator.Send(new FetchNotes.Request(courseId, entryId));
        if (string.IsNullOrEmpty(response.Note)) return NotFound();
        return Ok(response);
    }
}