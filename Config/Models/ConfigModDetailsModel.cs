using System;
using System.Collections.Generic;
namespace XModPackager.Config.Models
{
    public class ConfigModDetailsModel
    {
        private string version;

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public Dictionary<string, ConfigLangModel> Langs { get; set; }
        public IEnumerable<ConfigDependencyModel> Dependencies { get; set; }
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
        public bool? SaveDependent { get; set; }

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
