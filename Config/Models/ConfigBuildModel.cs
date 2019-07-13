using System.Collections.Generic;

namespace XModPackager.Config.Models
{
    public class ConfigBuildModel
    {
        public bool? Loose {get; set;}
        public string ArchiveName {get; set;}
        public string OutputDirectory {get; set;}
        public IEnumerable<string> ExcludePaths { get; set; }
    }
}