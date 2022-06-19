using Battle.StatusEffects;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(BattleController), "CompleteVictory")]
    public static class ResolveStatusEffectOnVictory
    {
        public static void Postfix(BattleController __instance, PlayerStatusEffectController ____playerStatusEffectController)
        {
            List<StatusEffect> effects = ____playerStatusEffectController._statusEffects;
            int attempts = 99;
            while (effects.Any(effect => StatusEffect.IsStatusEffectPerishable(effect.EffectType)) && attempts > 0)
            {
                ____playerStatusEffectController.ResolveStatusEffects();
                attempts--;
            }
        }
    }
}
