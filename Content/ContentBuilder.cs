using System.Linq;
using System;
using System.IO;
using System.Xml.Linq;
using XModPackager.Config.Models;
using XModPackager.Build;

namespace XModPackager.Content
{
    public class ContentBuilder
    {
        private readonly BuildContext context;
        private XDocument contentDocument;

        public ContentBuilder(BuildContext context)
        {
            this.context = context;
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
                throw new ArgumentException("Content document has no root element");
            }

            if (root.Name != "content")
            {
                throw new ArgumentException("Content document's root element is not a \"content\" tag");
            }
        }

        private void ApplyRootDetails()
        {
            var root = contentDocument.Root;

            var idToUse = context.Options.Workshop
                ? context.Config.ModDetails.WorkshopId
                : context.Config.ModDetails.Id;

            root.SetAttributeValue("id", idToUse != null ? idToUse : "");
            root.SetAttributeValue("name", context.Config.ModDetails.Title);
            root.SetAttributeValue("description", context.Config.ModDetails.Description);
            root.SetAttributeValue("author", context.Config.ModDetails.Author);
            root.SetAttributeValue("version", context.Config.ModDetails.Version);
            root.SetAttributeValue("date", DateTime.Now.ToString("yyyy-MM-dd"));
            root.SetAttributeValue("save", context.Config.ModDetails.SaveDependent.Value ? null : "0");
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

            var langText = context.Config.ModDetails.GetInfoForLanguage(langId);

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

            foreach (var langId in context.Config.ModDetails.Langs.Keys)
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
            var idToUse = dependency.GetId(context.Options.Workshop);

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
            foreach (var dependency in context.Config.ModDetails.Dependencies)
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