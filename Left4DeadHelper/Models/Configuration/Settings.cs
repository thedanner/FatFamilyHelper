using System.Collections.Generic;

namespace Left4DeadHelper.Models.Configuration;

public class Settings
{
    public DiscordSettings DiscordSettings { get; set; } = new DiscordSettings();
    public Left4DeadSettings Left4DeadSettings { get; set;} = new Left4DeadSettings();
    public ShiftCodeSettings ShiftCodes { get; set; } = new ShiftCodeSettings();
    public MinecraftSettings? Minecraft { get; set; }
    public List<TaskDefinition> Tasks { get; set; } = new List<TaskDefinition>();
    public List<UserMapping> UserMappings { get; set; } = new List<UserMapping>();
}
