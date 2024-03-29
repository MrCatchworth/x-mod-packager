using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using XModPackager.Config.Models;

namespace XModPackager
{
    public class BuildPathsProcessor
    {
        private readonly ConfigModel configModel;

        public BuildPathsProcessor(ConfigModel config)
        {
            configModel = config;
        }

        private IList<Regex> getExclusionRegexes()
        {
            return configModel.Build.ExcludePaths
                .Append(new Regex("^" + Regex.Escape(configModel.Build.OutputDirectory)))
                .ToList();
        }

        private string normalisePath(string basePath, string path)
        {
            return Path.GetRelativePath(basePath, path).Replace('\\', '/');
        }

        public IList<string> GetPathsToBuild(string basePath)
        {
            var exclusions = getExclusionRegexes();
            var rawPaths = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);

            var normalisedPaths = rawPaths.Select(path => normalisePath(basePath, path)).ToArray();

            var filteredPaths = normalisedPaths.Where(path => {
                return !exclusions.Any(exclusion => {
                    return exclusion.Match(path).Success;
                });
            });

            return filteredPaths.ToList();
        }
    }
}
