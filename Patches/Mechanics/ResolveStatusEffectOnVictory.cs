using Battle.StatusEffects;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(BattleController), nameof(BattleController.CompleteVictory))]
    public static class ResolveStatusEffectOnVictory
    {
        public static void Postfix(BattleController __instance)
        {
            List<StatusEffect> effects = __instance._playerStatusEffectController._statusEffects;
            int attempts = 99;
            while (effects.Any(effect => StatusEffect.IsStatusEffectPerishable(effect.EffectType)) && attempts > 0)
            {
                __instance._playerStatusEffectController.ResolveStatusEffects();
                attempts--;
            }
        }
    }
}
