using System.Collections.Generic;

namespace Left4DeadHelper.Models
{
    public class Maps
    {
        public Maps()
        {
            DefaultCategory = "normal";
            Categories = new Dictionary<string, List<string>>
            {
                { "normal", new List<string>() }
            };
        }

        public string DefaultCategory { get; set; }
        public Dictionary<string, List<string>> Categories { get; set; }
    }
}
