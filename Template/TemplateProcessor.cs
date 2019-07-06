using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
                var templateText = match.Groups[0].Value;

                var matchingSpec = templateSpecs.First(spec => spec.Text == templateText);
                return matchingSpec.Process(templateText);
            });
        }
    }
}
