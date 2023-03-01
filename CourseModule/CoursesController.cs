using System.Text.Json;
using System.Text.Json.Nodes;
using Contracts;
using CourseModule.Contracts;
using CourseModule.Entities;
using CourseModule.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneOf.Types;

namespace CourseModule;

[Route("api/v1/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ILogger<CoursesController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromServices] ICreate<int, CreateRequestBody, int> service, [FromBody] JsonObject json,
        CancellationToken ctx)
    {
        CreateRequestBody? body;
        try
        {
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (json.ContainsKey(nameof(CreateRequestBody.Playlist.PlaylistId)))
            {
                body = json.Deserialize<CreateRequestBody.Playlist>(jsonSerializerOptions);
                var validator = new PlaylistCreateRequestBodyValidator();
                var validationResult = validator.Validate((body as CreateRequestBody.Playlist)!);
                if (!validationResult.IsValid) return BadRequest(validationResult);
            }
            else
            {
                body = json.Deserialize<CreateRequestBody.Default>(jsonSerializerOptions);
                var validator = new DefaultCreateRequestBodyValidator();
                var validationResult = validator.Validate((body as CreateRequestBody.Default)!);
                if (!validationResult.IsValid) return BadRequest(validationResult);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in de-serializing POST request body");
            return BadRequest();
        }

        var request = new CreateCourseRequest { Body = body!, ParentId = default };
        var createdId = await service.Create(request, ctx);
        return Created($"/api/v1/courses/{createdId}", null);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? view, [FromServices] IList<int, CourseView, CourseResponse> service, CancellationToken ctx)
    {
        if (!Enum.TryParse(view, out CourseView courseView)) courseView = CourseView.Default;

        var response = await service.List(new ListCoursesRequest { ParentId = default, View = courseView, PageSize = 10, PageToken = string.Empty },
            ctx);

        return response.Match<IActionResult>(
            Ok,
            InternalServerError
        );
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, [FromQuery] string? view, [FromServices] IGet<int, int, CourseView, CourseResponse> service,
        CancellationToken ctx)
    {
        if (!Enum.TryParse(view, out CourseView courseView)) courseView = CourseView.Default;

        var response = await service.Get(new GetCourseRequest(default, id, courseView), ctx);
        return response.Match<IActionResult>(
            Ok,
            NotFound,
            InternalServerError
        );
    }

    [HttpDelete("{id:int}")]
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