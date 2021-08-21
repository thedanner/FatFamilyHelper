namespace Left4DeadHelper.Models.Configuration
{
    public interface ISteamUser : IGenericUser
    {
        string SteamId { get; set; }
    }
}
