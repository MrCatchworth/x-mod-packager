using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace XModPackager.Template
{
    public class TemplateProcessor
    {
        private static readonly Regex TemplateRegex = new Regex(@"\{(\w+?)\}");
        private readonly IEnumerable<TemplateSpec> templateSpecs;

        public TemplateProcessor(IEnumerable<TemplateSpec> specs)
        {
            templateSpecs = specs;
        }

        public string Process(string input)
        {
            return TemplateRegex.Replace(input, match => {
                var templateText = match.Groups[1].Value;

                var matchingSpec = templateSpecs.FirstOrDefault(spec => spec.Text == templateText);

                if (matchingSpec == null)
                {
                    throw new ArgumentException($"Unrecognised template \"{templateText}\" at position {match.Groups[1].Index}");
                }

                return matchingSpec.Process(templateText);
            });
        }
    }
}
