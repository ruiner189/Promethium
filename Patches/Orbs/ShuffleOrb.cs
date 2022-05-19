using Battle;
using Promethium.Extensions;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs
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
            PegManager pegManager = (PegManager) typeof(BattleController).GetField("_pegManager", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(battleController);
            Attack attack = heldOrb.GetComponent<Attack>();
            if (attack != null && attack.Level > 1)
            {
                pegManager.ShuffleSpecialPegs(true);
            }
        }
    }
}
