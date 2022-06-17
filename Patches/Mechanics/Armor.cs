using Battle;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Relics;
using System;
using Promethium.Components;

namespace Promethium.Patches.Mechanics
{

    [HarmonyPatch(typeof(PlayerHealthController), nameof(PlayerHealthController.Damage))]
    public static class MitigateDamage
    {
        public static void Prefix(ref float damage)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                float originalDamage = damage;
                damage = Math.Max(damage - armor.CurrentArmor.Value, 0);
                float difference = originalDamage - damage;
                armor.RemoveArmor(difference);
            }
        }
    }

    [HarmonyPatch(typeof(BattleController), "Start")]
    public static class ResetArmorOnStart
    {
        [HarmonyPriority(Priority.First)]
        private static void Postfix(BattleController __instance, RelicManager ____relicManager, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController)
        {
            Plugin.PromethiumManager.GetComponent<ArmorManager>()?.Init(____relicManager, ____cruciballManager, ____playerStatusEffectController);
        }
    }

    [HarmonyPatch(typeof(BattleController), "EnemyTurnComplete")]
    public static class AddArmorOnTurn
    {
        private static void Postfix()
        {
            ArmorManager armorManager = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armorManager != null)
            {
                armorManager.AddArmor(armorManager.GetArmorPerTurnFromRelics());
            }
        }
    }





}
