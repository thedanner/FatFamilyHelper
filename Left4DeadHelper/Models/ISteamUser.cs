namespace Left4DeadHelper.Models
{
    public interface ISteamUser : IGenericUser
    {
        string SteamId { get; set; }
    }
}
