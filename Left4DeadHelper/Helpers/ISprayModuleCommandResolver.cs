using Discord;
using System.Diagnostics.CodeAnalysis;

namespace Left4DeadHelper.Helpers;

public interface ISprayModuleCommandResolver
{
    bool TryResolve(string? arg1, string? arg2, IUserMessage message, [NotNullWhen(true)] out SprayModuleParseResult? result);
    bool TryHandleReferencedMessage(IMessage referencedMessage, string? fileName, [NotNullWhen(true)] out SprayModuleParseResult? result);
}
