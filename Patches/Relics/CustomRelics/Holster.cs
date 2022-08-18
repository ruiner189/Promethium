using ProLib.Orbs;
using ProLib.Relics;
using HarmonyLib;
using Promethium.Patches.Mechanics;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using UnityEngine;
using Battle.Attacks;

namespace Promethium.Patches.Relics
{
    public static class Holster
    {

        [HarmonyPatch(typeof(BattleController), "ShotFired")]
        public static class OnShotFired
        {
            public static void Prefix(BattleController __instance)
            {
                if (BattleController._battleState == BattleController.BattleState.NAVIGATION ||  !CustomRelicManager.RelicActive(RelicNames.HOLSTER) || Hold.HeldOrb == null) return;
                Attack attack = Hold.HeldOrb.GetComponent<Attack>();
                if (attack != null)
                {
                    ModifiedOrb orb = ModifiedOrb.GetOrb(attack.locNameString);
                    if (orb != null) orb.ShotWhileInHolster(__instance._relicManager, __instance, __instance._activePachinkoBall, Hold.HeldOrb);
                }
            }
        }
    }
}
