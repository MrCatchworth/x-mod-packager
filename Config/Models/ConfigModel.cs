using System.Collections.Generic;

namespace XModPackager.Config.Models
{
    public class ConfigModel
    {
        public ConfigModDetailsModel ModDetails { get; set; }
        public ConfigBuildModel Build {get; set;}
    }
}
