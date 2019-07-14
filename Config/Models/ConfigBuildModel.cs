using System.Collections.Generic;

namespace XModPackager.Config.Models
{
    public class ConfigBuildModel
    {
        public bool? Loose {get; set;} = false;
        public string ArchiveName {get; set;} = ConfigDefaults.DefaultArchiveName;
        public string OutputDirectory {get; set;} = ConfigDefaults.DefaultOutputPath;
        public IEnumerable<string> ExcludePaths { get; set; } = new List<string>(ConfigDefaults.DefaultExcludePaths);
    }
}