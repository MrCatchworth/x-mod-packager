using System.Collections.Generic;

namespace XModPackager.Config.Models
{
    public class ConfigModel
    {
        public IEnumerable<string> ExcludePaths { get; set; }
        public string ArchiveName { get; set; }
        public string ArchiveDirectory {get; set;}
        public string Id {get; set;}
    }
}
