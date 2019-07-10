using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System;
using XModPackager.Config.Models;
using Newtonsoft.Json;
using XModPackager.Config;
using XModPackager.Template;
using System.Collections.Generic;
using Ionic.Zip;
using CommandLine;
using XModPackager.Options;
using XModPackager.Content;

namespace XModPackager
{
    class Program
    {
        private static IEnumerable<TemplateSpec> GetTemplateSpecs(ConfigModel config)
        {
            return new TemplateSpec[] {
                new TemplateSpec {
                    Text = "id",
                    Process = text => config.ModDetails.Id
                },
                new TemplateSpec {
                    Text = "date",
                    Process = text => DateTime.Now.ToString("dd-MM-yyyy")
                },
                new TemplateSpec {
                    Text = "title",
                    Process = text => {
                        var rawTitle = config.ModDetails.Title;

                        rawTitle = Regex.Replace(rawTitle, @"(?:\s|\.)+", "-");
                        rawTitle = Regex.Replace(rawTitle, @"[^A-Za-z0-9\-_]+", "");
                        rawTitle = rawTitle.Trim('-', '_');

                        return rawTitle;
                    }
                },
                new TemplateSpec {
                    Text = "version",
                    Process = text => {
                        var rawVersion = config.ModDetails.Version;

                        var builder = new StringBuilder();

                        builder.Append(rawVersion[0]);
                        builder.Append('-');
                        builder.Append(rawVersion[1]);
                        builder.Append('-');
                        builder.Append(rawVersion[2]);

                        return builder.ToString();
                    }
                }
            };
        }

        static ConfigModel LoadConfig()
        {
            try
            {
                return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(PathUtils.ConfigPath));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load config: " + e.Message);
                return null;
            }
        }

        private static void CreateArchive(ConfigModel config)
        {
            var contentBuilder = new ContentBuilder(config, false);
            var contentDocument = contentBuilder.BuildContent();

            using (
                var contentWriter = XmlWriter.Create("content.xml", new XmlWriterSettings {
                    Indent = true,
                    IndentChars = "    "
                })
            ) {
                contentDocument.WriteTo(contentWriter);
            }

            var filesToPackage = new PackagedPathsProcessor(config).GetPackagedPaths(Directory.GetCurrentDirectory());

            var outputFileName = new TemplateProcessor(GetTemplateSpecs(config)).Process(config.ArchiveName);

            Directory.CreateDirectory(config.ArchiveDirectory);
            var outputPath = Path.Join(config.ArchiveDirectory, outputFileName);

            using (var zipFile = new ZipFile(outputPath))
            {
                zipFile.AddFiles(filesToPackage);
                zipFile.Save();
            }
        }

        static void Main(string[] args)
        {
            var config = LoadConfig();
            ConfigDefaults.ApplyDefaultConfig(config);

            Parser.Default.ParseArguments<PackageOptions>(args)
                .WithParsed(packageOptions => CreateArchive(config));
        }
    }
}
