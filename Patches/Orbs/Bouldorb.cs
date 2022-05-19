using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Promethium.Patches.Mechanics;
using Promethium.Extensions;
using Promethium.Patches.Orbs;
using System;
using UnityEngine;
using Relics;

namespace Promethium.Patches.Orbs
{

    public sealed class ModifiedBouldorb : ModifiedOrb
    {
        private static ModifiedBouldorb _instance;
        private ModifiedBouldorb() : base(OrbNames.Bouldorb){ }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (relicManager != null && relicManager.RelicEffectActive(Relics.CustomRelicEffect.HOLSTER))
            {
                ReplaceDescription(attack, "armor_hold", 3);
            }
            else
            {
                ReplaceDescription(attack, "armor_discard", 3);
            }
        }

        public override void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack)
        {
            CruciballManager cruciballManager = battleController.GetCruciballManager();
            PlayerStatusEffectController playerStatusEffectController = battleController.GetPlayerStatusEffectController();
            int original = Armor.currentArmor;
            int add = Armor.GetArmorDiscardFromOrb(attack, relicManager, cruciballManager);
            Armor.currentArmor = Mathf.Clamp(original + add, 0, Armor.GetTotalMaximumArmor(relicManager, cruciballManager));
            Armor.ChangeArmorDisplay(Armor.currentArmor - original, playerStatusEffectController);
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb) {
            CruciballManager cruciballManager = battleController.GetCruciballManager();
            PlayerStatusEffectController playerStatusEffectController = battleController.GetPlayerStatusEffectController();
            int original = Armor.currentArmor;
            int add = Armor.GetArmorHoldFromOrb(heldOrb.GetComponent<Attack>(), relicManager, cruciballManager);
            Armor.currentArmor = Mathf.Clamp(original + add, 0, Armor.GetTotalMaximumArmor(relicManager, cruciballManager));
            Armor.ChangeArmorDisplay(Armor.currentArmor - original, playerStatusEffectController);
        }


        public static ModifiedBouldorb Register()
        {
            if (_instance == null)
                _instance = new ModifiedBouldorb();
            return _instance;
        }
    }
}
