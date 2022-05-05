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
    public sealed class ModifiedNosforbatu : ModifiedOrb
    {
        private static ModifiedNosforbatu _instance;
        public ModifiedNosforbatu() : base("Nosforbatu") { }

        public static ModifiedNosforbatu Register()
        {
            if (_instance == null)
                _instance = new ModifiedNosforbatu();
            return _instance;
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            RelicManager relicManager = battleController.GetRelicManager();
            if (CurseRelic.IsCurseLevelActive(relicManager, 2))
            {
                battleController.AddDamageMultiplier(0.5f);
            }
        }
    }
}
