using System.Linq;
using System.Collections.Generic;
using XModPackager.Config.Models;

namespace XModPackager.Config
{
    public class ConfigDefaults
    {
        public static readonly string DefaultArchiveName = "{id}_{date}";
        public static readonly IList<string> DefaultExcludePaths = new string[] {
            @"^xmod\.json$",
            @"^\."
        };
        public static readonly string DefaultArchiveDirectory = ".xmodbuild";

        public static void ApplyDefaultConfig(ConfigModel config)
        {
            if (config.ExcludePaths == null)
            {
                config.ExcludePaths = new List<string>(DefaultExcludePaths).ToArray();
            }
            else
            {
                config.ExcludePaths = config.ExcludePaths.Concat(DefaultExcludePaths).ToArray();
            }

            if (config.ArchiveName == null)
            {
                config.ArchiveName = DefaultArchiveName;
            }

            if (config.ArchiveDirectory == null)
            {
                config.ArchiveDirectory = DefaultArchiveDirectory;
            }
        }
    }
}
