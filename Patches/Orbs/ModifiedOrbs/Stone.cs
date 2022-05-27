using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedStone : ModifiedOrb
    {
        private static ModifiedStone _instance;
        private ModifiedStone() : base(OrbNames.StoneOrb){}

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if(attack.Level > 1)
            {
                ReplaceDescription(attack, new string[] { "armor_max", "armor_turn"});
            }
        }
        public static ModifiedStone Register()
        {
            if (_instance == null)
                _instance = new ModifiedStone();
            return _instance;
        }
    }
}
