using Cruciball;
using HarmonyLib;
using I2.Loc;
using PeglinMod.Patches.Mechanics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PeglinMod.Patches.Balls
{

    public static class ChangeBall
    {
        
        public static void ChangeStats(GameObject obj, CruciballManager cruciball = null)
        {
            Attack attack = obj.GetComponent<Attack>();
            if (attack == null) return;

            string name = attack.locName;
            int level = attack.Level;

            if(name == "Orbelisk")
            {
                if(level == 1)
                {
                    attack.DamagePerPeg = 1;
                    attack.CritDamagePerPeg = 3;
                }

                if (level == 2)
                {
                    attack.DamagePerPeg = 2;
                    attack.CritDamagePerPeg = 5;
                }

                if (level == 3)
                {
                    attack.DamagePerPeg = 3;
                    attack.CritDamagePerPeg = 7;
                }

            }
        }

        public static void ChangeDescription(GameObject obj)
        {
            Attack attack = obj.GetComponent<Attack>();
            if (attack == null) return;

            string name = attack.locName;
            int level = attack.Level;

            if (name == "Stone")
            {
                if (level > 1)
                {
                    ReplaceDescription(attack, "ArmorMax", 0);
                    AddToDescription(attack, "ArmorTurn", 1);
                }
            } else if (name == "Bouldorb")
            {
                AddToDescription(attack, "ArmorDiscardMax", 3);
            } else if (name == "Orbelisk")
            {
                ReplaceDescription(attack, new string[] {"attacks_flying_and_ground", "ArmorDamageMultiplier", "ArmorDamageDiscardMultiplier"});
            }
        }

        private static void AddToDescription(Attack attack, String desc, int position = -1)
        {
            if (attack.locDescStrings == null || attack.locDescStrings.Length == 0) return;
            if (position == -1) position = attack.locDescStrings.Length;
            bool containsDesc = false;
            foreach (String s in attack.locDescStrings)
            {
                if (s == desc) containsDesc = true;
            }
            if (!containsDesc)
            {
                String[] newDesc = new String[attack.locDescStrings.Length + 1];
                int i = 0;
                for (int j = 0; j < newDesc.Length; j++)
                {
                    if (j == position)
                        newDesc[j] = desc;
                    else
                    {
                        newDesc[j] = attack.locDescStrings[i];
                        i++;
                    }

                }

                attack.locDescStrings = newDesc;
            }
        }

        private static void ReplaceDescription(Attack attack, String desc, int position)
        {
            attack.locDescStrings[position] = desc;
        }

        private static void ReplaceDescription(Attack attack, String[] desc)
        {
            attack.locDescStrings = desc;
        }
    }

    [HarmonyPatch(typeof(Attack), nameof(Attack.SoftInit))]
    public static class AttackInit
    {
        public static void Postfix(Attack __instance, CruciballManager ____cruciballManager)
        {
            ChangeBall.ChangeStats(__instance.gameObject, ____cruciballManager);
        }
    }


    [HarmonyPatch(typeof(Attack), nameof(Attack.Description), MethodType.Getter)]
    public static class ChangeDescription
    {
        public static Dictionary<String, String> stringDict;

        private static void CreateDictionary()
        {
            stringDict = new Dictionary<String, String>();

            stringDict.Add("ArmorMax", "Increases Maximum <color=\"purple\">Armor</color> by <color=\"purple\">%am</color>");
            stringDict.Add("ArmorTurn", "Restores <color=\"purple\">%ar</color> <color=\"purple\">Armor</color> every reload");
            stringDict.Add("ArmorDiscardMax", "Restores <color=\"purple\">Armor</color> to max if discarded");
            stringDict.Add("ArmorDiscard", "Restores <color=\"purple\">Armor</color> by <color=\"purple\">%ad</color> if discarded");
            stringDict.Add("ArmorDamageMultiplier", "Multiplies damage based on current <color=\"purple\">Armor</color>. Current multiplier: <color=\"purple\">%md</color>");
            stringDict.Add("ArmorDamageDiscardMultiplier", "Discard to transfer multiplier to the next orb. Takes away all <color=\"purple\">Armor</color> and damages you for <color=\"red\">%ac</color>");
        }
        public static bool Prefix(Attack __instance, CruciballManager ____cruciballManager, ref String __result)
        {
            if (stringDict == null)
                CreateDictionary();

            ChangeBall.ChangeDescription(__instance.gameObject);

            string text = "";
            foreach (string str in __instance.locDescStrings)
            {
                if (str == null || str == "") continue;
                if (stringDict.ContainsKey(str))
                {
                    text = text + "<sprite name=\"BULLET\"><indent=8%>" + stringDict[str]
                        .Replace("%am", "" + Armor.GetArmorMaxFromOrb(__instance, ____cruciballManager))
                        .Replace("%ar", "" + Armor.GetArmorReloadFromOrb(__instance, ____cruciballManager))
                        .Replace("%ad", "" + Armor.GetArmorDiscardFromOrb(__instance, ____cruciballManager))
                        .Replace("%ma", "" + Armor.GetTotalMaximumArmor(____cruciballManager))
                        .Replace("%md", "" + (int) (Armor.GetArmorDamageMultiplier(__instance, ____cruciballManager) * 100) + "%")
                        .Replace("%ac", "" + Armor.currentArmor)
                        + "</indent>\n";
                } else
                {
                    text = text + "<sprite name=\"BULLET\"><indent=8%>" + LocalizationManager.GetTranslation("Orbs/" + str, true, 0, true, true, __instance.gameObject, null, true) + "</indent>\n";
                }
            }

            __result = text;
            return false;
        }
    }

}
