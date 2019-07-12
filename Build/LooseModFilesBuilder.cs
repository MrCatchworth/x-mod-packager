using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace XModPackager.Build
{
    public class LooseModFilesBuilder : IModFilesBuilder
    {
        private ISet<string> cachedDirectoriesCreated = new HashSet<string>();

        private void ensureDirectoryExists(string directory)
        {
            if (cachedDirectoriesCreated.Contains(directory))
            {
                return;
            }

            Directory.CreateDirectory(directory);
            cachedDirectoriesCreated.Add(directory);
        }

        public void BuildModFiles(string outputPath, IEnumerable<string> filesFromDisk, IDictionary<string, string> filesFromMemory)
        {
            cachedDirectoriesCreated.Clear();

            var filesFromDiskOnly = filesFromDisk.Where(file => (
                !filesFromMemory.Keys.Contains(file)
            ));

            foreach (var fileFromMemory in filesFromMemory)
            {
                var fileOutputPath = Path.Combine(outputPath, fileFromMemory.Key);

                ensureDirectoryExists(Path.GetDirectoryName(fileOutputPath));
                File.WriteAllText(fileOutputPath, fileFromMemory.Value);
            }

            foreach (var sourceFilePath in filesFromDiskOnly)
            {
                var fileOutputPath = Path.Combine(outputPath, sourceFilePath);

                ensureDirectoryExists(Path.GetDirectoryName(fileOutputPath));
                File.Copy(sourceFilePath, fileOutputPath);
            }
        }
    }
}