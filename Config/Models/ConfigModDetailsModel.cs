using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XModPackager.Config.Models
{
    public class ConfigModDetailsModel
    {
        private string version;

        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }
        public string Description { get; set; } = "";

        [JsonProperty(Required = Required.Always)]
        public string Author { get; set; }
        public Dictionary<string, ConfigLangModel> Langs { get; set; } = new Dictionary<string, ConfigLangModel>();
        public IEnumerable<ConfigDependencyModel> Dependencies { get; set; } = new List<ConfigDependencyModel>();

        [JsonProperty(Required = Required.Always)]
        public string Version
        {
            get => version;
            set
            {
                var isValid = value == null || ConfigGlobals.ValidVersionRegex.IsMatch(value);

                if (!isValid)
                {
                    throw new ArgumentException("Invalid version for mod details (must be exactly three digits): " + value);
                }

                version = value;
            }
        }
        public bool? SaveDependent { get; set; } = true;

        public ConfigLangModel GetInfoForLanguage(string langId)
        {
            if (Langs.TryGetValue(langId, out var existingLang))
            {
                return new ConfigLangModel {
                    Title = existingLang.Title != null ? existingLang.Title : Title,
                    Description = existingLang.Description != null ? existingLang.Description : Description,
                    Author = existingLang.Author != null ? existingLang.Author : Author
                };
            }
            else
            {
                return new ConfigLangModel {
                    Title = Title,
                    Description = Description,
                    Author = Author
                };
            }
        }
    }
}
