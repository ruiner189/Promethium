using Battle;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Promethium.Patches.Relics;
using Promethium.Patches.Status_Effect;
using Promethium.Extensions;
using Relics;
using System;
using UnityEngine;

namespace Promethium.Patches.Mechanics
{
    public static class Armor
    {

        public static int currentArmor = 0;
        public static StatusEffect armorEffect;

        private static PlayerStatusEffectController _controller;

        public static void ChangeArmorDisplay(int change, PlayerStatusEffectController controller = null)
        {
            if (controller != null) _controller = controller;
            if (change == 0) return;

            armorEffect = new StatusEffect((StatusEffectType)CustomStatusEffect.Armor, change);

            if(_controller != null)
                _controller.ApplyStatusEffect(armorEffect);

        }


        public static int GetArmorMaxFromOrb(Attack ball, CruciballManager cruciballManager)
        {
            String orbName = ball.locName;
            int orbLevel = ball.Level;
            int cruciballLevel = cruciballManager != null ? cruciballManager.currentCruciballLevel : -1;

            int amount = 0;

            if(orbName == "Stone" && orbLevel > 1)
            {
                amount = (orbLevel - 1) * 3;

                if (cruciballLevel >= 3)
                    amount = (orbLevel - 1) * 2;
            }
            return amount;
        }

        public static int GetArmorReloadFromOrb(Attack ball, CruciballManager cruciballManager)
        {
            String orbName = ball.locName;
            int orbLevel = ball.Level;
            int cruciballLevel = cruciballManager != null ? cruciballManager.currentCruciballLevel : -1;

            int amount = 0;

            if (orbName == "Stone" && orbLevel > 1)
            {
                amount = (orbLevel - 1) * 2;

                if (cruciballLevel >= 3)
                    amount = orbLevel - 1;
            }

            return amount;
        }

        public static int GetArmorDiscardFromOrb(Attack ball, RelicManager relicManager, CruciballManager cruciballManager)
        {
            String orbName = ball.locName;
            int orbLevel = ball.Level;
            int cruciballLevel = cruciballManager != null ? cruciballManager.currentCruciballLevel : -1;

            int amount = 0;

            if(orbName == "Bouldorb")
            {
                amount = GetTotalMaximumArmor(relicManager, cruciballManager);
            }

            return amount;
        }

        public static float GetArmorDamageMultiplier(Attack ball, CruciballManager cruciballManager)
        {
            String orbName = ball.locName;
            int orbLevel = ball.Level;
            int cruciballLevel = cruciballManager != null ? cruciballManager.currentCruciballLevel : -1;

            if(orbName == "Orbelisk")
            {
                if(orbLevel == 1)
                    return currentArmor * 0.08f;
                else if (orbLevel == 2)
                    return currentArmor * 0.10f;
                else if (orbLevel == 3)
                    return currentArmor * 0.12f;
            }

            return 0;
        }

        public static int GetTotalMaximumArmor(RelicManager relicManager, CruciballManager cruciballManager)
        {
            int total = 0;

            foreach(GameObject obj in DeckManager.completeDeck)
            {
                FireballAttack attack = obj.GetComponent<FireballAttack>();
                if(attack != null)
                {
                    total += GetArmorMaxFromOrb(attack, cruciballManager);
                }
            }

            if(relicManager != null)
            {
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_TWO_B))
                    total += 5;
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_FOUR_B))
                    total += 5;
            }


                return total;
        }

        public static int GetTotalArmorPerReload(CruciballManager cruciballManager)
        {
            int total = 0;

            foreach (GameObject obj in DeckManager.completeDeck)
            {
                FireballAttack attack = obj.GetComponent<FireballAttack>();
                if (attack != null)
                {
                    total += GetArmorReloadFromOrb(attack, cruciballManager);
                }
            }
            return total;
        }

        public static int GetTotalArmorPerTurn(RelicManager relicManager, CruciballManager crucibalManager)
        {
            int total = 0;
            if(relicManager != null)
            {
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_TWO_C))
                    total += 1;
                if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_FOUR_C))
                    total += 1;

            }
            return total;
        }
    }

    [HarmonyPatch(typeof(PlayerHealthController), nameof(PlayerHealthController.Damage))]
    public static class MitigateDamage
    {
        public static void Prefix(ref float damage)
        {
            if(Armor.currentArmor > 0)
            {
                float originalDamage = damage;
                damage = Math.Max(damage - Armor.currentArmor, 0);
                float difference = originalDamage - damage;
                Armor.currentArmor = (int) Math.Max(Armor.currentArmor - difference, 0);
                Armor.ChangeArmorDisplay((int) difference * -1);
            }
        }
    }

    [HarmonyPatch(typeof(BattleController), "ShuffleDeck")]
    public static class ResetArmorAfterReloading
    {
        private static void Postfix(RelicManager ____relicManager, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController)
        {
            int original = Armor.currentArmor;
            int max = Armor.GetTotalMaximumArmor(____relicManager, ____cruciballManager);
            int additional = Armor.currentArmor + Armor.GetTotalArmorPerReload(____cruciballManager);
            Armor.currentArmor = Math.Min(max, additional);
            Armor.ChangeArmorDisplay(Armor.currentArmor - original ,____playerStatusEffectController);
        }
    }

    [HarmonyPatch(typeof(BattleController), "Start")]
    public static class ResetArmorOnStart
    {
        private static void Postfix(RelicManager ____relicManager, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController)
        {
            int max = Armor.GetTotalMaximumArmor(____relicManager, ____cruciballManager);
            int additional = Armor.GetTotalArmorPerReload(____cruciballManager) + Armor.GetTotalArmorPerTurn(____relicManager, ____cruciballManager);
            Armor.currentArmor = Math.Min(max, additional);
            Armor.ChangeArmorDisplay(Armor.currentArmor, ____playerStatusEffectController);
        }
    }

    [HarmonyPatch(typeof(BattleController), "EnemyTurnComplete")]
    public static class AddArmorOnTurn
    {
        private static void Postfix(RelicManager ____relicManager, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController)
        {
            int original = Armor.currentArmor;
            int max = Armor.GetTotalMaximumArmor(____relicManager, ____cruciballManager);
            int additional = Armor.currentArmor + Armor.GetTotalArmorPerTurn(____relicManager, ____cruciballManager);
            Armor.currentArmor = Math.Min(max, additional);
            Armor.ChangeArmorDisplay(Armor.currentArmor - original, ____playerStatusEffectController);
        }
    }





}
