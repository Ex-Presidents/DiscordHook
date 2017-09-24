using System.IO;
using SDG.Unturned;

namespace Modulator
{
    public static class ModulatorContants
    {
        public static string ServerPath => Directory.GetCurrentDirectory() + ServerSavedata.directory + "/" + Provider.serverID;

        public static string ModulatorPath => ServerPath + "/Modulator";
        public static string TranslationPath => ModulatorPath + "/Translations";
        public static string ConfigurationPath => ModulatorPath + "/Configurations";
    }
}
