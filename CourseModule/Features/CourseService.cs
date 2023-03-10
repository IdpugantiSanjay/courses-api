using CourseModule.Database;
using Injectio.Attributes;

namespace CourseModule.Features;

[RegisterScoped]
public sealed partial class CourseService
{
    private readonly CourseDbContext _context;

    public CourseService(CourseDbContext context)
    {
        _context = context;
    }
}

// public class Test
// {
//     public Test()
//     {
//         var service = new CourseService(null!);
//
//         var body = new AbstractRequestBody.Default
//             { Entries = Array.Empty<AbstractRequestBody.Default.Entry>(), Name = "Test", Duration = TimeSpan.Zero };
//
//         service.Create(
//             new CreateCourseRequest { Body = body, ParentId = 1 }, CancellationToken.None
//         );
//         service.Delete(new DeleteCourseRequest(1, 1), CancellationToken.None);
//         service.Get(new GetCourseRequest(1, 1, CourseView.Default), CancellationToken.None);
//
//         service.List(new ListCoursesRequest { ParentId = 1 }, CancellationToken.None);
//     }
// }