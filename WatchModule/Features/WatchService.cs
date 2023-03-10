using Injectio.Attributes;
using Mediator;
using WatchModule.Database;

namespace WatchModule.Features;

[RegisterScoped<WatchService>]
public sealed partial class WatchService
{
    private readonly WatchDbContext _context;
    private readonly IMediator _mediator;

    public WatchService(WatchDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }
}