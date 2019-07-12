using System.Linq;
using System;
using System.IO;
using System.Xml.Linq;
using XModPackager.Config.Models;

namespace XModPackager.Content
{
    public class ContentBuilder
    {
        private readonly ConfigModel config;
        private readonly bool workshopMode;
        private XDocument contentDocument;

        public ContentBuilder(ConfigModel config, bool workshopMode)
        {
            this.config = config;
            this.workshopMode = workshopMode;
        }

        private void AssertDocumentExists()
        {
            if (contentDocument == null)
            {
                throw new InvalidOperationException("A content document must be loaded first");
            }
        }

        private void SmokeTestDocument()
        {
            var root = contentDocument.Root;

            if (root == null)
            {
                throw new ArgumentException("Invalid document");
            }

            if (root.Name != "content")
            {
                throw new ArgumentException("Invalid document");
            }
        }

        private void ApplyRootDetails()
        {
            var root = contentDocument.Root;

            root.SetAttributeValue("id", config.ModDetails.Id);
            root.SetAttributeValue("name", config.ModDetails.Title);
            root.SetAttributeValue("description", config.ModDetails.Description);
            root.SetAttributeValue("author", config.ModDetails.Author);
            root.SetAttributeValue("version", config.ModDetails.Version);
            root.SetAttributeValue("date", DateTime.Now.ToString("yyyy-MM-dd"));
            root.SetAttributeValue("save", config.ModDetails.SaveDependent.Value ? null : "0");
        }

        private XElement GetLangElement(string langId)
        {
            return contentDocument.Root
                .Elements("text")
                .Where(ele => ((string)ele.Attribute("language")) == langId)
                .FirstOrDefault();
        }

        private void ApplyLang(string langId)
        {
            var langElement = GetLangElement(langId);

            if (langElement == null)
            {
                langElement = new XElement("text");
                contentDocument.Root.Add(langElement);
            }

            var langText = config.ModDetails.GetInfoForLanguage(langId);

            langElement.SetAttributeValue("language", langId);
            langElement.SetAttributeValue("name", langText.Title);
            langElement.SetAttributeValue("description", langText.Description);
            langElement.SetAttributeValue("author", langText.Author);
        }

        private void ApplyLangs()
        {
            foreach (var langId in ContentUtils.AutoLanguageIds)
            {
                ApplyLang(langId);
            }

            foreach (var langId in config.ModDetails.Langs.Keys)
            {
                ApplyLang(langId);
            }
        }

        private XElement GetModDependencyElement(string id)
        {
            return contentDocument.Root
                .Elements("dependency")
                .Where(ele => ((string)ele.Attribute("id")) == id)
                .FirstOrDefault();
        }

        private void ApplyDependency(ConfigDependencyModel dependency)
        {
            var idToUse = dependency.GetId(workshopMode);

            var dependencyElement = GetModDependencyElement(idToUse);

            if (dependencyElement == null)
            {
                dependencyElement = new XElement("dependency");
                contentDocument.Root.Add(dependencyElement);
            }

            dependencyElement.SetAttributeValue("id", idToUse);
            dependencyElement.SetAttributeValue("version", dependency.Version);
            dependencyElement.SetAttributeValue("optional", (dependency.Optional ?? false) ? "true" : null);
        }

        private void ApplyDependencies()
        {
            foreach (var dependency in config.ModDetails.Dependencies)
            {
                ApplyDependency(dependency);
            }
        }

        private void ApplyAllSteps()
        {
            ApplyRootDetails();
            ApplyLangs();
            ApplyDependencies();
        }

        public XDocument BuildContent(string templatePath)
        {
            contentDocument = XDocument.Load(templatePath);

            SmokeTestDocument();
            ApplyAllSteps();

            return contentDocument;
        }

        public XDocument BuildContent(Stream templateStream)
        {
            contentDocument = XDocument.Load(templateStream);

            SmokeTestDocument();
            ApplyAllSteps();

            return contentDocument;
        }

        public XDocument BuildContent(XDocument template)
        {
            contentDocument = template;

            SmokeTestDocument();
            ApplyAllSteps();

            return contentDocument;
        }

        public XDocument BuildContent()
        {
            contentDocument = new XDocument(new XElement("content"));

            SmokeTestDocument();
            ApplyAllSteps();

            return contentDocument;
        }
    }
}