using System;
using System.Text.RegularExpressions;
namespace XModPackager.Config.Models
{
    public class ConfigModDetailsModel
    {
        private static readonly Regex ValidVersionRegex = new Regex(@"^\d{3}$");
        private string version;

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version
        {
            get => version;
            set {
                var isValid = value == null || ValidVersionRegex.IsMatch(value);

                if (!isValid)
                {
                    throw new ArgumentException("Invalid version for mod details (must be exactly three digits): " + value);
                }

                version = value;
            }
        }
        public bool? SaveDependent { get; set; }
    }
}
