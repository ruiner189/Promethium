using Battle.Attacks.DamageModifiers;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Promethium.Extensions;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public bool IsEnabled()
        {
            if (this is CurseRelic) return true;
            return Plugin.ConfigFile.Bind<bool>("Custom Relics", locKey, true, "Disable to remove from relic pool. The relic itself is still in the game.").Value;
        }
    }

    [HarmonyPatch(typeof(Attack), nameof(Attack.GetModifiedDamagePerPeg))]
    public class ChangeAttackPerPeg
    {
        public static void Postfix(Attack __instance, int critCount, ref float __result)
        {
            RelicManager relicManager = __instance._relicManager;
            if (__instance._relicManager != null)
            {
                int currentValue = 0;
                bool isCrit = critCount > 0;
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_BALANCE))
                    currentValue += 1;
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_ATTACK) && !isCrit)
                    currentValue += 2;
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_CRIT) && isCrit)
                    currentValue += 2;
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_ATTACK) && !isCrit)
                    currentValue += 2;
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_CRIT) && isCrit)
                    currentValue += 2;

                __result += currentValue;
            }
        }
    }

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.MaxDiscardedShots), MethodType.Getter)]
    public static class ChangeDiscards
    {
        public static void Postfix(RelicManager ____relicManager, ref int __result)
        {
            if (____relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER))
                __result = 0;
            if (____relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
                __result = 0;
        }
    }
}
