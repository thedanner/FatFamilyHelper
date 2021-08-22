﻿using System.Collections.Generic;

namespace Left4DeadHelper.Models.Configuration
{
    public class Settings
    {
        public Settings()
        {
            DiscordSettings = new DiscordSettings();
            Left4DeadSettings = new Left4DeadSettings();
            Tasks = new List<TaskDefinition>();
            UserMappings = new UserMapping[0];
        }

        public DiscordSettings DiscordSettings { get; set; }
        public Left4DeadSettings Left4DeadSettings { get; set;}
        public List<TaskDefinition> Tasks { get; set; }
        public UserMapping[] UserMappings { get; set; }
    }
}