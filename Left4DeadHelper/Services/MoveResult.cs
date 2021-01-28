using System.Collections.Generic;

namespace Left4DeadHelper.Services
{
    public class MoveResult
    {
        public MoveResult()
        {
            UnmappedSteamUsers = new List<UnmappedSteamUser>();
        }

        public bool Success { get; set; }
        public int MoveCount { get; set; }
        public List<UnmappedSteamUser> UnmappedSteamUsers { get; set; }
    }
}
