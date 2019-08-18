using System.Collections.Generic;
using XModPackager.Build;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XModPackager.Config.Models
{
    public class ConfigBuildModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildMethod Method { get; set; } = BuildMethod.Archive;
        public string ArchiveName { get; set; } = ConfigDefaults.DefaultArchiveName;
        public string OutputDirectory { get; set; } = ConfigDefaults.DefaultOutputPath;
        public IEnumerable<string> ExcludePaths { get; set; } = new List<string>(ConfigDefaults.DefaultExcludePaths);
        public Dictionary<string, IEnumerable<string>> Cats { get; set; } = new Dictionary<string, IEnumerable<string>>();
        public IEnumerable<string> ForceLoosePaths { get; set; }
    }
}
