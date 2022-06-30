using HarmonyLib;
using Promethium.Extensions;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Relics.CustomRelics
{
    public sealed class Order : CustomRelic
    {
        public const String BOMB = "BOMB";
        public const String BOMB_DAMAGE = "BOMB_DAMAGE";
        public const String BOMB_AMOUNT = "BOMB_AMOUNT";
        public const String CRIT = "CRIT";
        public const String MODIFY_BOARD = "MODIFY_BOARD";
        public const String DISCARD = "DISCARD";
        public const String AFFECTS_ORB = "AFFECTS_ORB";
        public const String AFFECTS_PEG = "AFFECTS_PEG";
        public const String AFFECTS_ENEMY = "AFFECTS_ENEMY";
        public const String AFFECTS_PHYSICS = "AFFECTS_PHYSICS";
        public const String DEBUFF = "DEBUFF";
        public const String HEAL = "HEAL";
        public const String AIM = "AIM";
        public const String REWARDS = "REWARDS";
        public const String REFRESH = "REFRESH";
        public const String RANDOM = "RANDOM";
        public const String ORB_DAMAGE = "ORB_DAMAGE";
        public const String SELF_DAMAGE = "SELF_DAMAGE";
        public const String RELOAD = "RELOAD";
        public const String SPLASH = "SPLASH";
        public const String ARMOR = "ARMOR";
        public const String FLAT_DAMAGE = "FLAT_DAMAGE";
        public const String ON_HIT = "ON_HIT";
        public const String NEGATE_DAMAGE = "NEGATE_DAMAGE";
        public const String BUFF = "BUFF";
        public const String HEALTH = "HEALTH";
        public const String NAVIGATION = "NAVIGATION";

        public Order()
        {
            SetCategories();
        }

        private void SetCategories()
        {
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_BOMB_DAMAGE, BOMB, BOMB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_BOMB_DAMAGE2, BOMB, BOMB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_CRIT1, CRIT, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_CRIT2, CRIT, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_CRIT3, CRIT, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_DISCARD, DISCARD);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_END_BATTLE_HEAL, HEAL);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_ORB_RELIC_OPTIONS, REWARDS);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_REFRESH1, REFRESH, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_REFRESH2, REFRESH, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_REFRESH3, REFRESH, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.ADDITIONAL_STARTING_BOMBS, BOMB, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.ADD_ORBS_AND_UPGRADE, REWARDS, RANDOM);
            RelicCategory.AddCategories(RelicEffect.ALL_ATTACKS_ECHO, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.ALL_BOMBS_RIGGED, BOMB, BOMB_DAMAGE, SELF_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.ALL_IN_RELIC, CRIT, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.ALL_ORBS_BUFF, ORB_DAMAGE, AFFECTS_ORB);
            RelicCategory.AddCategories(RelicEffect.ALL_ORBS_PERSIST, AFFECTS_ORB);
            RelicCategory.AddCategories(RelicEffect.ALTERNATE_SHOT_POWER, AFFECTS_ORB);
            RelicCategory.AddCategories(RelicEffect.APPLIES_HEALING_SLIME, AFFECTS_PEG, HEAL, ON_HIT);
            RelicCategory.AddCategories(RelicEffect.ATTACKS_DEAL_BLIND, AFFECTS_ORB, DEBUFF, NEGATE_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.BAL_ON_RELOAD, ORB_DAMAGE, RELOAD);
            RelicCategory.AddCategories(RelicEffect.BASIC_STONE_BONUS_DAMAGE, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.BLIND_WHEN_HIT, AFFECTS_ENEMY, DEBUFF);
            RelicCategory.AddCategories(RelicEffect.BOMBS_APPLY_BLIND, BOMB, DEBUFF, NEGATE_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.BOMBS_ONE_HIT, BOMB, AFFECTS_PEG, ON_HIT);
            RelicCategory.AddCategories(RelicEffect.BOMBS_RESPAWN, BOMB, REFRESH, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.BOMB_FORCE_ALWAYS, AFFECTS_PEG);
            RelicCategory.AddCategories(RelicEffect.BOMB_SPLASH, AFFECTS_PEG, BOMB, SPLASH);
            RelicCategory.AddCategories(RelicEffect.BOUNCERS_COUNT, ORB_DAMAGE, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.CONFUSION_RELIC, DEBUFF, AFFECTS_ORB, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.CRITS_STACK, CRIT, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.CRIT_ALSO_REFRESH, CRIT, REFRESH);
            RelicCategory.AddCategories(RelicEffect.CRIT_BONUS_DMG, CRIT, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.CRIT_PIT, CRIT, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.DAMAGE_BONUS_PLANT_FLAT, ARMOR, FLAT_DAMAGE, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.DAMAGE_BONUS_SLIME_FLAT, FLAT_DAMAGE, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.DAMAGE_ENEMIES_ON_RELOAD, RELOAD);
            RelicCategory.AddCategories(RelicEffect.DAMAGE_RETURN, AFFECTS_ENEMY);
            RelicCategory.AddCategories(RelicEffect.DAMAGE_TARGETED_ON_HEAL, HEAL);
            RelicCategory.AddCategories(RelicEffect.DAMAGE_TARGETED_PEG_HITS, AFFECTS_PEG, ON_HIT);
            RelicCategory.AddCategories(RelicEffect.DOUBLE_BOMBS_ON_MAP, BOMB, BOMB_AMOUNT, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG, SELF_DAMAGE, ORB_DAMAGE, ON_HIT);
            RelicCategory.AddCategories(RelicEffect.EVADE_CHANCE, AFFECTS_ENEMY, DEBUFF, NEGATE_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.FLYING_HORIZONTAL_PIERCE, AFFECTS_ORB);
            RelicCategory.AddCategories(RelicEffect.FREE_RELOAD, RELOAD, NEGATE_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.HEAL_ON_PEG_HITS, HEAL, ON_HIT);
            RelicCategory.AddCategories(RelicEffect.HEAL_ON_REFRESH_POTION, HEAL, REFRESH);
            RelicCategory.AddCategories(RelicEffect.HEAL_ON_RELOAD, HEAL, RELOAD);
            RelicCategory.AddCategories(RelicEffect.HEDGE_BETS, ORB_DAMAGE, CRIT);
            RelicCategory.AddCategories(RelicEffect.INCREASE_STRENGTH_SMALL, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.INFLIGHT_DAMAGE, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.LEGACY_METEORITE, AFFECTS_PEG, ON_HIT);
            RelicCategory.AddCategories(RelicEffect.LIFESTEAL_PEG_HITS, HEAL, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.LONGER_AIMER, AIM);
            RelicCategory.AddCategories(RelicEffect.LOW_HEALTH_GUARANTEED_CRIT, CRIT, HEALTH);
            RelicCategory.AddCategories(RelicEffect.LOW_HEALTH_INCREASED_DAMAGE, ORB_DAMAGE, HEALTH);
            RelicCategory.AddCategories(RelicEffect.MATRYOSHKA, AFFECTS_ORB, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.MAX_HEALTH_LARGE, HEALTH);
            RelicCategory.AddCategories(RelicEffect.MAX_HEALTH_MEDIUM, HEALTH);
            RelicCategory.AddCategories(RelicEffect.MAX_HEALTH_SMALL, HEALTH);
            RelicCategory.AddCategories(RelicEffect.NON_CRIT_BONUS_DMG, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.NORMAL_ATTACKS_OVERFLOW, AFFECTS_ORB);
            RelicCategory.AddCategories(RelicEffect.NO_DAMAGE_ON_RELOAD, NEGATE_DAMAGE, RELOAD);
            RelicCategory.AddCategories(RelicEffect.NO_DISCARD, DISCARD, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.PEG_CLEAR_DAMAGE_SCALING);
            RelicCategory.AddCategories(RelicEffect.PEG_MAGNET, AFFECTS_ORB, AFFECTS_PHYSICS, AFFECTS_PEG, CRIT, REFRESH, BOMB);
            RelicCategory.AddCategories(RelicEffect.PEG_TO_BOMB, AFFECTS_PEG, BOMB, BOMB_AMOUNT);
            RelicCategory.AddCategories(RelicEffect.SLOT_PORTAL, MODIFY_BOARD, AFFECTS_ORB, RANDOM);
            RelicCategory.AddCategories(RelicEffect.POTION_PEGS_COUNT, CRIT, REFRESH);
            RelicCategory.AddCategories(RelicEffect.PREVENT_FIRST_DAMAGE, NEGATE_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.RANDOM_ENEMY_HEALTH, AFFECTS_ENEMY, HEALTH);
            RelicCategory.AddCategories(RelicEffect.REDUCE_CRIT, CRIT, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.REDUCE_LOST_HEALTH, HEALTH, NEGATE_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.REDUCE_REFRESH, REFRESH, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.REFRESH_ALSO_CRIT, REFRESH, CRIT);
            RelicCategory.AddCategories(RelicEffect.REFRESH_BOARD_ON_ENEMY_KILLED, REFRESH);
            RelicCategory.AddCategories(RelicEffect.REFRESH_BOARD_ON_RELOAD, REFRESH, RELOAD);
            RelicCategory.AddCategories(RelicEffect.REFRESH_DAMAGES_PEG_COUNT, REFRESH);
            RelicCategory.AddCategories(RelicEffect.REFRESH_PEGS_SPLASH, REFRESH, SPLASH, AFFECTS_PEG);
            RelicCategory.AddCategories(RelicEffect.SHUFFLE_REFRESH_PEG, REFRESH, MODIFY_BOARD);
            RelicCategory.AddCategories(RelicEffect.SLOT_MULTIPLIERS, ORB_DAMAGE, RANDOM);
            RelicCategory.AddCategories(RelicEffect.SPAWN_BOMB_ON_PEG_HITS, BOMB, BOMB_DAMAGE, MODIFY_BOARD, ON_HIT);
            RelicCategory.AddCategories(RelicEffect.START_WITH_STR, BUFF, ORB_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.STR_ON_RELOAD, BUFF, ORB_DAMAGE, RELOAD);
            RelicCategory.AddCategories(RelicEffect.SUPER_BOOTS, NAVIGATION, HEAL, NEGATE_DAMAGE);
            RelicCategory.AddCategories(RelicEffect.UNPOPPABLE_PEGS, AFFECTS_PEG);
            RelicCategory.AddCategories(RelicEffect.WALL_BOUNCES_COUNT, ORB_DAMAGE, MODIFY_BOARD);

            RelicCategory.AddCategories(CustomRelicEffect.HOLSTER, DISCARD, AFFECTS_ORB);
            RelicCategory.AddCategories(CustomRelicEffect.MINI, AFFECTS_ORB, AFFECTS_PHYSICS);
            RelicCategory.AddCategories(CustomRelicEffect.WUMBO, AFFECTS_ORB, AFFECTS_PHYSICS);
            RelicCategory.AddCategories(CustomRelicEffect.KILL_BUTTON, AFFECTS_ORB, RELOAD, BOMB);
            RelicCategory.AddCategories(CustomRelicEffect.PLASMA_BALL, ORB_DAMAGE, AFFECTS_ORB);
            RelicCategory.AddCategories(CustomRelicEffect.REDUCED_GRAVITY, AFFECTS_PHYSICS, MODIFY_BOARD);
            RelicCategory.AddCategories(CustomRelicEffect.GRAVITY_CHANGE, AFFECTS_PHYSICS, MODIFY_BOARD, RANDOM);
        }
        
        [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetCommonRelic))]
        public static class RigCommonRelics
        {
            public static bool Prefix(RelicManager __instance, bool fallback, ref Relic __result)
            {
                if (__instance.RelicEffectActive(CustomRelicEffect.WEIGHTED_ITEM_POOL))
                {
                    __instance.CheckForDebugReset(__instance._availableCommonRelics);
                    if (__instance._availableCommonRelics.Count > 0)
                    {
                        __result = RelicCategory.GetRandomRelicWithWeights(__instance._availableCommonRelics);
                    }
                    if (fallback && __result == null)
                    {
                        return __instance.GetRareRelic(false);
                    }
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetRareRelic))]
        public static class RigRareRelics
        {
            public static bool Prefix(RelicManager __instance, bool fallback, ref Relic __result)
            {
                if (__instance.RelicEffectActive(CustomRelicEffect.WEIGHTED_ITEM_POOL))
                {
                    __instance.CheckForDebugReset(__instance._availableRareRelics);
                    if (__instance._availableRareRelics.Count > 0)
                    {
                        __result = RelicCategory.GetRandomRelicWithWeights(__instance._availableRareRelics);
                    }
                    if (fallback && __result == null)
                    {
                        return __instance.GetCommonRelic(false);
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetBossRelic))]
        public static class RigBossRelics
        {
            public static bool Prefix(RelicManager __instance, bool fallback, ref Relic __result)
            {
                if (__instance.RelicEffectActive(CustomRelicEffect.WEIGHTED_ITEM_POOL))
                {
                    __instance.CheckForDebugReset(__instance._availableBossRelics);
                    if (__instance._availableBossRelics.Count > 0)
                    {
                        __result = RelicCategory.GetRandomRelicWithWeights(__instance._availableBossRelics);
                    }
                    if (fallback && __result == null)
                    {
                        return __instance.GetRareRelic(true);
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
