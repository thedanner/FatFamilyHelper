﻿using System.Collections.Generic;

namespace Left4DeadHelper.Models.Configuration
{
    public class Left4DeadSettings
    {
        public Left4DeadSettings()
        {
            ServerInfo = new ServerInfo();
            Maps = new Maps();
        }

        public ServerInfo ServerInfo { get; set; }
        public Maps Maps { get; set; }
    }
}
