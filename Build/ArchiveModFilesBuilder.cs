using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using Ionic.Zip;
using XModPackager.Config.Models;
using XModPackager.Template;
using XModPackager.Logging;

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


        public void BuildModFiles(string outputDirectory, IEnumerable<string> filesFromDisk, string contentFileText)
        {
            if (config.Build.ArchiveName == null)
            {
                throw new ArgumentException("Cannot build mod to archive when an archive name/template is not supplied");
            }

            var archiveFileName = processor.Process(config.Build.ArchiveName);
            var archivePath = Path.Combine(outputDirectory, archiveFileName);

            Logger.Log(LogCategory.Info, "Building mod to zip archive " + archivePath);

            using (var zipFile = new ZipFile())
            {

                try
                {
                    zipFile.UpdateEntry("content.xml", contentFileText);

                    zipFile.AddFiles(filesFromDisk);

                    zipFile.Save(archivePath);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to build mod archive: " + e.Message, e);
                }
            }
        }
    }
}
