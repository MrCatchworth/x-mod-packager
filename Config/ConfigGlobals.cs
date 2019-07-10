using System.Text.RegularExpressions;

namespace XModPackager.Config
{
    public class ConfigGlobals
    {
        public static readonly Regex ValidVersionRegex = new Regex(@"^\d{3}$");
    }
}