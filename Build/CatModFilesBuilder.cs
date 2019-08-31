using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using XModPackager.Config.Models;
using XCatalogs;

namespace XModPackager.Build
{
    public class CatModFilesBuilder : IModFilesBuilder
    {
        private readonly ConfigModel config;

        public CatModFilesBuilder(ConfigModel config)
        {
            this.config = config;
        }

        public void BuildModFiles(string outputPath, IEnumerable<string> filesFromDisk, string contentFileText)
        {
            var filesLeftToWrite = new List<string>(filesFromDisk);

            var looseFilesToWrite = filesLeftToWrite.Where(path => config.Build.CatLoosePaths.Any(regex => regex.IsMatch(path))).ToList();
            foreach (var loosePath in looseFilesToWrite) {
                filesLeftToWrite.Remove(loosePath);
            }

            foreach (var catInfo in config.Build.Cats)
            {
                var catFile = new XCatalogFile();
                var catPath = catInfo.Key;
                var catPatterns = catInfo.Value;

                var pathsForThisCat = filesLeftToWrite.Where(path => catPatterns.Any(regex => regex.IsMatch(path))).ToList();
                foreach (var catItemPath in looseFilesToWrite) {
                    filesLeftToWrite.Remove(catItemPath);

                    catFile.Entries.Add(new XCatalogEntry(catItemPath));
                }

                catFile.Write(catPath);
            }

            var contentPath = Path.Combine(config.Build.OutputDirectory, "content.xml");
            File.WriteAllText(contentPath, contentFileText);
        }
    }
}