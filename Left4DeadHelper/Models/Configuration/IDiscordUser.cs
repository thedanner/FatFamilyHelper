namespace Left4DeadHelper.Models.Configuration
{
    public interface IDiscordUser : IGenericUser
    {
        ulong DiscordId { get; set; }
    }
}
