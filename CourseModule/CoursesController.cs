using System.Net.Mime;
using Contracts;
using CourseModule.Contracts;
using CourseModule.Entities;
using CourseModule.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneOf.Types;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseModule;

[Route("api/v1/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class CoursesController : ControllerBase
{
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ILogger<CoursesController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Endpoint to create course", Description = """
        Create a course using ```CreateRequestBody.Default``` or ```CreateRequestBody.Playlist``` request types.
        """
    )]
    [Consumes(typeof(CreateRequestBody.Default), MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateRequestBody body, [FromServices] ICreate<int, CreateRequestBody, int> service,
        CancellationToken ctx)
    {
        switch (body)
        {
            case CreateRequestBody.Default @default:
            {
                var validator = new DefaultCreateRequestBodyValidator();
                var validationResult = await validator.ValidateAsync(@default, ctx);
                if (!validationResult.IsValid) return BadRequest(validationResult);
                break;
            }
            case CreateRequestBody.Playlist playlist:
            {
                var validator = new PlaylistCreateRequestBodyValidator();
                var validationResult = await validator.ValidateAsync(playlist, ctx);
                if (!validationResult.IsValid) return BadRequest(validationResult);
                break;
            }
        }

        var request = new CreateCourseRequest { Body = body, ParentId = default };
        var createdId = await service.Create(request, ctx);
        return Created($"/api/v1/courses/{createdId}", createdId);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Endpoint to get a list of courses from database", Description = """
        Get list of courses from the database. If ```CourseView``` is Default ```ListCoursesResponse<CourseResponse.Default>``` response type is returned, If ```CourseView``` is Entries 
        ```ListCoursesResponse<CourseResponse.Entries>``` response type is returned
        """
    )]
    [ProducesResponseType(typeof(ListCoursesResponse<CourseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] CourseView? view, [FromServices] IList<int, CourseView, CourseResponse> service, CancellationToken ctx)
    {
        var response = await service.List(
            new ListCoursesRequest { ParentId = default, View = view ?? CourseView.Default, PageSize = 10, PageToken = string.Empty },
            ctx);

        return response.Match(
            Ok,
            InternalServerError
        );
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] CourseView? view, int id, [FromServices] IGet<int, int, CourseView, CourseResponse> service,
        CancellationToken ctx)
    {
        // if (!Enum.TryParse(view, out CourseView courseView)) courseView = CourseView.Default;

        var response = await service.Get(new GetCourseRequest(default, id, view ?? CourseView.Default), ctx);
        return response.Match(
            Ok,
            NotFound,
            InternalServerError
        );
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, [FromServices] IDelete<int, int, Course> service, CancellationToken ctx)
    {
        var response = await service.Delete(new DeleteCourseRequest(default, id), ctx);
        return response.Match<IActionResult>(
            _ => NoContent(),
            NotFound
        );
    }

    private IActionResult InternalServerError(Error<Exception> error)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, error.Value);
    }

    private IActionResult NotFound(NotFound notFound)
    {
        return NotFound();
    }
}