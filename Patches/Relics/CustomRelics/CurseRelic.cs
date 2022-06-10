using Battle;
using Battle.Enemies;
using Battle.StatusEffects;
using HarmonyLib;
using Promethium.Extensions;
using Promethium.Patches.Mechanics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Worldmap;

namespace Promethium.Patches.Relics
{

    public class CurseRelic : CustomRelic
    {
        public static List<CurseRelic> AllCurseRelics = new List<CurseRelic>();
        public int CurseLevel;

        public CurseRelic() : base()
        {
            AllCurseRelics.Add(this);
        }

        public static List<CurseRelic> GetCurseRelicOfLevel(int level)
        {
            return AllCurseRelics.FindAll(relic => relic.CurseLevel == level);
        }

        public static bool IsCurseLevelActive(RelicManager relicManager, int curseLevel)
        {
            if(relicManager != null)
            {
                foreach (CurseRelic relic in AllCurseRelics)
                {
                    if (relic.CurseLevel == curseLevel)
                    {
                        if (relicManager.RelicEffectActive(relic.effect))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int AmountOfCurseRelics(RelicManager relicManager)
        {
            return AllCurseRelics.Where(relic => relicManager.RelicEffectActive(relic.effect)).Count();
        }
    }

    [HarmonyPatch(typeof(Enemy), nameof(Enemy.Initialize))]
    public static class EnemyInit
    {
        public static void Postfix(Enemy __instance, RelicManager relicManager, ref float ____maxHealth)
        {
            if (CurseRelic.IsCurseLevelActive(relicManager, 1))
            {
                int amountOfCurse = CurseRelic.AmountOfCurseRelics(relicManager);
                float multiplier = (float)(Plugin.TierOneHealthMultiplier * Math.Pow(Plugin.ExponentialCurseHealthMultiplier, amountOfCurse - 1));
                ____maxHealth *= multiplier;
                __instance.CurrentHealth = ____maxHealth;
                __instance.UpdateHealthBar();
            }
        }
    }

    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.AddRelic))]
    public static class OnRelicAdded
    {
        public static void Prefix(Relic relic, FloatVariable ____maxPlayerHealth, FloatVariable ____playerHealth)
        {
            if (relic == null) return;
            CustomRelicEffect effect = (CustomRelicEffect)relic.effect;
            if (effect == CustomRelicEffect.CURSE_TWO_HEALTH)
            {
                ____maxPlayerHealth?.Add(25f);
                ____playerHealth?.Add(25f);
            }
            else if (effect == CustomRelicEffect.CURSE_FOUR_HEALTH)
            {
                ____maxPlayerHealth?.Add(25f);
                ____playerHealth?.Add(25f);
            }
        }
    }


    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.RemoveRelic))]
    public static class OnRelicRemoved
    {
        public static void Prefix(RelicEffect re, Dictionary<RelicEffect, Relic> ____ownedRelics, FloatVariable ____maxPlayerHealth, FloatVariable ____playerHealth)
        {
            if (____ownedRelics.ContainsKey(re))
            {
                CustomRelicEffect effect = (CustomRelicEffect)re;
                if (effect == CustomRelicEffect.CURSE_TWO_HEALTH)
                {
                    ____maxPlayerHealth.Subtract(25f);
                    ____playerHealth.Subtract(25f);
                }
                else if (effect == CustomRelicEffect.CURSE_FOUR_HEALTH)
                {
                    ____maxPlayerHealth.Subtract(25f);
                    ____playerHealth.Subtract(25f);
                }
            }

        }
    }

    [HarmonyPatch(typeof(Enemy), nameof(Enemy.Attack))]
    public static class HealEnemiesOnTurnEnd
    {
        public static void Prefix(Enemy __instance, RelicManager ____relicManager)
        {
            if (CurseRelic.IsCurseLevelActive(____relicManager, 3))
                __instance.Heal((float)Math.Round(__instance.maxHealth * 0.05f));
            if (CurseRelic.IsCurseLevelActive(____relicManager, 4))
            {
                StatusEffect statusEffect = new StatusEffect(StatusEffectType.DmgBuff, 1);
                __instance.ApplyStatusEffect(statusEffect);
            }

        }
    }

    [HarmonyPatch(typeof(MapNode), nameof(MapNode.SetActiveState))]
    public static class ElitesReplaceTreasure
    {
        private static RelicManager _relicManager;
        public static void Postfix(MapNode __instance)
        {
            if (_relicManager == null) _relicManager = Resources.FindObjectsOfTypeAll<RelicManager>().FirstOrDefault();
            if (CurseRelic.IsCurseLevelActive(_relicManager, 5))
            if (__instance.RoomType == RoomType.TREASURE)
                __instance.RoomType = RoomType.MINI_BOSS;
        }
    }

    [HarmonyPatch(typeof(PlayerHealthController), nameof(PlayerHealthController.endOfBattleHealPercent), MethodType.Getter)]
    public static class HalfNavigationHealing
    {
        public static void Postfix(PlayerHealthController __instance, RelicManager ____relicManager, ref float __result)
        {
            if (CurseRelic.IsCurseLevelActive(____relicManager, 2))
                __result = (float)__result * 0.5f;
        }
    }

    [HarmonyPatch(typeof(BattleController), "CheckRelicsForStartingBombCount")]
    public static class AdditionalBombs
    {

        public static void Postfix(PegManager ____pegManager, RelicManager ____relicManager)
        {
            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_BOMB))
            {
                ____pegManager.ConvertPegsToBombs(3, false);
            }
        }
    }
}
