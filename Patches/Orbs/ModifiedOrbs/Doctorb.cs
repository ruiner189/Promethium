using Promethium.Patches.Relics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedDoctorb : ModifiedOrb
    {
        private static ModifiedDoctorb _instance;
        private ModifiedDoctorb() : base(OrbNames.Doctorb) { }

        public static ModifiedDoctorb Register()
        {
            if (_instance == null)
                _instance = new ModifiedDoctorb();
            return _instance;
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            RelicManager relicManager = battleController._relicManager;
            if (CurseRelic.IsCurseLevelActive(relicManager, 2))
            {
                battleController.AddDamageMultiplier(0.5f);
            }
        }
    }
}
