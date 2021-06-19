using Discord;
using Left4DeadHelper.Helpers.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Left4DeadHelper.Helpers
{
    public class SprayModuleCommandResolver : ISprayModuleCommandResolver
    {
        private readonly ILogger<SprayModuleCommandResolver> _logger;

        public SprayModuleCommandResolver(ILogger<SprayModuleCommandResolver> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public SprayModuleParseResult? Resolve(string? arg1, string? arg2, IUserMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var referencedMessage = message.ReferencedMessage;

            // Supported scenarios:
            // 1. Message content starts with a URL.
            // 2. Message content starts with a filename and URL.
            // 3. Message content starts with a filename and has an attachment.
            // 4. Message content starts with a filename and is a reply to a message with only a URL as its content.
            // 5. Message content starts with a filename and is a reply to a message with an attachement.
            // 6. Message content is empty and has an attachment.
            // 7. Message content is empty and is a reply to a message with only a URL as its content.
            // 8. Message content is empty and is a reply to a message with an attachment.

            if (!string.IsNullOrEmpty(arg1))
            {
                // Case 1. Message content starts with a URL.
                if (Uri.TryCreate(arg1, UriKind.Absolute, out var uri)
                    && uri != null)
                {
                    return arg1.StartsWithHttpProtocol()
                        ? new SprayModuleParseResult(uri)
                        : null;
                }
                else
                {
                    var fileName = arg1;

                    // Case 2. Message content starts with a filename and URL.
                    if (!string.IsNullOrEmpty(arg2)
                        && Uri.TryCreate(arg2, UriKind.Absolute, out uri)
                        && uri != null)
                    {
                        return arg2.StartsWithHttpProtocol()
                            ? new SprayModuleParseResult(uri, fileName)
                            : null;
                    }

                    // Case 3. Message content starts with a filename and attachment.
                    if (message.Attachments.Any()
                        && Uri.TryCreate(message.Attachments.First().ProxyUrl, UriKind.Absolute, out uri)
                        && uri != null)
                    {
                        return new SprayModuleParseResult(uri, fileName);
                    }

                    // 4. Message content starts with a filename and is a reply to a message with only a URL as its content.
                    // 5. Message content starts with a filename and is a reply to a message with an attachement.
                    if (referencedMessage != null)
                    {
                        var referencedMessageResult = HandleReferencedMessage(referencedMessage, fileName);
                        if (referencedMessageResult != null) return referencedMessageResult;
                    }
                }
            }
            else 
            {
                // 6. Message content is empty and has an attachment.
                if (message.Attachments.Any()
                    && Uri.TryCreate(message.Attachments.First().ProxyUrl, UriKind.Absolute, out var uri)
                    && uri != null)
                {
                    return new SprayModuleParseResult(uri, null);
                }

                // 7. Message content is empty and is a reply to a message with only a URL as its content.
                // 8. Message content is empty and is a reply to a message with an attachment.
                if (referencedMessage != null)
                {
                    var referencedMessageResult = HandleReferencedMessage(referencedMessage, null);
                    if (referencedMessageResult != null) return referencedMessageResult;
                }
            }

            return null;
        }

        private SprayModuleParseResult? HandleReferencedMessage(IUserMessage referencedMessage, string? fileName)
        {
            if (referencedMessage is null) throw new ArgumentNullException(nameof(referencedMessage));
            
            if (!string.IsNullOrEmpty(referencedMessage.Content)
                && Uri.TryCreate(referencedMessage.Content, UriKind.Absolute, out var uri)
                && uri != null)
            {
                return referencedMessage.Content.StartsWithHttpProtocol()
                    ? new SprayModuleParseResult(uri, fileName)
                    : null;
            }

            if (referencedMessage.Attachments.Any()
                && Uri.TryCreate(referencedMessage.Attachments.First().ProxyUrl, UriKind.Absolute, out uri)
                && uri != null)
            {
                return new SprayModuleParseResult(uri, fileName);
            }

            return null;
        }
    }
}
