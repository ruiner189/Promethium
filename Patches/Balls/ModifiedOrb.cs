using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using I2.Loc;
using Promethium.Patches.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Balls
{
    public abstract class ModifiedOrb
    {
        private static List<ModifiedOrb> AllModifiedOrbs = new List<ModifiedOrb>();

        private String _name;        
        public ModifiedOrb(String orbName)
        {
            _name = orbName;
            AllModifiedOrbs.Add(this);
            Plugin.Log.LogMessage($"Registered {_name}");
        }

        public static ModifiedOrb GetOrb(String name)
        {
            return AllModifiedOrbs.Find(orb => orb.GetName() == name);
        }

        public String GetName()
        {
            return _name;
        }

        public virtual void OnDiscard(BattleController battleController, GameObject orb, Attack attack){}

        public virtual void OnShotFired(BattleController battleController, GameObject orb, Attack attack) {}

        public virtual void ChangeDescription(Attack attack) { }
        public virtual int GetAttackValue(CruciballManager cruciballManager, Attack attack)
        {
            return int.MinValue;
        }

        public virtual int GetCritValue(CruciballManager cruciballManager, Attack attack)
        {
            return int.MinValue;
        }

        protected static void AddToDescription(Attack attack, String desc, int position = -1)
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

        protected static void ReplaceDescription(Attack attack, String desc, int position)
        {
            attack.locDescStrings[position] = desc;
        }

        protected static void ReplaceDescription(Attack attack, String[] desc)
        {
            attack.locDescStrings = desc;
        }
    }

    [HarmonyPatch(typeof(BattleController), "ShotFired")]
    public static class OnShotFired
    {
        public static void Prefix(BattleController __instance, int ____battleState, GameObject ____ball)
        {
            if (____battleState == 9) return;
            Attack attack = ____ball.GetComponent<Attack>();
            if(attack != null)
            {
                ModifiedOrb orb = ModifiedOrb.GetOrb(attack.locName);
                if (orb != null) orb.OnShotFired(__instance, ____ball, attack);
            }

        }
    }

    [HarmonyPatch(typeof(BattleController), "AttemptOrbDiscard")]
    public static class OnDiscard
    {
        public static void Prefix(BattleController __instance, int ____battleState, GameObject ____ball) {
            if (____battleState == 9) return;
            if (__instance.NumShotsDiscarded >= __instance.MaxDiscardedShots) return;

            Attack attack = ____ball.GetComponent<Attack>();
            if (attack != null)
            {
                ModifiedOrb orb = ModifiedOrb.GetOrb(attack.locName);
                if (orb != null) orb.OnDiscard(__instance, ____ball, attack);
            }

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

            ModifiedOrb orb = ModifiedOrb.GetOrb(__instance.locName);
            if(orb != null)
                orb.ChangeDescription(__instance);
            
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
                        .Replace("%md", "" + (Armor.GetArmorDamageMultiplier(__instance, ____cruciballManager) + 1) + "x")
                        .Replace("%ac", "" + Armor.currentArmor)
                        + "</indent>\n";
                }
                else
                {
                    text = text + "<sprite name=\"BULLET\"><indent=8%>" + LocalizationManager.GetTranslation("Orbs/" + str, true, 0, true, true, __instance.gameObject, null, true) + "</indent>\n";
                }
            }
            __result = text;
            return false;
        }
    }


    [HarmonyPatch(typeof(Attack), nameof(Attack.SoftInit))]
    public static class AttackInit
    {
        public static void Postfix(Attack __instance, CruciballManager ____cruciballManager)
        {
            ModifiedOrb orb = ModifiedOrb.GetOrb(__instance.locName);
            if(orb != null)
            {
                int damage = orb.GetAttackValue(____cruciballManager, __instance);
                if (damage != int.MinValue)
                    __instance.DamagePerPeg = damage;
                int crit = orb.GetCritValue(____cruciballManager, __instance);
                if (crit != int.MinValue)
                    __instance.CritDamagePerPeg = crit;
            }
        }
        
    }
}
