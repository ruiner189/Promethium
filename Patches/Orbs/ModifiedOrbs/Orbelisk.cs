using Battle;
using Battle.Attacks.DamageModifiers;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Promethium.Patches.Mechanics;
using Promethium.Extensions;
using System.Collections.Generic;
using UnityEngine;
using Relics;
using I2.Loc;
using Promethium.Components;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{

    public sealed class ModifiedOrbelisk : ModifiedOrb
    {
        private static ModifiedOrbelisk _instance;
        private ModifiedOrbelisk() : base(OrbNames.Orbelisk)
        {
            LocalVariables = true;
        }

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            GameObject inventory = GameObject.Find("InventoryView");
            GameObject battleUpgrade = GameObject.Find("BattleUpgradesCanvas");
            if ((inventory != null && inventory.transform.GetChild(0).gameObject.activeInHierarchy) || (battleUpgrade != null && battleUpgrade.activeInHierarchy))
            {
                localParams.SetParameterValue(ParamKeys.ARMOR_DAMAGE_MULTIPLIER, $"x * {GetDamageShotMultiplier(attack, true)}");
                localParams.SetParameterValue(ParamKeys.HOLD_DAMAGE_MULTIPLIER, $"(x/2) * {GetDamageHoldMultiplier(attack, true)}");
            }
            else
            {
                localParams.SetParameterValue(ParamKeys.ARMOR_DAMAGE_MULTIPLIER, $"{GetDamageShotMultiplier(attack)}x");
                localParams.SetParameterValue(ParamKeys.HOLD_DAMAGE_MULTIPLIER, $"{GetDamageHoldMultiplier(attack)}x");
            }

            localParams.SetParameterValue(ParamKeys.DISCARD_ARMOR_DAMAGE, $"{GetArmorDiscardDamage(attack)}");
            localParams.SetParameterValue(ParamKeys.DISCARD_HEALTH_DAMAGE, $"{GetHealthDiscardDamage(attack)}");
            localParams.SetParameterValue(ParamKeys.ARMOR_HOLD_DAMAGE, $"{GetArmorHoldDamage(attack)}");
        }

        public float GetDamageShotMultiplier(Attack attack, bool showMath = false)
        {
            int level = attack.Level;

            float multiplier = 0;
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();


            if (armor != null)
            {

                if (level == 1) multiplier = 0.08f;
                else if (level == 2) multiplier = 0.1f;
                else if (level == 3) multiplier = 0.12f;

                if (!showMath)
                    multiplier *= armor.CurrentArmor.Value;
            }

            return 1 + multiplier;
        }

        public float GetDamageHoldMultiplier(Attack attack, bool showMath = false)
        {
            float multiplier = GetDamageShotMultiplier(attack, showMath) - 1;
            if (showMath) return multiplier + 1;
            return (multiplier * 0.5f) + 1;
        }

        public float GetArmorDiscardDamage(Attack attack)
        {
            float amount = 0;

            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                amount = armor.CurrentArmor.Value;
            }
            return amount;
        }

        public float GetHealthDiscardDamage(Attack attack)
        {
            float amount = 0;

            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                amount = armor.CurrentArmor.Value;
            }
            return amount;
        }

        public float GetArmorHoldDamage(Attack attack)
        {
            return 4;
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (relicManager != null && relicManager.RelicEffectActive(Relics.CustomRelicEffect.HOLSTER))
            {
                ReplaceDescription(attack, new string[] { "attacks_flying_and_ground", "armor_damage_multiplier", "armor_damage_hold_multiplier" });
            }
            else
            {
                ReplaceDescription(attack, new string[] { "attacks_flying_and_ground", "armor_damage_multiplier", "armor_damage_discard_multiplier" });
            }
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            float multiplier = GetDamageShotMultiplier(attack);
            if (multiplier > 0 && multiplier != 1)
                battleController._damageMultipliers.Add(multiplier);
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            Attack attack = heldOrb.GetComponent<Attack>();
            if (armor != null && attack != null)
            {
                float multiplier = GetDamageHoldMultiplier(attack);
                armor.RemoveArmor(GetArmorHoldDamage(attack));
                battleController.AddDamageMultiplier(multiplier);
            }
        }

        public override void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                float multiplier = GetDamageShotMultiplier(attack);
                float damageToSelf = GetHealthDiscardDamage(attack);
                armor.RemoveArmor(GetArmorDiscardDamage(attack));
                battleController.AddDamageMultiplier(multiplier);

                PlayerHealthController playerHealthController = battleController._playerHealthController;
                if (playerHealthController != null)
                {
                    playerHealthController.DealUnblockableDamage(damageToSelf);
                }
            }
        }

        public override int GetAttackValue(CruciballManager cruciballManager, Attack attack)
        {
            if (attack.Level == 1) return 1;
            if (attack.Level == 2) return 2;
            if (attack.Level == 3) return 3;
            return base.GetAttackValue(cruciballManager, attack);
        }

        public override int GetCritValue(CruciballManager cruciballManager, Attack attack)
        {
            if (attack.Level == 1) return 3;
            if (attack.Level == 2) return 5;
            if (attack.Level == 3) return 7;
            return base.GetCritValue(cruciballManager, attack);
        }

        public static ModifiedOrbelisk Register()
        {
            if (_instance == null)
                _instance = new ModifiedOrbelisk();
            return _instance;
        }
    }


    [HarmonyPatch(typeof(AddDamageForStonesInDeck), nameof(AddDamageForStonesInDeck.GetDamageMod))]
    public static class RemoveStoneBonusDamage
    {
        public static bool Prefix(ref float __result)
        {
            __result = 0;
            return false;
        }
    }
}
