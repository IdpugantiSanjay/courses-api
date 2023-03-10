// using Contracts;
// using Injectio.Attributes;
// using Microsoft.EntityFrameworkCore;
// using WatchModule.Contracts;
//
// namespace WatchModule.Features;
//
// file record BatchGetWatchResponse : IBatchGetResponse<WatchResponseBody>
// {
//     public required WatchResponseBody?[] Responses { get; set; }
// }
//
// internal record BatchGetWatchRequest(int ParentId, int[] Identities, WatchResponseView View) : IBatchGetRequest<int, int, WatchResponseView>;
//
//
// [RegisterScoped<IBatchGet<int, int, WatchResponseView, WatchResponseBody>>]
// public partial class WatchService : IBatchGet<int, int, WatchResponseView, WatchResponseBody>
// {
//     public async Task<IBatchGetResponse<WatchResponseBody>> BatchGet(IBatchGetRequest<int, int, WatchResponseView> request, CancellationToken cancellationToken)
//     {
//         var history = await _context.WatchHistory.Where(wh => wh.CourseId == request.ParentId && request.Identities.Contains(wh.EntryId)).ToArrayAsync(cancellationToken: cancellationToken);
//         var identitiesSet = request.Identities.ToHashSet();
//         var response = new BatchGetWatchResponse
//         {
//             Responses = history.Select(item => identitiesSet.Contains(item.EntryId) ? new WatchResponseBody(item.Id, item.CourseId, item.EntryId) : null).ToArray()
//         };
//         return response;
//     }
// }

