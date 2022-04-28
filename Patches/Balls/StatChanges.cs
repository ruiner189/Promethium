using Cruciball;
using HarmonyLib;
using I2.Loc;
using PeglinMod.Patches.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinMod.Patches.Balls
{

    public static class ChangeBall
    {
        
        public static void ChangeStats(GameObject obj, CruciballManager cruciball)
        {
            FireballAttack attack = obj.GetComponent<FireballAttack>();
            if (attack == null) return;

            string name = attack.locName;
            int level = attack.Level;
        }

        public static void ChangeDescription(GameObject obj)
        {
            FireballAttack attack = obj.GetComponent<FireballAttack>();
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
            }
        }

        private static void AddToDescription(FireballAttack attack, String desc, int position)
        {
            if (attack.locDescStrings == null || attack.locDescStrings.Length == 0) return;
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

        private static void ReplaceDescription(FireballAttack attack, String desc, int position)
        {
            attack.locDescStrings[position] = desc;
        }
    }

    [HarmonyPatch(typeof(FireballAttack), nameof(FireballAttack.Initialize))]
    public static class FireBallInit
    {
        public static void Postfix(FireballAttack __instance, CruciballManager ____cruciballManager)
        {
            ChangeBall.ChangeStats(__instance.gameObject, ____cruciballManager);
        }
    }

    [HarmonyPatch(typeof(Attack), nameof(Attack.GetModifiedDamagePerPeg))]
    public static class GetModifiedDamage
    {
        public static void Prefix(Attack __instance, CruciballManager ____cruciballManager)
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
            stringDict.Add("ArmorDiscardMax", "Restores <color=\"purple\">Armor</color> to Max(<color=\"purple\">%ma</color>) if discarded");
            stringDict.Add("ArmorDiscard", "Restores <color=\"purple\">Armor</color> by <color=\"purple\">%ad</color> if discarded");
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
