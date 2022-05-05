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

namespace Promethium.Patches.Orbs
{

    public sealed class ModifiedOrbelisk : ModifiedOrb
    {
        private static ModifiedOrbelisk _instance;
        private ModifiedOrbelisk() : base("Orbelisk"){}

        public override void ChangeDescription(Attack attack)
        {
            ReplaceDescription(attack, new string[] { "attacks_flying_and_ground", "armor_damage_multiplier", "armor_damage_discard_multiplier"});
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            CruciballManager cruciballManager = battleController.GetCruciballManager();
            float multiplier = Armor.GetArmorDamageMultiplier(attack, cruciballManager);
            if (multiplier > 0)
                battleController.GetDamageMultipliers().Add(multiplier + 1);
            
        }

        public override void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack)
        {

            CruciballManager cruciballManager = battleController.GetCruciballManager();

            float multiplier = Armor.GetArmorDamageMultiplier(attack, cruciballManager);
            if (multiplier > 0)
            {
                PlayerStatusEffectController playerStatusEffectController = battleController.GetPlayerStatusEffectController();
                PlayerHealthController playerHealthController = battleController.GetPlayerHealthController();

                battleController.GetDamageMultipliers().Add(multiplier + 1);
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
