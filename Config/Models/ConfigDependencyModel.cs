using System;
using Newtonsoft.Json;

namespace XModPackager.Config.Models
{
    public class ConfigDependencyModel
    {
        private string version;

        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }
        public string WorkshopId { get; set; }
        public bool? Optional { get; set; } = false;
        public string Version
        {
            get => version;
            set
            {
                var isValid = value == null || ConfigGlobals.ValidVersionRegex.IsMatch(value);

                if (!isValid)
                {
                    throw new ArgumentException("Invalid version for dependency (must be exactly three digits): " + value);
                }

                version = value;
            }
        }

        public string GetId(bool preferWorkshop)
        {
            if (preferWorkshop && WorkshopId != null)
            {
                return WorkshopId;
            }
            else
            {
                return Id;
            }
        }
    }
}