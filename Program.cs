using System.IO;
using System;
using XModPackager.Config.Models;
using Newtonsoft.Json;
using XModPackager.Config;
using XModPackager.Template;
using System.Collections.Generic;
using Ionic.Zip;

namespace XModPackager
{
    class Program
    {
        private static IEnumerable<TemplateSpec> GetTemplateSpecs(ConfigModel config)
        {
            return new TemplateSpec[] {
                new TemplateSpec {
                    Text = "id",
                    Process = text => config.Id
                },
                new TemplateSpec {
                    Text = "date",
                    Process = text => DateTime.Now.ToString("dd-MM-yyyy")
                }
            };
        }

        static ConfigModel LoadConfig()
        {
            try
            {
                return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(PathConstants.ConfigPath));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load config: " + e.Message);
                return null;
            }
        }

        static void Main(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();

            var config = LoadConfig();
            ConfigDefaults.ApplyDefaultConfig(config);

            var filesToPackage = new PackagedPathsProcessor(config).GetPackagedPaths(currentDir);

            var outputFileName = new TemplateProcessor(GetTemplateSpecs(config)).Process(config.ArchiveName);

            Directory.CreateDirectory(config.ArchiveDirectory);
            var outputPath = Path.Join(config.ArchiveDirectory, config.ArchiveName);

            using (var zipFile = new ZipFile(outputPath))
            {
                zipFile.AddFiles(filesToPackage);

                zipFile.Save();
            }
        }
    }
}
