using Promethium.Extensions;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedRefreshOrb : ModifiedOrb
    {
        private static ModifiedRefreshOrb _instance;
        private ModifiedRefreshOrb() : base(OrbNames.Refreshorb) { }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            int level = attack.Level;
            if (relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER))
            {
                if (level == 2)
                {
                    ReplaceDescription(attack, "refresh_on_hold", 1);
                }
                else if (level == 3)
                {
                    ReplaceDescription(attack, "refresh_on_hold", 2);
                }
            }
        }
        public static ModifiedRefreshOrb Register()
        {
            if (_instance == null)
                _instance = new ModifiedRefreshOrb();
            return _instance;
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            Attack attack = heldOrb.GetComponent<Attack>();
            if (attack != null && attack.Level > 1)
            {
                Peg.OnPegDestroyedMutable?.Invoke(Peg.PegType.RESET);
                battleController.ResetField(false);
            }
        }
    }
}
