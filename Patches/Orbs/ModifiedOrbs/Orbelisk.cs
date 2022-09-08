using Battle;
using Battle.Attacks.DamageModifiers;
using Cruciball;
using HarmonyLib;
using Promethium.Extensions;
using UnityEngine;
using Relics;
using I2.Loc;
using Promethium.Components;
using ProLib.Orbs;
using BepInEx.Configuration;
using ProLib.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Battle.Attacks;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedOrbelisk : ModifiedOrb
    {
        private static ModifiedOrbelisk _instance;
        private static readonly string _name = OrbNames.Orbelisk;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");
        private ModifiedOrbelisk() : base(_name)
        {
            LocalVariables = true;
        }

        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
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
            localParams.SetParameterValue(ParamKeys.DISCARD_DAMAGE, $"{GetHealthDiscardDamage(attack)}");
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
                    multiplier = (multiplier * armor.CurrentArmor.Value) + 1;
            }

            return multiplier;
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
                amount = (int) Mathf.Clamp(armor.CurrentArmor.Value, 0, 5);
            }
            return amount;
        }

        public float GetHealthDiscardDamage(Attack attack)
        {
            return GetArmorDiscardDamage(attack);
        }

        public float GetArmorHoldDamage(Attack attack)
        {
            return 3;
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (CustomRelicManager.RelicActive(RelicNames.HOLSTER))
            {
                ReplaceDescription(attack, new string[] { "attacks_flying_and_ground", "armor_damage_multiplier", "armor_damage_discard_multiplier", "armor_damage_hold_multiplier" });
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
            if (ModifiedOrb.GetOrb(OrbNames.Orbelisk, true).IsEnabled())
            {
                __result = 0;
                return false;
            }

            return true;
        }
    }
}
