namespace WatchModule.Contracts;

public record WatchStatsResponse(string WatchedDuration, string DurationLeft, int[] WatchedIdList);