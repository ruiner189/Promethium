using BepInEx.Configuration;
using ProLib.Orbs;
using Promethium.Patches.Relics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedDoctorb : ModifiedOrb
    {
        private static ModifiedDoctorb _instance;

        private static readonly string _name = OrbNames.Doctorb;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");
        private ModifiedDoctorb() : base(_name) { }

        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
        }

        public static ModifiedDoctorb Register()
        {
            if (_instance == null)
                _instance = new ModifiedDoctorb();
            return _instance;
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            if (CurseRelic.IsCurseLevelActive(2))
            {
                battleController.AddDamageMultiplier(0.5f);
            }
        }
    }
}
