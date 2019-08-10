using System.IO;

namespace XModPackager.Build.XTools
{
    public static class XToolsUtils
    {
        private static readonly string RelativeXToolsPath = "steamapps/common/X Tools";

        public static string GetXCatToolPath()
        {
            return Path.Combine(PathUtils.GetSteamInstallPath(), RelativeXToolsPath, "XRCatTool");
        }
    }
}