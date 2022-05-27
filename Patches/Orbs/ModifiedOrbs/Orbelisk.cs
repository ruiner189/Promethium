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

namespace Promethium.Patches.Orbs.ModifiedOrbs
{

    public sealed class ModifiedOrbelisk : ModifiedOrb
    {
        private static ModifiedOrbelisk _instance;
        private ModifiedOrbelisk() : base(OrbNames.Orbelisk){}

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (relicManager != null && relicManager.RelicEffectActive(Relics.CustomRelicEffect.HOLSTER))
            {
                ReplaceDescription(attack, new string[] { "attacks_flying_and_ground", "armor_damage_multiplier", "armor_damage_hold_multiplier" });
            } else
            {
                ReplaceDescription(attack, new string[] { "attacks_flying_and_ground", "armor_damage_multiplier", "armor_damage_discard_multiplier" });
            }
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            CruciballManager cruciballManager = battleController._cruciballManager;
            float multiplier = Armor.GetArmorDamageMultiplier(attack, cruciballManager);
            if (multiplier > 0)
                battleController._damageMultipliers.Add(multiplier + 1);
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            CruciballManager cruciballManager = battleController._cruciballManager;
            float multiplier = (Armor.GetArmorDamageMultiplier(heldOrb.GetComponent<Attack>(), cruciballManager) / 2);
            if (multiplier > 0)
            {
                PlayerStatusEffectController playerStatusEffectController = battleController._playerStatusEffectController;
                battleController._damageMultipliers.Add(multiplier + 1);
                int originalArmor = Armor.currentArmor;
                int armorDamage = 4;
                Armor.currentArmor = Mathf.Max(Armor.currentArmor - armorDamage, 0);
                Armor.ChangeArmorDisplay(Armor.currentArmor - originalArmor, playerStatusEffectController);
            }
        }

        public override void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack)
        {

            CruciballManager cruciballManager = battleController._cruciballManager;

            float multiplier = Armor.GetArmorDamageMultiplier(attack, cruciballManager);
            if (multiplier > 0)
            {
                PlayerStatusEffectController playerStatusEffectController = battleController._playerStatusEffectController;
                PlayerHealthController playerHealthController = battleController._playerHealthController;

                battleController._damageMultipliers.Add(multiplier + 1);
                int armorDamage = Armor.currentArmor;
                Armor.currentArmor = 0;
                Armor.ChangeArmorDisplay(-armorDamage, playerStatusEffectController);
                playerHealthController.DealUnblockableDamage(armorDamage);
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
