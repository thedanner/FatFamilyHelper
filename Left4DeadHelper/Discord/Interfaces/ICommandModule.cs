using Left4DeadHelper.Models;

namespace Left4DeadHelper.Discord.Interfaces
{
    public interface ICommandModule
    {
        string GetGeneralHelpMessage(HelpContext helpMessageContext);
    }
}
