using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System;
using XModPackager.Config.Models;
using Newtonsoft.Json;
using XModPackager.Template;
using System.Collections.Generic;
using CommandLine;
using XModPackager.Options;
using XModPackager.Content;
using XModPackager.Build;
using XModPackager.Logging;
using XModPackager.Config;
using System.Linq;

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
            var config = ConfigDefaults.GetDefaultConfig();
            JsonConvert.PopulateObject(File.ReadAllText(PathUtils.ConfigPath), config, new JsonSerializerSettings {
                ObjectCreationHandling = ObjectCreationHandling.Reuse
            });

            return config;
        }

        private static void CleanOutputDirectory(CleanOptions options, ConfigModel config)
        {
            Logger.Log(LogCategory.Info, "Cleaning output folder " + config.Build.OutputDirectory);

            DirectoryInfo di = new DirectoryInfo(config.Build.OutputDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete(); 
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true); 
            }
        }

        private static void BuildMod(BuildContext context)
        {
            var contentBuilder = new ContentBuilder(context);

            if (context.Options.Workshop && context.Method != BuildMethod.Cats)
            {
                Logger.Log(LogCategory.Warning, "You have specified workshop mode, but the build method is not catalogs. Is this intentional?");
            }

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

            Logger.Log(LogCategory.Debug, $"{context.Config.Build.ExcludePaths.Count()} excluded path patterns:");
            Logger.Log(LogCategory.Debug, string.Join("\n", context.Config.Build.ExcludePaths));

            var filesToPackage = new BuildPathsProcessor(context.Config).GetPathsToBuild(Directory.GetCurrentDirectory());

            Logger.Log(LogCategory.Debug, $"{filesToPackage.Count} files to build:");
            Logger.Log(LogCategory.Debug, string.Join("\n", filesToPackage));

            try
            {
                Logger.Log(LogCategory.Info, "Building mod to directory " + context.Config.Build.OutputDirectory);
                Directory.CreateDirectory(context.Config.Build.OutputDirectory);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed to create build output directory at {context.Config.Build.OutputDirectory}: " + e.Message,
                    e
                );
            }

            IModFilesBuilder builder;
            var buildOutputPath = context.Config.Build.OutputDirectory;

            switch (context.Method)
            {
                case BuildMethod.Loose:
                    builder = new LooseModFilesBuilder();
                    Logger.Log(LogCategory.Info, "Using build method: loose files");
                    break;
                case BuildMethod.Archive:
                    builder = new ArchiveModFilesBuilder(context.Config, GetTemplateSpecs(context.Config));
                    Logger.Log(LogCategory.Info, "Using build method: zip archive");
                    break;
                case BuildMethod.Cats:
                    builder = new CatModFilesBuilder(context.Config);
                    Logger.Log(LogCategory.Info, "Using build method: catalogs");
                    break;
                default:
                    throw new ArgumentException("Invalid build method: " + context.Config.Build.Method);
            }

            builder.BuildModFiles(buildOutputPath, filesToPackage, contentStringBuilder.ToString());
        }

        static int Main(string[] args)
        {
            try
            {
                ConfigModel config = null;
                BaseOptions options = null;

                Parser.Default.ParseArguments<BuildOptions, CleanOptions, ImportOptions, InitOptions>(args)
                    .WithParsed(o => options = (BaseOptions)o);

                if (options == null)
                {
                    return 1;
                }

                if (options.Verbose)
                {
                    Logger.MinLogLevel = LogCategory.Debug;
                }
                if (options.Quiet)
                {
                    Logger.MinLogLevel = LogCategory.Warning;
                }

                var shouldLoadConfig = options is BuildOptions || options is CleanOptions;

                if (shouldLoadConfig)
                {
                    try
                    {
                        config = LoadConfig();
                    }
                    catch (Exception e)
                    {
                        Logger.Log(LogCategory.Fatal, "Failed to load config: " + e.Message);
                        return 1;
                    }

                    PathUtils.CheckOutputPath(config.Build.OutputDirectory);
                }

                if (options is BuildOptions buildOptions)
                {
                    BuildMod(new BuildContext(config, buildOptions));
                }
                else if (options is CleanOptions cleanOptions)
                {
                    CleanOutputDirectory(cleanOptions, config);
                }
                else if (options is ImportOptions importOptions)
                {
                    InitScript.ImportContentXml(importOptions);
                }
                else if (options is InitOptions initOptions)
                {
                    InitScript.InitNewConfig(initOptions);
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogCategory.Fatal, e.Message);
                if (e.InnerException != null)
                {
                    Logger.Log(LogCategory.Info, e.StackTrace);
                }
                return 1;
            }

            Logger.Log(LogCategory.Info, "Done!");
            return 0;
        }
    }
}
