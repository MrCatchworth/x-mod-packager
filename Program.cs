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
using XModPackager.Build;

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

        private static void BuildMod(BuildOptions options, ConfigModel config)
        {
            var contentBuilder = new ContentBuilder(config, false);
            var contentDocument = contentBuilder.BuildContent();
            var contentStringBuilder = new StringBuilder();

            using (
                var contentWriter = XmlWriter.Create(contentStringBuilder, new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    "
                })
            )
            {
                contentDocument.WriteTo(contentWriter);
            }

            var filesToPackage = new PackagedPathsProcessor(config).GetPackagedPaths(Directory.GetCurrentDirectory());

            Directory.CreateDirectory(config.Build.OutputDirectory);

            IModFilesBuilder builder;
            var buildOutputPath = config.Build.OutputDirectory;
            if (options.Loose)
            {
                builder = new LooseModFilesBuilder();
            }
            else
            {
                builder = new ArchiveModFilesBuilder(config, GetTemplateSpecs(config));
            }

            builder.BuildModFiles(buildOutputPath, filesToPackage, new Dictionary<string, string> {
                ["content.xml"] = contentStringBuilder.ToString()
            });
        }

        static void Main(string[] args)
        {
            var config = LoadConfig();

            Parser.Default.ParseArguments<BuildOptions>(args)
                .WithParsed(options => BuildMod(options, config));
        }
    }
}
