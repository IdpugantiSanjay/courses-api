using System.Text.Json.Serialization;
using CourseModule.Contracts.JsonConvertors;

namespace CourseModule.Contracts;

[JsonConverter(typeof(CreateResponseJsonConvertor))]
public abstract record CourseResponse
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Duration { get; init; }

    public abstract string Kind { get; }

    public record Default : CourseResponse
    {
        public bool IsHighDefinition { get; init; }
        public string? PlaylistId { get; init; }
        public override string Kind => nameof(Default);
    }

    public record WithEntries : CourseResponse
    {
        public override string Kind => nameof(WithEntries);

        public required Entry[] Entries { get; init; } = Array.Empty<Entry>();

        public record Entry
        {
            public required int Id { get; init; }

            public required string Name { get; init; }

            public string? VideoId { get; init; }

            public int SequenceNumber { get; init; }

            public TimeSpan Duration { get; init; }
        }
    }
}

// public static class Extensions
// {
//     public static bool IsSuccess(this OneOf<CourseResponse, NotFound, Error<Exception>> @class)
//     {
//         return @class.IsT0;
//     }
// }