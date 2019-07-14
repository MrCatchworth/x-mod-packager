using System.Linq;
using System.Collections.Generic;
using XModPackager.Config.Models;
using System.Text.RegularExpressions;

namespace XModPackager.Config
{
    public class ConfigDefaults
    {
        public static readonly string DefaultArchiveName = "{id}_{date}.zip";
        public static readonly string DefaultOutputPath = ".xmodbuild";
        public static readonly IList<string> DefaultExcludePaths = new string[] {
            $"^{Regex.Escape(PathUtils.ConfigPath)}$",
            $"^{Regex.Escape(PathUtils.ContentTemplatePath)}$",
            @"^\."
        };
    }
}
