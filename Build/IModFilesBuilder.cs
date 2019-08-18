using System.Collections.Generic;
namespace XModPackager.Build
{
    public interface IModFilesBuilder
    {
        /// <summary>
        /// Perform actions to build the mod files to some location.
        /// </summary>
        /// <param name="outputPath">Where to put the combined file(s). Implementations may require this to be a file or a directory as necessary.</param>
        /// <param name="filesFromDisk">List of file paths to be read from disk.</param>
        /// <param name="filesFromMemory">Directory of files to write from memory. The key is the required output path, the value is the content of the file.</param>
        void BuildModFiles(string outputPath, IEnumerable<string> filesFromDisk, string contentFileText);
    }
}
