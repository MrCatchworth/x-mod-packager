using System.Linq;
using System.Collections.Generic;
using Ionic.Zip;

namespace XModPackager.Build
{
    public class ArchiveModFilesBuilder : IModFilesBuilder
    {
        public void BuildModFiles(string outputPath, IEnumerable<string> filesFromDisk, IDictionary<string, string> filesFromMemory)
        {
            using (var zipFile = new ZipFile())
            {
                var filesFromDiskOnly = filesFromDisk.Where(file => (
                    !filesFromMemory.Keys.Contains(file)
                ));

                foreach (var fileFromMemory in filesFromMemory)
                {
                    zipFile.UpdateEntry(fileFromMemory.Key, fileFromMemory.Value);
                }

                zipFile.AddFiles(filesFromDiskOnly);

                zipFile.Save(outputPath);
            }
        }
    }
}
