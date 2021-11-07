using System.Collections.Generic;

namespace Left4DeadHelper.Models.Configuration
{
    public class Settings
    {
        public Settings()
        {
            DiscordSettings = new DiscordSettings();
            ShiftCodes = new ShiftCodeSettings();
            Left4DeadSettings = new Left4DeadSettings();
            Tasks = new List<TaskDefinition>();
            UserMappings = new List<UserMapping>();
        }

        public DiscordSettings DiscordSettings { get; set; }
        public ShiftCodeSettings ShiftCodes { get; set; }
        public Left4DeadSettings Left4DeadSettings { get; set;}
        public List<TaskDefinition> Tasks { get; set; }
        public List<UserMapping> UserMappings { get; set; }
    }
}
