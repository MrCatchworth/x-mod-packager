using System.Linq;
using System.Text.RegularExpressions;
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
        [JsonProperty(ItemConverterType = typeof(RegexConverter))]
        public IEnumerable<Regex> ExcludePaths { get; set; } = ConfigDefaults.DefaultExcludePaths.Select(path => new Regex(path)).ToList();
        [JsonProperty(ItemConverterType = typeof(ConfigCatsModelConverter))]
        public Dictionary<string, IEnumerable<Regex>> Cats { get; set; } = new Dictionary<string, IEnumerable<Regex>>();
        [JsonProperty(ItemConverterType = typeof(RegexConverter))]
        public IEnumerable<Regex> CatLoosePaths { get; set; } = new List<Regex>() {
            new Regex(@"\.pdf$"),
            new Regex(@"\.cur$"),
            new Regex(@"\.txt$")
        };
    }
}
