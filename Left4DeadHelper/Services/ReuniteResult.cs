namespace Left4DeadHelper.Services;

public class ReuniteResult
{
    public ReuniteFailureReason FailureReason { get; set; }
    public int MoveCount { get; set; }

    public enum ReuniteFailureReason
    {
        None,
        TooManyPopulatedVoiceChannels,
    }
}
