using Battle.Attacks;
using ProLib.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Relics.ModifiedRelics
{
    class MatryoshkaDamageModifier : IDamageModifier
    {
        public const int MATRYOSHKA_SHELL_DAMAGE = -2;
        public const int MATRYOSHKA_SHELL_CRIT = -2;

        public float GetDamageModifier(Attack attack, RelicManager relicManager, int critCount, float damage)
        {
            if (relicManager.RelicEffectActive(RelicEffect.MATRYOSHKA))
            {
                if (critCount == 0) return -MATRYOSHKA_SHELL_DAMAGE;
                else return -MATRYOSHKA_SHELL_CRIT;
            }
            return 0f;
        }
    }
}
