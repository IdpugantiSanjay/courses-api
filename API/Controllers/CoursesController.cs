namespace Courses.API.Controllers;
//
// [Route("api/[controller]")]
// [ApiController]
// public class CoursesController : ControllerBase
// {
//     private readonly IMediator _mediator;
//
//     public CoursesController(IMediator mediator)
//     {
//         _mediator = mediator;
//     }
//
//     [HttpPost]
//     public async Task<IActionResult> Create(CreateCourseRequest request)
//     {
//         return Ok(await _mediator.Send(request));
//     }
//
//     [HttpPost(":playlist")]
//     public async Task<IActionResult> CreateFromYoutubePlaylist(CreateCourseFromPlaylistRequest request)
//     {
//         return Ok(await _mediator.Send(request));
//     }
//
//     [HttpGet]
//     public async Task<IActionResult> Get(string? q)
//     {
//         return Ok(await _mediator.Send(new GetCoursesRequest()));
//     }
//
//     [HttpGet("{id:int}")]
//     public async Task<IActionResult> Get(int id)
//     {
//         var @return = await _mediator.Send(new GetCourseByIdRequest(id));
//         return Ok(@return);
//     }
//
//     [HttpDelete("{id:int}")]
//     public async Task<IActionResult> Delete(int id)
//     {
//         return Ok(await _mediator.Send(new DeleteCourseByIdRequest(id)));
//     }
// }