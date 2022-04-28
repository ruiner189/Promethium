using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using PeglinMod.Patches.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinMod.Patches.Balls
{
    [HarmonyPatch(typeof(BattleController), "AttemptOrbDiscard")]
    public class OnDiscard
    {
        public static void Prefix(BattleController __instance, GameObject ____ball, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController)
        {
            if(____ball != null && __instance.NumShotsDiscarded < __instance.MaxDiscardedShots)
            {
                FireballAttack attack = ____ball.GetComponent<FireballAttack>();
                if(attack != null)
                {
                    String orbName = attack.locName;
                    int orbLevel = attack.Level;
                    int cruciballLevel = ____cruciballManager.currentCruciballLevel;

                    if (orbName == "Bouldorb")
                    {
                        int original = Armor.currentArmor;
                        int max = Armor.GetTotalMaximumArmor(____cruciballManager);
                        Armor.currentArmor = max;
                        Plugin.Log.LogMessage($"Bouldorb discarded. Armor set to { Armor.currentArmor } / {max}");
                        Armor.ChangeArmorDisplay(Armor.currentArmor - original, ____playerStatusEffectController);

                    }
                }
            }
        }
    }
}
