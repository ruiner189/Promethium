using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public static class ModifiedRelic
    {
        public static List<RelicEffect> ModifiedRelics = new List<RelicEffect>();
        public static readonly String AltText = $"_{Plugin.Name.ToLower()}";

        public const int MATRYOSHKA_SHELL_DAMAGE = -2;
        public const int MATRYOSHKA_SHELL_CRIT = -2;
        public const int NO_DISCARD_RELIC_DAMAGE = 2;
        public const int NO_DISCARD_RELIC_CRIT = 2;

        public const float MATRYOSHKA_SHELL_MULTIPLIER = 0.65f;
        public const float NO_DISCARD_RELIC_MULTIPLIER = 0.25f;

        public static float NO_DISCARD_RELIC_REMOVED_DISCARDS = 0;


        public static void AddRelic(RelicEffect relic, bool alwaysEnabled = false)
        {
            if (alwaysEnabled)
            {
                ModifiedRelics.Add(relic);
                Plugin.Log.LogDebug($"{relic} Successfully modified.");
                return;
            }

            if (Plugin.ConfigFile.Bind<bool>("Modified Relics", relic.ToString(), true, "Disable to remove modifications of this relic. Will revert to vanilla behavior").Value)
            {
                ModifiedRelics.Add(relic);
                Plugin.Log.LogDebug($"{relic} Successfully modified.");
            }
        }

        public static bool HasRelicEffect(RelicEffect effect)
        {
            return ModifiedRelics.Contains(effect);
        }



        public static float CalculateNoDiscardMultiplier()
        {
            float multiplier = 1 + (NO_DISCARD_RELIC_MULTIPLIER * NO_DISCARD_RELIC_REMOVED_DISCARDS);

            return multiplier;
        }

        [HarmonyPatch(typeof(Relic), nameof(Relic.descKey), MethodType.Getter)]
        public static class ChangeDescription
        {
            public static void Prefix(Relic __instance)
            {
                if (ModifiedRelics.Contains(__instance.effect)) __instance.descMod = "";
            }
            public static void Postfix(Relic __instance, ref String __result)
            {
                if (ModifiedRelics.Contains(__instance.effect)) __result += AltText;
            }
        }


        [HarmonyPatch(typeof(BattleController), nameof(BattleController.ShotFired))]
        public static class OnShotFired
        {
            public static void Prefix(BattleController __instance, int ____battleState, GameObject ____ball)
            {
                if (____battleState == 9) return;

                RelicManager relicManager = __instance._relicManager;
                if (relicManager != null)
                {
                    if (ModifiedRelic.HasRelicEffect(RelicEffect.MATRYOSHKA) && relicManager.AttemptUseRelic(RelicEffect.MATRYOSHKA))
                    {
                        __instance._damageMultipliers.Add(MATRYOSHKA_SHELL_MULTIPLIER);
                    }

                    if (ModifiedRelic.HasRelicEffect(RelicEffect.NO_DISCARD) && relicManager.AttemptUseRelic(RelicEffect.NO_DISCARD))
                    {
                        __instance._damageMultipliers.Add(CalculateNoDiscardMultiplier());
                    }
                }
            }
        }
    }
}
