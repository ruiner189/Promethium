using Promethium.Patches.Relics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedNosforbatu : ModifiedOrb
    {
        private static ModifiedNosforbatu _instance;
        private ModifiedNosforbatu() : base(OrbNames.VampireOrb) { }

        public static ModifiedNosforbatu Register()
        {
            if (_instance == null)
                _instance = new ModifiedNosforbatu();
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
