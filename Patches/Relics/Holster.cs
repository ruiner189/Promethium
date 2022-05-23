using HarmonyLib;
using Promethium.Extensions;
using Promethium.Patches.Mechanics;
using Promethium.Patches.Orbs;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public static class Holster
    {
        [HarmonyPatch(typeof(BattleController), "AttemptOrbDiscard")]
        public static class OnDiscard
        {
            [HarmonyPriority(Priority.First)]
            public static bool Prefix(BattleController __instance, RelicManager ____relicManager)
            {
                if (____relicManager != null && ____relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER))
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BattleController), "ShotFired")]
        public static class OnShotFired
        {
            public static void Prefix(BattleController __instance, RelicManager ____relicManager, int ____battleState, GameObject ____ball)
            {
                if (____battleState == 9 || ____relicManager == null || !____relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER) || Hold.HeldOrb == null) return;
                Attack attack = Hold.HeldOrb.GetComponent<Attack>();
                if (attack != null)
                {
                    ModifiedOrb orb = ModifiedOrb.GetOrb(attack.locNameString);
                    if (orb != null) orb.ShotWhileInHolster(____relicManager, __instance, ____ball, Hold.HeldOrb);
                }
            }
        }
    }
}
