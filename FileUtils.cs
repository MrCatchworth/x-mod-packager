using System.IO;

namespace XModPackager
{
    internal static class FileUtils
    {
        internal static void CopyAndCreateDirectory(string sourcePath, string destinationPath, bool overwrite = false)
        {
            var destinationDir = Path.GetDirectoryName(destinationPath);

            Directory.CreateDirectory(destinationDir);
            File.Copy(sourcePath, destinationPath, overwrite);
        }
    }
}
