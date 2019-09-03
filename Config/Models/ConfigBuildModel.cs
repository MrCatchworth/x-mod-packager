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
        public BuildMethod Method { get; set; }
        public string ArchiveName { get; set; }
        public string OutputDirectory { get; set; }
        [JsonProperty(ItemConverterType = typeof(RegexConverter))]
        public IEnumerable<Regex> ExcludePaths { get; set; }
        public Dictionary<string, IEnumerable<Regex>> Cats { get; set; } = new Dictionary<string, IEnumerable<Regex>>();
        public IEnumerable<Regex> CatLoosePaths { get; set; } = new List<Regex>();
    }
}
