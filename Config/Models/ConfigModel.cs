using Newtonsoft.Json;

namespace XModPackager.Config.Models
{
    public class ConfigModel
    {
        [JsonProperty(Required = Required.Always)]
        public ConfigModDetailsModel ModDetails { get; set; } = new ConfigModDetailsModel();

        [JsonProperty(Required = Required.Always)]
        public ConfigBuildModel Build {get; set;} = new ConfigBuildModel();
    }
}
