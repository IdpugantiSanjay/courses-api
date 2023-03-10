using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WatchModule.Contracts;
using WatchModule.Features;

namespace WatchModule;

[Route("api/v1/[controller]")]
[ApiController]
public class WatchController : ControllerBase
{
    private readonly ILogger<WatchController> _logger;
    private readonly WatchService _service;

    public WatchController(ILogger<WatchController> logger, WatchService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost("{courseId:int}")]
    public async Task<IActionResult> Create(int courseId, [FromBody] CreateWatchRequestBody requestBody, CancellationToken cancellationToken)
    {
        var createdId = await _service.Create(new CreateWatchRequest(courseId, requestBody), cancellationToken);
        return Created($"/api/v1/watch/${courseId}/{createdId}", null);
    }

    [HttpDelete("{courseId:int}/{entryId:int}")]
    public async Task Delete(int courseId, int entryId, CancellationToken cancellationToken)
    {
        await _service.Delete(new DeleteWatch(courseId, entryId), cancellationToken);
    }

    [HttpGet("{courseId:int}")]
    public async Task<IActionResult> Get(int courseId, CancellationToken cancellationToken)
    {
        var result = await _service.Get(new GetWatchRequest(default, courseId, CourseWatchStatsView.Default), cancellationToken);
        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(),
            error => StatusCode(StatusCodes.Status500InternalServerError, error.Value)
        );
    }
}