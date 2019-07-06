using System;

namespace XModPackager.Template
{
    public class TemplateSpec
    {
        public string Text {get; set;}
        public Func<string, string> Process {get; set;}
    }
}