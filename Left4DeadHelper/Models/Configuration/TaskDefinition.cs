﻿using System.Collections.Generic;

namespace Left4DeadHelper.Models.Configuration;

public class TaskDefinition
{
    public TaskDefinition()
    {
        Name = "";
        CronSchedule = "";
        ClassName = "";
        Settings = new Dictionary<string, object>();
    }

    public string Name { get; set; }
    public string CronSchedule { get; set; }
    public string ClassName { get; set; }
    public Dictionary<string, object> Settings { get; set; }
}
