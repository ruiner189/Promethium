using Battle.StatusEffects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(BattleController), "CompleteVictory")]
    public static class ResolveStatusEffectOnVictory
    {
        public static void Postfix(BattleController __instance, PlayerStatusEffectController ____playerStatusEffectController)
        {
            List<StatusEffect> effects = (List<StatusEffect>) typeof(PlayerStatusEffectController).GetField("_statusEffects", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(____playerStatusEffectController);
            int attempts = 99;
            while(effects.Any(effect => StatusEffect.IsStatusEffectPerishable(effect.EffectType)) && attempts > 0)
            {
                ____playerStatusEffectController.ResolveStatusEffects();
                attempts--;
            }
        }
    }
}
