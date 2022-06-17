using Cruciball;
using HarmonyLib;
using I2.Loc;
using PeglinUI;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public abstract class ModifiedOrb
    {
        public static readonly List<ModifiedOrb> AllModifiedOrbs = new List<ModifiedOrb>();

        private String _name;
        public bool LocalVariables = false;
        public readonly bool Registered = false;

        public ModifiedOrb(String orbName)
        {
            _name = orbName;
            if (Plugin.ConfigFile.Bind<bool>("Orbs", orbName, true, "Disable to remove modifications").Value)
            {
                Registered = true;
                Plugin.Log.LogDebug($"{_name} was successfully registered.");
            }

            AllModifiedOrbs.Add(this);
        }

        public static ModifiedOrb GetOrb(String name, bool includeAll = false)
        {
            if (includeAll)
                return AllModifiedOrbs.Find(orb => orb.GetName() == name);
            else
                return AllModifiedOrbs.Find(orb => orb.GetName() == name && orb.Registered);
        }

        public String GetName()
        {
            return _name;
        }

        public virtual void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack) { }

        public virtual void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack) { }
        public virtual void OnAddedToDeck(DeckManager deckManager, CruciballManager cruciballManager, GameObject orb, Attack attack) { }
        public virtual void OnRemovedFromBattleDeck(DeckManager deckManager, CruciballManager cruciballManager, GameObject orb, Attack attack) { }
        public virtual void OnRemovedFromDeck(DeckManager deckManager, GameObject orb, Attack attack) { }
        public virtual void OnDeckShuffle(BattleController battleController, GameObject orb, Attack attack) { }
        public virtual void OnEnemyTurnEnd(BattleController battleController, GameObject orb, Attack attack) { }
        public virtual void OnBattleStart(BattleController battleController, GameObject orb, Attack attack) { }

        public virtual void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb) { }

        public virtual void OnShotFired(BattleController battleController, GameObject orb, Attack attack) { }

        public virtual void ChangeDescription(Attack attack, RelicManager relicManager) { }
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
                if (s == desc)
                {
                    containsDesc = true;
                    break;
                }
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
            if (attack.locDescStrings.Length > position)
                attack.locDescStrings[position] = desc;
            else
                AddToDescription(attack, desc);
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
            if (attack != null)
            {
                ModifiedOrb orb = ModifiedOrb.GetOrb(attack.locNameString);
                if (orb != null) orb.OnShotFired(__instance, ____ball, attack);
            }

        }
    }

    [HarmonyPatch(typeof(BattleController), "AttemptOrbDiscard")]
    public static class OnDiscard
    {
        [HarmonyPriority(Priority.LowerThanNormal)]
        public static void Prefix(BattleController __instance, bool __runOriginal, RelicManager ____relicManager, int ____battleState, GameObject ____ball)
        {
            if (____battleState == 9 || !__runOriginal) return;
            if (____ball != null && ____ball.GetComponent<PachinkoBall>().available && !DeckInfoManager.populatingDisplayOrb && !GameBlockingWindow.windowOpen && __instance.NumShotsDiscarded < __instance.MaxDiscardedShots)
            {
                Attack attack = ____ball.GetComponent<Attack>();
                if (attack != null)
                {
                    ModifiedOrb orb = ModifiedOrb.GetOrb(attack.locNameString);
                    if (orb != null) orb.OnDiscard(____relicManager, __instance, ____ball, attack);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Attack), nameof(Attack.Description), MethodType.Getter)]
    public static class ChangeDescription
    {

        public static bool Prefix(Attack __instance, RelicManager ____relicManager, CruciballManager ____cruciballManager, ref String __result)
        {
            ModifiedOrb orb = ModifiedOrb.GetOrb(__instance.locNameString);

            if (orb == null || ____relicManager == null) return true;

            orb.ChangeDescription(__instance, ____relicManager);
            if (orb.LocalVariables)
            {
                LocalizationParamsManager localParams = __instance.GetComponent<LocalizationParamsManager>();
                if (localParams == null) localParams = __instance.gameObject.AddComponent<LocalizationParamsManager>();
                if (localParams != null)
                    orb.SetLocalVariables(localParams, __instance.gameObject, __instance);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Attack), nameof(Attack.SoftInit))]
    public static class AttackInit
    {
        public static void Postfix(Attack __instance, CruciballManager ____cruciballManager)
        {
            ModifiedOrb orb = ModifiedOrb.GetOrb(__instance.locNameString);
            if (orb != null)
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

    [HarmonyPatch(typeof(Attack), nameof(Attack.SetId))]
    public static class FixMirrorOrb
    {
        public static void Postfix(Attack __instance)
        {
            DeckManager deckManager = Resources.FindObjectsOfTypeAll<DeckManager>().FirstOrDefault();
            RelicManager relicManager = Resources.FindObjectsOfTypeAll<RelicManager>().FirstOrDefault();
            CruciballManager cruciballManager = Resources.FindObjectsOfTypeAll<CruciballManager>().FirstOrDefault();
            __instance.SoftInit(deckManager, relicManager, cruciballManager);
        }
    }

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.ArmBallForShot))]
    public static class ArmBall
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(GameObject ____ball)
        {
            ____ball.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.AddOrbToDeck))]
    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.AddOrbToDeckSilent))]
    public static class AddOrb
    {
        public static void Postfix(DeckManager __instance, GameObject orbPrefab)
        {
            Attack attack = orbPrefab.GetComponent<Attack>();
            if (attack != null)
            {
                ModifiedOrb orb = ModifiedOrb.GetOrb(attack.locNameString);
                if (orb != null)
                {
                    orb.OnAddedToDeck(__instance, attack._cruciballManager, orbPrefab, attack);
                }
            }
        }
    }

    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.RemoveOrbFromBattleDeck))]
    public static class RemoveOrbFromBattleDeck
    {
        public static void Postfix(DeckManager __instance, GameObject orb)
        {
            Attack attack = orb.GetComponent<Attack>();
            if (attack != null)
            {
                ModifiedOrb modifiedOrb = ModifiedOrb.GetOrb(attack.locNameString);
                if (modifiedOrb != null)
                {
                    modifiedOrb.OnRemovedFromBattleDeck(__instance, attack._cruciballManager, orb, attack);
                }
            }
        }
    }

    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.RemoveSpecifiedOrbFromDeck))]
    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.SoftRemoveSpecifiedOrbFromDeck))]
    public static class RemoveOrbFromDeck
    {
        public static void Postfix(DeckManager __instance, GameObject orb)
        {
            Attack attack = orb.GetComponent<Attack>();
            if (attack != null)
            {
                ModifiedOrb modifiedOrb = ModifiedOrb.GetOrb(attack.locNameString);
                if (orb != null)
                {
                    modifiedOrb.OnRemovedFromDeck(__instance, orb, attack);
                }
            }
        }
    }

    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.RemoveRandomOrbFromDeck))]
    public static class RemoveRandomOrbFromDeck
    {
        public static void Prefix(DeckManager __instance, out List<GameObject> __state)
        {
            __state = new List<GameObject>(DeckManager.completeDeck);
        }

        public static void Postfix(DeckManager __instance, List<GameObject> __state)
        {
            if (__state == null || DeckManager.completeDeck == null) return;
            foreach (GameObject orb in DeckManager.completeDeck)
            {
                __state.Remove(orb);
            }

            foreach (GameObject orb in __state)
            {
                Attack attack = orb.GetComponent<Attack>();
                if (attack != null)
                {
                    ModifiedOrb modifiedOrb = ModifiedOrb.GetOrb(attack.locNameString);
                    if (modifiedOrb != null)
                    {
                        modifiedOrb.OnRemovedFromDeck(__instance, orb, attack);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.ShuffleDeck))]
    public static class DeckShuffle
    {
        public static void Postfix(BattleController __instance)
        {

            foreach (GameObject orb in DeckManager.completeDeck)
            {
                Attack attack = orb.GetComponent<Attack>();
                if (attack != null)
                {
                    ModifiedOrb modifiedOrb = ModifiedOrb.GetOrb(attack.locNameString);
                    if (modifiedOrb != null)
                    {
                        modifiedOrb.OnDeckShuffle(__instance, orb, attack);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.EnemyTurnComplete))]
    public static class EnemyTurnEnd
    {
        public static void Postfix(BattleController __instance)
        {
            foreach (GameObject orb in DeckManager.completeDeck)
            {
                Attack attack = orb.GetComponent<Attack>();
                if (attack != null)
                {
                    ModifiedOrb modifiedOrb = ModifiedOrb.GetOrb(attack.locNameString);
                    if (modifiedOrb != null)
                    {
                        modifiedOrb.OnEnemyTurnEnd(__instance, orb, attack);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.Start))]
    public static class BattleStart
    {
        public static void Postfix(BattleController __instance)
        {
            if(DeckManager.completeDeck != null)
            {
                foreach (GameObject orb in DeckManager.completeDeck)
                {
                    Attack attack = orb.GetComponent<Attack>();
                    if (attack != null)
                    {
                        ModifiedOrb modifiedOrb = ModifiedOrb.GetOrb(attack.locNameString);
                        if (modifiedOrb != null)
                        {
                            modifiedOrb.OnBattleStart(__instance, orb, attack);
                        }
                    }
                }
            }

        }
    }




}
