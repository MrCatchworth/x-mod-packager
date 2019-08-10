using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using XModPackager.Build.XTools;
using XModPackager.Config.Models;

namespace XModPackager.Build
{
    public class CatModFilesBuilder : IModFilesBuilder
    {
        private readonly ConfigModel config;

        public CatModFilesBuilder(ConfigModel config)
        {
            this.config = config;
        }

        public void BuildModFiles(string outputPath, IEnumerable<string> filesFromDisk, IDictionary<string, string> filesFromMemory)
        {
            var catToolPath = XToolsUtils.GetXCatToolPath();

            if (catToolPath == null)
            {
                throw new InvalidOperationException("Failed to determine path of cat tool executable");
            }

            foreach (var catInfo in config.Build.Cats)
            {
                var processStartInfo = new ProcessStartInfo() {
                    CreateNoWindow = true
                };

                processStartInfo.FileName = catToolPath;

                var catPath = catInfo.Key + ".cat";
                var catIncludes = catInfo.Value;

                // Build from current directory
                processStartInfo.ArgumentList.Add("-in");
                processStartInfo.ArgumentList.Add(".");

                // Apply correct filename
                processStartInfo.ArgumentList.Add("-out");
                processStartInfo.ArgumentList.Add(Path.Combine(outputPath, catPath));

                // Apply inclusions (hope regex flavours are compatible!)
                processStartInfo.ArgumentList.Add("-include");
                foreach (var includePattern in catIncludes)
                {
                    processStartInfo.ArgumentList.Add(includePattern);
                }

                // Apply exclusions
                processStartInfo.ArgumentList.Add("-exclude");
                foreach (var excludePattern in config.Build.ExcludePaths)
                {
                    processStartInfo.ArgumentList.Add(excludePattern);
                }

                using (var process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    process.Start();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Cat tool failed to generate {catPath}: exited with code {process.ExitCode}");
                    }
                }

                
            }
        }
    }
}