namespace Left4DeadHelper.Discord.Interfaces
{
    public interface ICommandModule
    {
        string CommandString { get; }

        string GetGeneralHelpMessage();
    }
}
