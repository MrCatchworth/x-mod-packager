using System.Xml.Linq;
using System.IO;
using System;
using XModPackager.Config.Models;
using Newtonsoft.Json;
using XModPackager.Options;
using XModPackager.Logging;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace XModPackager
{
    internal static class InitScript
    {
        private static TResult ReadFromPrompt<TResult>(string prompt, Func<string, TResult> responseParser, bool hasDefaultResponse, TResult defaultResponse)
        {
            var complete = false;
            TResult responseValue;

            while (!complete)
            {
                Console.Write(prompt + " ");
                if (hasDefaultResponse)
                {
                    Console.Write($"({defaultResponse.ToString()}) ");
                }

                var rawResponse = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(rawResponse))
                {
                    return defaultResponse;
                }

                try
                {
                    responseValue = responseParser(rawResponse);
                    return responseValue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sorry, that wasn't valid, please enter another value");
                }
            }

            return defaultResponse;
        }

        public static void ImportContentXml(ImportOptions options)
        {
            try
            {
                Logger.Log(LogCategory.Info, "Importing configuration from content.xml ...");

                var config = new ConfigModel();

                if (!File.Exists("content.xml"))
                {
                    throw new IOException("Existing content.xml file not found for importing");
                }

                if (File.Exists(PathUtils.ConfigPath))
                {
                    throw new IOException($"Config file {PathUtils.ConfigPath} already exists - don't want to overwrite it");
                }

                var document = XDocument.Load("content.xml");

                var root = document.Root;

                var id = root.Attribute("id")?.Value;
                if (string.IsNullOrWhiteSpace(id))
                {
                    id = "";
                    Logger.Log(LogCategory.Warning, "Your content.xml does not appear to have an ID set - please set one in the config before trying to build the mod!");
                }
                else
                {
                    Logger.Log(LogCategory.Info, "ID: " + id);
                }
                config.ModDetails.Id = id;

                var title = root.Attribute("name")?.Value;
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = "";
                    Logger.Log(LogCategory.Warning, "Your content.xml does not appear to have a title set - please set one in the config before trying to build the mod!");
                }
                else
                {
                    Logger.Log(LogCategory.Info, "Mod name: " + title);
                }
                config.ModDetails.Title = title;

                var version = root.Attribute("version")?.Value;
                if (string.IsNullOrWhiteSpace(version))
                {
                    version = "";
                    Logger.Log(LogCategory.Warning, "Your content.xml does not appear to have a version set - please set one in the config before trying to build the mod!");
                }
                else
                {
                    Logger.Log(LogCategory.Info, "Version: " + version);
                }
                config.ModDetails.Version = version;


                config.ModDetails.Description = root.Attribute("description")?.Value ?? "";
                config.ModDetails.Author = root.Attribute("author")?.Value ?? "";
                config.ModDetails.Version = root.Attribute("version")?.Value ?? "";

                var saveValue = root.Attribute("description")?.Value.Trim().ToLower() ?? "";
                if (saveValue == "0" || saveValue == "false" || saveValue == "no")
                {
                    config.ModDetails.SaveDependent = false;
                }

                var dependencyElements = root.Elements("dependency");
                var dependencyList = new List<ConfigDependencyModel>();
                foreach (var ele in dependencyElements)
                {
                    var dependencyVersion = ele.Attribute("version")?.Value ?? "";
                    var dependencyId = ele.Attribute("id")?.Value ?? "";
                    var optional = ele.Attribute("optional")?.Value ?? "";

                    if (!string.IsNullOrWhiteSpace(dependencyId))
                    {
                        var dependency = new ConfigDependencyModel();

                        dependency.Id = dependencyId;
                        
                        if (!string.IsNullOrWhiteSpace(dependencyVersion))
                        {
                            dependency.Version = dependencyVersion;
                        }

                        if (optional == "true")
                        {
                            dependency.Optional = true;
                        }

                        dependencyList.Add(dependency);
                    }
                }

                config.ModDetails.Dependencies = dependencyList;

                var serialiserSettings = new JsonSerializerSettings {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                };

                var newConfigText = JsonConvert.SerializeObject(config, serialiserSettings);
                using (var file = File.Open(PathUtils.ConfigPath, FileMode.CreateNew))
                {
                    var writer = new StreamWriter(file);
                    writer.Write(newConfigText);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogCategory.Error, "Could not import config from content.xml: " + e.Message);
            }
        }
    }
}