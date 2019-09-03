using System.Linq;
using System.Collections.Generic;
using XModPackager.Config.Models;
using System.Text.RegularExpressions;
using XModPackager.Build;

namespace XModPackager.Config
{
    public class ConfigDefaults
    {
        public static ConfigModel GetDefaultConfig()
        {
            return new ConfigModel {
                ModDetails = {
                    SaveDependent = true
                },

                Build = {
                    Method = BuildMethod.Archive,
                    ArchiveName = "{id}_{date}.zip",
                    OutputDirectory = ".xmodbuild",
                    ExcludePaths = new Regex[] {
                        new Regex("^" + Regex.Escape(PathUtils.ConfigPath) + "$"),
                        new Regex("^" + Regex.Escape(PathUtils.ContentTemplatePath) + "$"),
                        new Regex(@"[^/\\]\.")
                    }
                }
            };
        }
    }
}
