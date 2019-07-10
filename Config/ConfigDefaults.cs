using System.Linq;
using System.Collections.Generic;
using XModPackager.Config.Models;
using System.Text.RegularExpressions;

namespace XModPackager.Config
{
    public class ConfigDefaults
    {
        public static readonly string DefaultArchiveName = "{id}_{date}.zip";
        public static readonly IList<string> DefaultExcludePaths = new string[] {
            $"^{Regex.Escape(PathUtils.ConfigPath)}$",
            $"^{Regex.Escape(PathUtils.ContentTemplatePath)}$",
            @"^\."
        };

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

            if (config.ModDetails.Dependencies == null)
            {
                config.ModDetails.Dependencies = new ConfigDependencyModel[0];
            }

            if (config.ArchiveName == null)
            {
                config.ArchiveName = DefaultArchiveName;
            }

            if (config.ArchiveDirectory == null)
            {
                config.ArchiveDirectory = PathUtils.OutputPath;
            }

            if (config.ModDetails.Version == null)
            {
                config.ModDetails.Version = "100";
            }

            if (config.ModDetails.Langs == null)
            {
                config.ModDetails.Langs = new Dictionary<string, ConfigLangModel>();
            }

            if (!config.ModDetails.SaveDependent.HasValue)
            {
                config.ModDetails.SaveDependent = true;
            }
        }
    }
}
