﻿using Battle.StatusEffects;
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
        private ModifiedBouldorb() : base("Bouldorb"){ }

        public override void ChangeDescription(Attack attack)
        {
            AddToDescription(attack, "armor_discard_max", 3);
        }

        public override void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack)
        {
            CruciballManager cruciballManager = battleController.GetCruciballManager();
            PlayerStatusEffectController playerStatusEffectController = battleController.GetPlayerStatusEffectController();
            int original = Armor.currentArmor;
            int max = Armor.GetTotalMaximumArmor(relicManager, cruciballManager);
            Armor.currentArmor = max;
            Plugin.Log.LogMessage($"Bouldorb discarded. Armor set to { Armor.currentArmor } / {max}");
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