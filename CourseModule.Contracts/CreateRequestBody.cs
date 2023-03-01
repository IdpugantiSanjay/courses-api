namespace CourseModule.Contracts;

public abstract record CreateRequestBody
{
    public required string Name { get; init; } = string.Empty;
    public required TimeSpan Duration { get; init; } = TimeSpan.Zero;

    private interface IEntryProps
    {
        public string Name { get; init; }
        public TimeSpan Duration { get; init; }
    }

    public record Default : CreateRequestBody
    {
        public string[]? Categories { get; init; }
        public bool IsHighDefinition { get; init; }
        public required Entry[] Entries { get; init; } = Array.Empty<Entry>();

        public record Entry : IEntryProps
        {
            public required int SequenceNumber { get; init; }
            public string? Section { get; init; }
            public required string Name { get; init; }
            public required TimeSpan Duration { get; init; } = TimeSpan.Zero;
        }
    }

    public record Playlist : CreateRequestBody
    {
        public required string PlaylistId { get; init; }

        public string[]? Categories { get; init; } = Array.Empty<string>();

        public bool IsHighDefinition { get; init; }

        public required Entry[] Entries { get; init; } = Array.Empty<Entry>();

        public record Entry : IEntryProps
        {
            public required int SequenceNumber { get; init; }
            public required string VideoId { get; init; }
            public required string Name { get; init; }
            public required TimeSpan Duration { get; init; }
        }
    }
}