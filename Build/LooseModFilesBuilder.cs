using System.IO;
using System.Collections.Generic;

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

        public void BuildModFiles(string outputPath, IEnumerable<string> filesFromDisk, string contentFileText)
        {
            cachedDirectoriesCreated.Clear();

            foreach (var sourceFilePath in filesFromDisk)
            {
                var fileOutputPath = Path.Combine(outputPath, sourceFilePath);

                ensureDirectoryExists(Path.GetDirectoryName(fileOutputPath));
                File.Copy(sourceFilePath, fileOutputPath);
            }

            File.WriteAllText(Path.Combine(outputPath, "content.xml"), contentFileText);
        }
    }
}
