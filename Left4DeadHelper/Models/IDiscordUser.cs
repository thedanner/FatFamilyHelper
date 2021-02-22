namespace Left4DeadHelper.Models
{
    public interface IDiscordUser : IGenericUser
    {
        ulong DiscordId { get; set; }
    }
}
