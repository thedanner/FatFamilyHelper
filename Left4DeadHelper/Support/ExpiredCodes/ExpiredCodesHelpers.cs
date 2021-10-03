using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Left4DeadHelper.Support.ExpiredCodes
{
    public static class ExpiredCodesHelpers
    {
        private static readonly Regex ExpiresRegex = new Regex(
            @"Expires: (?<expires>(?<date>(?<dayOfMonth>\d{1,2}) (?<month>[a-zA-Z]{3}) (?<year>\d{4})) (?<time>(?<hours>\d{1,2}):(?<minutes>\d{2}))) (?<timezone>[a-zA-Z]{3})",
            RegexOptions.Multiline | RegexOptions.Compiled);

        public static List<IMessage> GetMessagesWithExpiredCodes(List<IMessage> messages, ILogger logger)
        {
            var messagesToDelete = new List<IMessage>();
            var messageIdsToDelete = new List<ulong>();

            foreach (var message in messages)
            {
                if (TryGetExpirationDateFromMessage(message, logger, out var givenExpiry)
                    && givenExpiry <= DateTimeOffset.Now)
                {
                    messagesToDelete.Add(message);
                    messageIdsToDelete.Add(message.Id);
                }
                else if (message.Reference != null
                    && message.Reference.MessageId.IsSpecified
                    && messageIdsToDelete.Contains(message.Reference.MessageId.Value))
                {
                    messagesToDelete.Add(message);
                    messageIdsToDelete.Add(message.Id);
                }
            }

            return messagesToDelete;
        }

        public static bool TryGetExpirationDateFromMessage(IMessage message, ILogger logger, out DateTimeOffset expiration)
        {
            var embed = message.Embeds.FirstOrDefault();

            if (message.Author.IsBot
                && embed != null
                && !string.IsNullOrEmpty(embed.Description))
            {
                // Look for:
                // Expires: 24 JUN 2021 15:00 UTC

                var match = ExpiresRegex.Match(embed.Description);
                if (match.Success
                    && DateTimeOffset.TryParseExact(
                        match.Groups["expires"].Value,
                        "d MMM yyyy H:mm",
                        CultureInfo.CurrentCulture,
                        DateTimeStyles.AssumeUniversal,
                        out var givenExpiry))
                {
                    if (!"UTC".Equals(match.Groups["timezone"].Value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        logger.LogWarning("Non-UTC expiration found for message with ID {messageId}.", message.Id);
                        expiration = default;
                        return false;
                    }

                    expiration = givenExpiry;
                    return true;
                }
            }

            expiration = default;
            return false;
        }
    }
}