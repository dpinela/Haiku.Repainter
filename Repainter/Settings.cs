using BepConfig = BepInEx.Configuration;
using MAPI = Modding;

namespace Haiku.Repainter
{
    internal class Settings
    {
        public BepConfig.ConfigEntry<string> Seed;
        public BepConfig.ConfigEntry<bool> ApplyOnStart;
        public BepConfig.ConfigEntry<bool> DisableInCreatorRooms;

        private const string MainGroup = "";

        public Settings(BepConfig.ConfigFile config, Action applyPalette)
        {
            Seed = config.Bind(MainGroup, "Seed", "", "Seed for palette generation (blank for random seed)");
            ApplyOnStart = config.Bind(MainGroup, "Apply Palette on Game Start", false);
            DisableInCreatorRooms = config.Bind(MainGroup, "Disable in Creator Rooms", false);
            MAPI.ConfigManagerUtil.createButton(config, applyPalette, MainGroup, "Apply Palette", "Generate and apply a palette now");
        }
    }
}