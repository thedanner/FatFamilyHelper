using Discord;

namespace Left4DeadHelper.Helpers
{
    public interface ISprayModuleCommandResolver
    {
        SprayModuleParseResult? Resolve(string? arg1, string? arg2, IUserMessage message);
    }
}
