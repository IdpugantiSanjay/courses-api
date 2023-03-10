using Contracts;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using WatchModule.Entities;

namespace WatchModule.Features;

public record DeleteWatch(int ParentId, int Id) : IDeleteRequest<int, int>;

// [RegisterScoped<IDelete<int, int, Watch>>]
public partial class WatchService : IDelete<int, int, Watch>
{
    public async Task<OneOf<Success, NotFound>> Delete(IDeleteRequest<int, int> request, CancellationToken cancellationToken)
    {
        var deleted = await _context.WatchHistory.Where(c => c.Id == request.Id).ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);
        if (deleted == 0) return new NotFound();
        return new Success();
    }
}