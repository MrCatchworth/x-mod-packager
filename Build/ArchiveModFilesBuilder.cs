using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using Ionic.Zip;
using XModPackager.Config.Models;
using XModPackager.Template;

namespace XModPackager.Build
{
    public class ArchiveModFilesBuilder : IModFilesBuilder
    {
        private readonly ConfigModel config;
        private readonly IEnumerable<TemplateSpec> templates;
        private readonly TemplateProcessor processor;
        public ArchiveModFilesBuilder(ConfigModel config, IEnumerable<TemplateSpec> templates)
        {
            this.config = config;
            this.templates = templates;

            processor = new TemplateProcessor(templates);
        }


        public void BuildModFiles(string outputDirectory, IEnumerable<string> filesFromDisk, IDictionary<string, string> filesFromMemory)
        {
            if (config.ArchiveName == null)
            {
                throw new ArgumentException("Cannot build mod to archive when an archive name/template is not supplied");
            }

            var archiveFileName = processor.Process(config.ArchiveName);
            var archivePath = Path.Combine(outputDirectory, archiveFileName);

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

                zipFile.Save(archivePath);
            }
        }
    }
}
