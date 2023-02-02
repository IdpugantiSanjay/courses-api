using Courses.API.Courses;
using Courses.Shared;
using Mapster;

namespace Courses.API.Mappings;

public class CourseMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Course, GetCourseView>()
            // .Map(view => view.Duration, c => c.Duration.TotalHours > 1 ? $"{c.Duration.TotalHours}h" : $"{c.Duration.Minutes}m")
            // .Map(view => view.Duration, c => c.Duration.ToString("h'h 'm'm 's's'"))
            .Map(view => view.Duration, c => c.Duration.TotalHours > 24 ? $"{Math.Round(c.Duration.TotalHours)}h" : c.Duration.ToString("h'h 'm'm'"))
            // .Map(view => view.Progress, c => c.WatchHistory)
            // .Map(view => view.Author, c => c.Author.Name)
            // .Map(view => view.Platform, c => c.Platform.Name)    
            ;

        config.NewConfig<Course, GetByIdCourseView>()
            .Map(dest => dest.Author, c => c.Author)
            .Map(dest => dest.Platform, c => c.Platform)
            // .Map(dest => dest.Duration, c => c.Duration.TotalHours > 1 ? $"{c.Duration.TotalHours}h" : $"{c.Duration.Minutes}m")
            // .Map(view => view.Duration, c => c.Duration.ToString("h'h 'm'm 's's'"))
            .Map(view => view.Duration,
                c => c.Duration.TotalHours > 24 ? $"{Math.Round(c.Duration.TotalHours)}h" :
                    c.Duration.TotalHours < 1 ? c.Duration.ToString("m'm'") : c.Duration.ToString("h'h 'm'm'"))
            ;

        config.NewConfig<CourseEntry, GetByIdCourseEntryView>()
            .Map(dest => dest.Duration, s => $"{Math.Round(s.Duration.TotalMinutes)}m")
            ;

        config.NewConfig<WatchHistory.WatchHistory, GetWatchedResponseEntryView>()
            .Map(dest => dest.Id, s => s.EntryId);
    }
}