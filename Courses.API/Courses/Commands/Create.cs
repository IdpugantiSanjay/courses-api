using Courses.API.Database;
using Courses.Shared;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Courses.Commands;

[UsedImplicitly]
public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(c => c.Name).MaximumLength(64);
        RuleFor(c => c.Duration).GreaterThan(TimeSpan.FromMinutes(1)).LessThan(TimeSpan.FromDays(5));
        RuleFor(c => c.Author).MaximumLength(64);
        RuleFor(c => c.Platform).MaximumLength(64);

        RuleFor(c => c.Entries.Length).GreaterThan(0);
        RuleForEach(c => c.Entries).SetValidator(new CreateCourseRequestEntryValidator());
    }
}

[UsedImplicitly]
public class CreateCourseRequestEntryValidator : AbstractValidator<CreateCourseRequestEntry>
{
    public CreateCourseRequestEntryValidator()
    {
        RuleFor(c => c.Name).MaximumLength(128);
        RuleFor(c => c.Duration).GreaterThan(TimeSpan.FromSeconds(1)).LessThan(TimeSpan.FromDays(5));
        RuleFor(c => c.Section).MaximumLength(128);
    }
}

[UsedImplicitly]
public class CreateCourseFromPlaylistRequestValidator : AbstractValidator<CreateCourseFromPlaylistRequest>
{
    public CreateCourseFromPlaylistRequestValidator()
    {
        RuleFor(c => c.Name).MaximumLength(64);
        RuleFor(c => c.Duration).GreaterThan(TimeSpan.FromMinutes(1)).LessThan(TimeSpan.FromDays(5));
        RuleFor(c => c.PlaylistId).NotEmpty();

        RuleFor(c => c.Entries.Length).GreaterThan(0);
        RuleForEach(c => c.Entries).SetValidator(new CreateCourseRequestFromPlaylistEntryValidator());
    }
}

[UsedImplicitly]
public class CreateCourseRequestFromPlaylistEntryValidator : AbstractValidator<CreateCourseFromPlaylistRequestEntry>
{
    public CreateCourseRequestFromPlaylistEntryValidator()
    {
        RuleFor(c => c.Name).MaximumLength(128);
        RuleFor(c => c.Duration).GreaterThan(TimeSpan.FromSeconds(1)).LessThan(TimeSpan.FromDays(5));
        RuleFor(c => c.VideoId).NotEmpty();
    }
}

//
// [UsedImplicitly]
// public class CreateCourseFromPlaylistRequestEntryValidator : AbstractValidator<CreateCourseFromPlaylistRequest>
// {
//     public CreateCourseFromPlaylistRequestEntryValidator()
//     {
//         RuleFor(c => c.Name).MaximumLength(128);
//         RuleFor(c => c.Duration).GreaterThan(TimeSpan.FromSeconds(1)).LessThan(TimeSpan.FromDays(5));
//         RuleFor(c => c.Entries).NotNull().NotEmpty();
//         RuleFor(c => c.Entries.Length).GreaterThan(0);
//         
//         
//     }
// }

[UsedImplicitly]
public class CreateCourseHandler : IRequestHandler<CreateCourseRequest>, IRequestHandler<CreateCourseFromPlaylistRequest>
{
    private readonly AppDbContext _context;
    private readonly ILogger<IRequestHandler<CreateCourseRequest>> _logger;

    public CreateCourseHandler(AppDbContext context, ILogger<CreateCourseHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateCourseFromPlaylistRequest request, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Duration = request.Duration,
            Categories = request.Categories,
            Name = request.Name,
            IsHighDefinition = request.IsHighDefinition,
            PlaylistId = request.PlaylistId,
            Entries = request.Entries.Select(e => new CourseEntry
            {
                Duration = e.Duration,
                Name = e.Name,
                SequenceNumber = e.SequenceNumber,
                VideoId = e.VideoId
            }).ToList()
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Added Course: {Name} to Database", course.Name);
        return await Unit.Task;
    }

    public async Task<Unit> Handle(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        Author? author = null;
        var authorName = request.Author;
        if (!string.IsNullOrWhiteSpace(authorName))
        {
            author = _context.Authors.FirstOrDefault(a => EF.Functions.ILike(a.Name, $"%{authorName}%"));
            if (author is null)
            {
                author = new Author { Name = authorName };
                _context.Authors.Add(author);
            }
        }

        Platform? platform = null;
        var platformName = request.Platform;
        if (!string.IsNullOrWhiteSpace(platformName))
        {
            platform = _context.Platforms.FirstOrDefault(a => EF.Functions.ILike(a.Name, $"%{request.Platform}%"));
            if (platform is null)
            {
                platform = new Platform { Name = platformName };
                _context.Platforms.Add(platform);
            }
        }

        var course = new Course
        {
            Author = author,
            Platform = platform,
            Duration = request.Duration,
            Categories = request.Categories,
            Name = request.Name,
            IsHighDefinition = request.IsHighDefinition,
            Path = request.Path,
            Host = request.Host,
            Entries = request.Entries.Select(e => new CourseEntry
            {
                Duration = e.Duration,
                Name = e.Name,
                Section = e.Section,
                SequenceNumber = e.SequenceNumber
            }).ToList()
        };

        _context.Courses.Add(course);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Added Course: {Name} to Database", course.Name);

        return await Unit.Task;
    }
}