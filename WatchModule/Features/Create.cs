using Contracts;
using WatchModule.Contracts;
using WatchModule.Entities;

namespace WatchModule.Features;

internal record CreateWatchRequest(int ParentId, CreateWatchRequestBody Body) : ICreateRequest<int, CreateWatchRequestBody>;

// [RegisterScoped<ICreate<int, CreateWatchRequestBody, int>>]
public partial class WatchService : ICreate<int, CreateWatchRequestBody, int>
{
    public async Task<int> Create(ICreateRequest<int, CreateWatchRequestBody> request, CancellationToken cancellationToken)
    {
        var entity = new Watch { CourseId = request.ParentId, EntryId = request.Body.EntryId, CreatedAt = DateTimeOffset.UtcNow };
        _context.WatchHistory.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}