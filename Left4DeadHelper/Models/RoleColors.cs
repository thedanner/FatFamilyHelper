namespace Left4DeadHelper.Models
{
    public class RoleColors
    {
        public RoleColors()
        {
            Top = new DiscordEntity();
            Bottom = new DiscordEntity();
        }

        public DiscordEntity Top { get; set; }
        public DiscordEntity Bottom { get; set; }
    }
}
