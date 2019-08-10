﻿using System.Xml.Linq;
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
using XModPackager.Logging;

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
            return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(PathUtils.ConfigPath));
        }

        private static void BuildMod(BuildOptions options, ConfigModel config)
        {
            var contentBuilder = new ContentBuilder(config, false);

            XDocument contentDocument;
            try
            {
                contentDocument = contentBuilder.BuildContent();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to build content.xml output: " + e.Message, e);
            }

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

            var filesToPackage = new BuildPathsProcessor(config).GetPathsToBuild(Directory.GetCurrentDirectory());

            try
            {
                Logger.Log(LogCategory.Info, "Building mod to directory " + config.Build.OutputDirectory);
                Directory.CreateDirectory(config.Build.OutputDirectory);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed to create build output directory at {config.Build.OutputDirectory}: " + e.Message,
                    e
                );
            }

            IModFilesBuilder builder;
            var buildOutputPath = config.Build.OutputDirectory;

            switch (config.Build.Method)
            {
                case BuildMethod.Loose:
                    builder = new LooseModFilesBuilder();
                    Logger.Log(LogCategory.Info, "Using build method: loose files");
                    break;
                case BuildMethod.Archive:
                    builder = new ArchiveModFilesBuilder(config, GetTemplateSpecs(config));
                    Logger.Log(LogCategory.Info, "Using build method: zip archive");
                    break;
                case BuildMethod.Cat:
                    builder = new CatModFilesBuilder(config);
                    Logger.Log(LogCategory.Info, "Using build method: catalogs");
                    break;
                default:
                    throw new ArgumentException("Invalid build method: " + config.Build.Method);
            }

            builder.BuildModFiles(buildOutputPath, filesToPackage, new Dictionary<string, string> {
                ["content.xml"] = contentStringBuilder.ToString()
            });
        }

        static int Main(string[] args)
        {
            ConfigModel config;
            try
            {
                config = LoadConfig();
            }
            catch (Exception e)
            {
                Logger.Log(LogCategory.Fatal, "Failed to load config: " + e.Message);
                return 1;
            }

            try
            {
                Parser.Default.ParseArguments<BuildOptions>(args)
                    .WithParsed(options => BuildMod(options, config));
            }
            catch (Exception e)
            {
                Logger.Log(LogCategory.Fatal, e.Message);
            }

            return 0;
        }
    }
}
