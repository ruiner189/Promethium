using Battle;
using Battle.Attacks.DamageModifiers;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using PeglinMod.Patches.Mechanics;
using System.Collections.Generic;
using UnityEngine;

namespace Promethium.Patches.Balls
{
    [HarmonyPatch(typeof(AddDamageForStonesInDeck), nameof(AddDamageForStonesInDeck.GetDamageMod))]
    public static class RemoveStoneBonusDamage
    {
        public static bool Prefix(ref float __result)
        {
            __result = 0;
            return false;
        }
    }

    [HarmonyPatch(typeof(BattleController), "ShotFired")]
    public static class ApplyMultiplierOnShot
    {
        public static void Prefix(int ____battleState, GameObject ____ball, List<float> ____damageMultipliers, CruciballManager ____cruciballManager)
        {
            if (____battleState == 9) return;
            Attack attack = ____ball.GetComponent<Attack>();
            if(attack != null)
            {
                if(attack.locName == "Orbelisk")
                {
                    float multiplier = Armor.GetArmorDamageMultiplier(attack, ____cruciballManager);
                    if(multiplier > 0)
                    {
                        ____damageMultipliers.Add(multiplier + 1);
                    }
                }
            }

        }
    }

    [HarmonyPatch(typeof(BattleController), "AttemptOrbDiscard")]
    public static class ApplyMultiplierOnDiscard
    {
        public static void Prefix(BattleController __instance, int ____battleState, GameObject ____ball, List<float> ____damageMultipliers, PlayerHealthController ____playerHealthController, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController)
        {
            if (____battleState == 9) return;
            if (__instance.NumShotsDiscarded >= __instance.MaxDiscardedShots) return;
            Attack attack = ____ball.GetComponent<Attack>();
            if (attack != null)
            {
                if (attack.locName == "Orbelisk")
                {
                    float multiplier = Armor.GetArmorDamageMultiplier(attack, ____cruciballManager);
                    if (multiplier > 0)
                    {
                        ____damageMultipliers.Add(multiplier + 1);
                        int armorDamage = Armor.currentArmor;
                        Armor.currentArmor = 0;
                        Armor.ChangeArmorDisplay(-armorDamage, ____playerStatusEffectController);
                        ____playerHealthController.DealUnblockableDamage(armorDamage);
                    }
                }
            }

        }
    }
}
