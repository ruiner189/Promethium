using HarmonyLib;
using Promethium.Extensions;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Relics
{
    public class CustomRelic : Relic
    {
        private RelicPool _pool = RelicPool.RARE_SCENARIO;
        public static List<CustomRelic> AllCustomRelics = new List<CustomRelic>();

        public static CustomRelic GetCustomRelic(CustomRelicEffect effect)
        {
            return AllCustomRelics.Find(relic => relic.effect == (RelicEffect)effect);
        }
        public CustomRelic()
        {
            AllCustomRelics.Add(this);
        }

        public void SetPoolType(RelicPool pool)
        {
            _pool = pool;
        }

        public RelicPool GetPoolType()
        {
            return _pool;
        }
    }

    [HarmonyPatch(typeof(Attack), nameof(Attack.GetModifiedDamagePerPeg))]
    public static class AddAttackPerPeg
    {
        public static void Postfix(RelicManager ____relicManager, int critCount, ref float __result)
        {
            if (____relicManager == null) return;
            bool isCrit = critCount > 0;

            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_A))
                __result += 1;
            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_B) && !isCrit)
                __result += 2;
            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_C) && isCrit)
                __result += 2;
        }
    }


}
