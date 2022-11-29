using Battle.Attacks;
using ProLib.Relics;
using Relics;

namespace Promethium.Patches.Relics.ModifiedRelics
{
    public class NoDiscardDamageModifier : IDamageModifier
    {
        public const int NO_DISCARD_RELIC_DAMAGE = 2;
        public const int NO_DISCARD_RELIC_CRIT = 2;
        public float GetDamageModifier(Attack attack, RelicManager relicManager, int critCount, float damage)
        {
            if (relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
            {
                if (critCount == 0) return -NO_DISCARD_RELIC_DAMAGE;
                else return -NO_DISCARD_RELIC_CRIT;
            }
            return 0f;
        }
    }
}
