using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Left4DeadHelper.Helpers;
using Left4DeadHelper.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Left4DeadHelper.Discord.Modules
{
    [Group(Constants.GroupL4d)]
    [Alias(Constants.GroupL4d2)]
    public class RoleGradientModule : ModuleBase<SocketCommandContext>
    {
        private static readonly Regex HexColorPattern = new Regex(@"#(?:[09a-fA-F]){6}", RegexOptions.Compiled);

        private const string Command = "roles";

        private readonly ILogger<PickMapModule> _logger;
        private readonly Settings _settings;

        public RoleGradientModule(ILogger<PickMapModule> logger, Settings settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [Command(Command)]
        [Summary("Sets role colors!")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task HandleCommandAsync(params string[] colors)
        {
            // TODO run gradient calculation.

            var guild = Context.Guild;
            if (guild == null)
            {
                throw new Exception("Could not get guild.");
            }

            var guildSettings = _settings.DiscordSettings.GuildSettings.FirstOrDefault(g => g.Id == guild.Id);
            if (guildSettings == null)
            {
                throw new Exception($"Unable to find guild setting with ID {guild.Id} in the configuration.");
            }

            var guildRoleColorSettings = guildSettings.RoleColors;
            if (guildRoleColorSettings == null) return; // Disabled for guild.

            var gradientRoles = GetSortedRolesForColors(guild, guildRoleColorSettings);

            if (colors.Length == 0 || (colors.Length == 1 && "dump".Equals(colors[0], StringComparison.CurrentCultureIgnoreCase)))
            {
                await ReplyAsync(string.Join(" ", gradientRoles.Select(r => r.Color)));
                return;
            }
            else
            {
                var invalidColors = colors.Where(c => !HexColorPattern.IsMatch(c)).ToList();

                if (invalidColors.Any())
                {
                    await ReplyAsync(
                        $"Color{(invalidColors.Count != 1 ? "s " : "")} " +
                        $"{string.Join(", ", invalidColors.Select(c => "\"" + c + "\""))} " +
                        $"{(invalidColors.Count != 1 ? "are" : "is")} are not in the correct format (#rrggbb).");
                    return;
                }

                if (colors.Length != gradientRoles.Count)
                {
                    await ReplyAsync(
                        "The number of colors doesn't match the number of roles to update " +
                        $"(got {colors.Length} colors, but there are {gradientRoles.Count} roles to change).");
                }

                for (var i = 0; i < gradientRoles.Count; i++)
                {
                    // TODO actually set role colors!
                    var role = gradientRoles[i];
                    var color = colors[i];

                    if (color.StartsWith("#"))
                    {
                        color = color.Substring(1);
                    }

                    var r = byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                    var g = byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                    var b = byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber);

                    //await role.ModifyAsync(r => r.Color = new Color(r, g, b));
                }

                await ReplyAsync($"{gradientRoles.Count} roles updated!");
            }
        }
        
        private List<SocketRole> GetSortedRolesForColors(SocketGuild guild, RoleColors guildRoleColorSettings)
        {
            if (guild is null) throw new ArgumentNullException(nameof(guild));
            if (guildRoleColorSettings is null) throw new ArgumentNullException(nameof(guildRoleColorSettings));

            var allRolesSorted = new List<SocketRole>(guild.Roles.OrderByDescending(r => r.Position));

            var gradientRoles = new List<SocketRole>();

            var adding = false;

            foreach (var role in allRolesSorted)
            {
                if (role.Id == guildRoleColorSettings.Top.Id) adding = true;

                if (adding)
                {
                    gradientRoles.Add(role);
                }

                if (role.Id == guildRoleColorSettings.Bottom.Id)
                {
                    adding = false;
                    break;
                }
            }

            return gradientRoles; 
        }
    }
}
