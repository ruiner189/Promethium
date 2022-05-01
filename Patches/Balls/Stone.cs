using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Balls
{
    public sealed class ModifiedStone : ModifiedOrb
    {
        private static ModifiedStone _instance;
        private ModifiedStone() : base("Stone"){}

        public override void ChangeDescription(Attack attack)
        {
            if(attack.Level > 1)
            {
                ReplaceDescription(attack, new string[] { "ArmorMax", "ArmorTurn"});
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
