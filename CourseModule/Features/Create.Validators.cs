using CourseModule.Contracts;
using FluentValidation;

namespace CourseModule.Features;

public class DefaultCreateRequestBodyValidator : AbstractValidator<CreateRequestBody.Default>
{
    public DefaultCreateRequestBodyValidator()
    {
        RuleFor(x => x.Duration).NotEmpty();
        RuleFor(x => x.Name).MinimumLength(3).NotEmpty();
        RuleFor(x => x.Entries.Length).GreaterThan(0);

        RuleForEach(x => x.Entries).SetValidator(new EntryValidator());
    }

    private class EntryValidator : AbstractValidator<CreateRequestBody.Default.Entry>
    {
        public EntryValidator()
        {
            RuleFor(x => x.Duration).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.SequenceNumber).NotEmpty();
        }
    }
}

public class PlaylistCreateRequestBodyValidator : AbstractValidator<CreateRequestBody.Playlist>
{
    public PlaylistCreateRequestBodyValidator()
    {
        RuleFor(x => x.Duration).NotEmpty();
        RuleFor(x => x.Name).MinimumLength(3).NotEmpty();
        RuleFor(x => x.Entries.Length).GreaterThan(0);
        RuleFor(x => x.PlaylistId).NotEmpty();
        RuleForEach(x => x.Entries).SetValidator(new EntryValidator());
    }

    private class EntryValidator : AbstractValidator<CreateRequestBody.Playlist.Entry>
    {
        public EntryValidator()
        {
            RuleFor(x => x.Duration).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.VideoId).NotEmpty();
            RuleFor(x => x.SequenceNumber).NotEmpty();
        }
    }
}