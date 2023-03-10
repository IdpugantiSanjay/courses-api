using Contracts;
using Mediator;
using OneOf;
using OneOf.Types;

namespace CourseModule.Contracts;

public record GetCourseRequest(int ParentId, int Id, CourseView View) : IGetRequest<int, int, CourseView>,
    IRequest<OneOf<CourseResponse, NotFound, Error<Exception>>>;