using Battle;
using Battle.Enemies;
using Battle.StatusEffects;
using HarmonyLib;
using Promethium.Extensions;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Worldmap;
using ProLib.Relics;
using Promethium.Patches.Relics.CustomRelics;

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

        public static bool IsCurseLevelActive(int curseLevel)
        {
                foreach (CurseRelic relic in AllCurseRelics)
                {
                    if (relic.CurseLevel == curseLevel)
                    {
                        if (CustomRelicManager.RelicActive(relic))
                        {
                            return true;
                        }
                    }
                }
            return false;
        }

        public static int AmountOfCurseRelics()
        {
            return AllCurseRelics.Where(relic => CustomRelicManager.RelicActive(relic)).Count();
        }

        public override int DamageModifier(Attack attack, int critCount)
        {
            if (Id == RelicNames.CURSE_ONE_ATTACK || Id == RelicNames.CURSE_THREE_ATTACK)
                return 2;
            if (Id == RelicNames.CURSE_ONE_BALANCE)
                return 1;
            return 0;
        }

        public override int CritModifier(Attack attack, int critCount)
        {
            if (Id == RelicNames.CURSE_ONE_CRIT || Id == RelicNames.CURSE_THREE_CRIT)
                return 2;
            if (Id == RelicNames.CURSE_ONE_BALANCE)
                return 1;
            return 0;
        }

        public override void OnRelicAdded(RelicManager relicManager)
        {
            if(Id == RelicNames.CURSE_TWO_HEALTH  || Id == RelicNames.CURSE_FOUR_HEALTH)
            {
                relicManager._maxPlayerHealth?.Add(25);
                relicManager._playerHealth?.Add(25);
            }
        }

        public override void OnRelicRemoved(RelicManager relicManager)
        {
            if (Id == RelicNames.CURSE_TWO_HEALTH || Id == RelicNames.CURSE_FOUR_HEALTH)
            {
                relicManager._playerHealth?.Subtract(25);
                relicManager._maxPlayerHealth?.Subtract(25);
            }
        }
    }

    [HarmonyPatch(typeof(Enemy), nameof(Enemy.Initialize))]
    public static class EnemyInit
    {
        public static void Postfix(Enemy __instance, RelicManager relicManager, ref float ____maxHealth)
        {
            if (CurseRelic.IsCurseLevelActive(1))
            {
                int amountOfCurse = CurseRelic.AmountOfCurseRelics();
                float multiplier = (float)(Plugin.TierOneHealthMultiplier * Math.Pow(Plugin.ExponentialCurseHealthMultiplier, amountOfCurse - 1));
                ____maxHealth *= multiplier;
                __instance.CurrentHealth = ____maxHealth;
                __instance.UpdateHealthBar();
            }
        }
    }

    [HarmonyPatch(typeof(Enemy), nameof(Enemy.Attack))]
    public static class HealEnemiesOnTurnEnd
    {
        public static void Prefix(Enemy __instance, RelicManager ____relicManager)
        {
            if (CurseRelic.IsCurseLevelActive(3))
                __instance.Heal((float)Math.Round(__instance.maxHealth * 0.05f));
            if (CurseRelic.IsCurseLevelActive(4))
            {
                StatusEffect statusEffect = new StatusEffect(StatusEffectType.Strength, 1);
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
            if (CurseRelic.IsCurseLevelActive(5))
                if (__instance.RoomType == RoomType.TREASURE)
                    __instance.RoomType = RoomType.MINI_BOSS;
        }
    }

    [HarmonyPatch(typeof(PlayerHealthController), nameof(PlayerHealthController.endOfBattleHealPercent), MethodType.Getter)]
    public static class HalfNavigationHealing
    {
        public static void Postfix(PlayerHealthController __instance, RelicManager ____relicManager, ref float __result)
        {
            if (CurseRelic.IsCurseLevelActive(2))
                __result = (float)__result * 0.5f;
        }
    }

    [HarmonyPatch(typeof(BattleController), "CheckRelicsForStartingBombCount")]
    public static class AdditionalBombs
    {

        public static void Postfix(PegManager ____pegManager)
        {
            if (CustomRelicManager.RelicActive(RelicNames.CURSE_THREE_BOMB))
            {
                ____pegManager.ConvertPegsToBombs(3, false);

            }
        }
    }
}
