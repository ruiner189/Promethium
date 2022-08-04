using BepInEx.Configuration;
using ProLib.Orbs;
using Promethium.Patches.Relics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedNosforbatu : ModifiedOrb
    {
        private static ModifiedNosforbatu _instance;
        private static readonly string _name = OrbNames.VampireOrb;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");

        private ModifiedNosforbatu() : base(_name) { }

        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
        }
        public static ModifiedNosforbatu Register()
        {
            if (_instance == null)
                _instance = new ModifiedNosforbatu();
            return _instance;
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            RelicManager relicManager = battleController._relicManager;
            if (CurseRelic.IsCurseLevelActive(2))
            {
                battleController.AddDamageMultiplier(0.5f);
            }
        }
    }
}
