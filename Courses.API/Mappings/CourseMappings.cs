using Courses.API.Courses;
using Courses.Shared;
using Mapster;

namespace Courses.API.Mappings;

public class CourseMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Course, GetCourseView>()
            .Map(view => view.Author, c => c.Author.Name)
            .Map(view => view.Platform, c => c.Platform.Name)
            ;

        config.NewConfig<Course, GetByIdCourseView>()
            .Map(dest => dest.Author, c => c.Author)
            .Map(dest => dest.Platform, c => c.Platform)
            ;

        config.NewConfig<CourseEntry, GetByIdCourseEntryView>();
        // config.NewConfig<Course, CourseDto>()
        //     .Map(dto => dto.Entries, nameof(Course.Entries))
        //     // .Map(dto => dto.Name, nameof(Course.Name))
        //     // .Map(dto => dto.Name, nameof(Course.Name))
        //     ;
        // config.NewConfig<CourseEntry, CourseEntryDto>();
    }
}