using Battle;
using Promethium.Extensions;
using Promethium.Patches.Relics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedShuffleOrb : ModifiedOrb
    {
        private static ModifiedShuffleOrb _instance;
        private ModifiedShuffleOrb() : base(OrbNames.ShuffleOrb) { }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            int level = attack.Level;
            if (relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER))
            {
                if (level == 2)
                {
                    ReplaceDescription(attack, "shuffle_on_hold", 1);
                }
                else if (level == 3)
                {
                    ReplaceDescription(attack, "shuffle_on_hold", 2);
                }
            }
        }
        public static ModifiedShuffleOrb Register()
        {
            if (_instance == null)
                _instance = new ModifiedShuffleOrb();
            return _instance;
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            PegManager pegManager = battleController._pegManager;
            Attack attack = heldOrb.GetComponent<Attack>();
            if (attack != null && attack.Level > 1)
            {
                pegManager.ShuffleSpecialPegs(true);
            }
        }
    }
}
