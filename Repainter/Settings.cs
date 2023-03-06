using BepConfig = BepInEx.Configuration;
using MAPI = Modding;

namespace Haiku.Repainter
{
    internal class Settings
    {
        public BepConfig.ConfigEntry<string> Seed;
        public BepConfig.ConfigEntry<bool> ApplyOnStart;

        private const string MainGroup = "";

        public Settings(BepConfig.ConfigFile config)
        {
            Seed = config.Bind(MainGroup, "Seed", "", "Seed for palette generation (blank for random seed)");
            ApplyOnStart = config.Bind(MainGroup, "Apply Palette on Game Start", false);
            MAPI.ConfigManagerUtil.createButton(config, ApplyImmediately, MainGroup, "Apply Palette", "Generate and apply a palette now");
        }

        private void ApplyImmediately()
        {
            
        }
    }
}