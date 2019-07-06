using System.Collections.Generic;

namespace XModPackager.Config.Models
{
    public class ConfigModel
    {
        public ConfigModDetailsModel ModDetails { get; set; }
        public IEnumerable<string> ExcludePaths { get; set; }
        public string ArchiveName { get; set; }
        public string ArchiveDirectory { get; set; }
    }
}
