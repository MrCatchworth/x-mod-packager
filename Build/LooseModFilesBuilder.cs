using System.IO;
using System.Collections.Generic;

namespace XModPackager.Build
{
    public class LooseModFilesBuilder : IModFilesBuilder
    {
        public void BuildModFiles(string outputPath, IEnumerable<string> filesFromDisk, string contentFileText)
        {
            foreach (var sourceFilePath in filesFromDisk)
            {
                var fileOutputPath = Path.Combine(outputPath, sourceFilePath);

                FileUtils.CopyAndCreateDirectory(sourceFilePath, fileOutputPath, true);
            }

            File.WriteAllText(Path.Combine(outputPath, "content.xml"), contentFileText);
        }
    }
}
