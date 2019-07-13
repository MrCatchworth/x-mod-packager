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
            if (config.Build.ExcludePaths == null)
            {
                config.Build.ExcludePaths = new List<string>(DefaultExcludePaths).ToArray();
            }
            else
            {
                config.Build.ExcludePaths = config.Build.ExcludePaths.Concat(DefaultExcludePaths).ToArray();
            }

            if (config.ModDetails.Dependencies == null)
            {
                config.ModDetails.Dependencies = new ConfigDependencyModel[0];
            }

            if (config.Build.ArchiveName == null)
            {
                config.Build.ArchiveName = DefaultArchiveName;
            }

            if (config.Build.OutputDirectory == null)
            {
            config.Build.OutputDirectory = PathUtils.OutputPath;
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
