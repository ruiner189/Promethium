using Promethium.Patches.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Promethium.Loaders
{
    public class RelicLoader : MonoBehaviour
    {
        private static bool _relicsRegistered = false;
        public void Start()
        {
            if (!_relicsRegistered)
            {
                RegisterCustomRelics();
                RegisterDynamicRelicIcons();
            }
            StartCoroutine(DelayedStart());
        }

        public IEnumerator DelayedStart()
        {
            yield return new WaitForEndOfFrame();
            AddRelicsToPools();
        }

        private void RegisterCustomRelics()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CustomRelicBuilder.Build("holster", Plugin.Holster, CustomRelicEffect.HOLSTER, RelicPool.BOSS);
            CustomRelicBuilder.Build("mini", Plugin.MiniBelt, CustomRelicEffect.MINI, RelicPool.RARE);
            CustomRelicBuilder.Build("wumbo", Plugin.WumboBelt, CustomRelicEffect.WUMBO, RelicPool.RARE);
            CustomRelicBuilder.Build("kill_button", Plugin.KillButtonRelic, CustomRelicEffect.KILL_BUTTON, RelicPool.COMMON);

            CustomRelicBuilder.Build<PocketMoon>("reduced_gravity", Plugin.PocketMoon, CustomRelicEffect.REDUCED_GRAVITY, RelicPool.RARE);
            CustomRelicBuilder.Build<RealityMarble>("gravity_change", Plugin.RealityMarble[0], CustomRelicEffect.GRAVITY_CHANGE, RelicPool.RARE);
            CustomRelicBuilder.Build<Plasmaball>("plasma_ball", Plugin.PlasmaBall, CustomRelicEffect.PLASMA_BALL, RelicPool.RARE);
            CustomRelicBuilder.Build<Chaos>("single_item_pool", Plugin.Chaos, CustomRelicEffect.SINGLE_ITEM_POOL, RelicPool.COMMON);
            CustomRelicBuilder.Build<Order>("weighted_item_pool", Plugin.Order, CustomRelicEffect.WEIGHTED_ITEM_POOL, RelicPool.BOSS);

            CustomRelicBuilder.BuildAsCurse("curse_one_balance", Plugin.CurseOne, CustomRelicEffect.CURSE_ONE_BALANCE, 1);
            CustomRelicBuilder.BuildAsCurse("curse_one_attack", Plugin.CurseOne, CustomRelicEffect.CURSE_ONE_ATTACK, 1);
            CustomRelicBuilder.BuildAsCurse("curse_one_crit", Plugin.CurseOne, CustomRelicEffect.CURSE_ONE_CRIT, 1);

            CustomRelicBuilder.BuildAsCurse("curse_two_health", Plugin.CurseTwo, CustomRelicEffect.CURSE_TWO_HEALTH, 2);
            CustomRelicBuilder.BuildAsCurse("curse_two_armor", Plugin.CurseTwo, CustomRelicEffect.CURSE_TWO_ARMOR, 2);
            CustomRelicBuilder.BuildAsCurse("curse_two_equip", Plugin.CurseTwo, CustomRelicEffect.CURSE_TWO_EQUIP, 2);

            CustomRelicBuilder.BuildAsCurse("curse_three_bomb", Plugin.CurseThree, CustomRelicEffect.CURSE_THREE_BOMB, 3);
            CustomRelicBuilder.BuildAsCurse("curse_three_attack", Plugin.CurseThree, CustomRelicEffect.CURSE_THREE_ATTACK, 3);
            CustomRelicBuilder.BuildAsCurse("curse_three_crit", Plugin.CurseThree, CustomRelicEffect.CURSE_THREE_CRIT, 3);

            CustomRelicBuilder.BuildAsCurse("curse_four_health", Plugin.CurseFour, CustomRelicEffect.CURSE_FOUR_HEALTH, 4);
            CustomRelicBuilder.BuildAsCurse("curse_four_armor", Plugin.CurseFour, CustomRelicEffect.CURSE_FOUR_ARMOR, 4);
            CustomRelicBuilder.BuildAsCurse("curse_four_equip", Plugin.CurseFour, CustomRelicEffect.CURSE_FOUR_EQUIP, 4);

            CustomRelicBuilder.BuildAsCurse("curseFiveA", Plugin.CurseFive, CustomRelicEffect.CURSE_FIVE_A, 5);
            CustomRelicBuilder.BuildAsCurse("curseFiveB", Plugin.CurseFive, CustomRelicEffect.CURSE_FIVE_B, 5);
            CustomRelicBuilder.BuildAsCurse("curseFiveC", Plugin.CurseFive, CustomRelicEffect.CURSE_FIVE_C, 5);
            _relicsRegistered = true;

            stopwatch.Stop();
            Plugin.Log.LogInfo($"All Custom relics built! Took {stopwatch.ElapsedMilliseconds}ms");
        }

        public void RegisterDynamicRelicIcons()
        {
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_BOMB_DAMAGE, true, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_BOMB_DAMAGE2, true, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_CRIT1, false, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_CRIT2, false, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_CRIT3, false, true, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_DISCARD, true, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_END_BATTLE_HEAL, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_ORB_RELIC_OPTIONS, false, false, false, true);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_REFRESH1, false, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_REFRESH2, false, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_REFRESH3, false, false, false);
            new DynamicRelicIcon(RelicEffect.ADDITIONAL_STARTING_BOMBS, false, false, false);
            new DynamicRelicIcon(RelicEffect.ADD_ORBS_AND_UPGRADE, false, false, false);
            new DynamicRelicIcon(RelicEffect.ALL_ATTACKS_ECHO, navigating: false);
            new DynamicRelicIcon(RelicEffect.ALL_BOMBS_RIGGED, true, false, false);
            new DynamicRelicIcon(RelicEffect.ALL_IN_RELIC, navigating: false);
            new DynamicRelicIcon(RelicEffect.ALL_ORBS_BUFF, navigating: false);
            new DynamicRelicIcon(RelicEffect.ALL_ORBS_PERSIST, true, false, false);
            new DynamicRelicIcon(RelicEffect.APPLIES_HEALING_SLIME, navigating: false);
            new DynamicRelicIcon(RelicEffect.ATTACKS_DEAL_BLIND, navigating: false);
            new DynamicRelicIcon(RelicEffect.BAL_ON_RELOAD, true, false, false);
            new DynamicRelicIcon(RelicEffect.BASIC_STONE_BONUS_DAMAGE, true, false, false);
            new DynamicRelicIcon(RelicEffect.BLIND_WHEN_HIT, true, false, false);
            new DynamicRelicIcon(RelicEffect.BOMBS_APPLY_BLIND, navigating: false);
            new DynamicRelicIcon(RelicEffect.BOMBS_ONE_HIT, attacking: false, treasureNavigation: true);
            new DynamicRelicIcon(RelicEffect.BOMBS_RESPAWN, navigating: false, treasureNavigation: true);
            new DynamicRelicIcon(RelicEffect.BOMB_FORCE_ALWAYS, true, true, true, true);
            new DynamicRelicIcon(RelicEffect.BOMB_SPLASH, navigating: false, treasureNavigation: true);
            new DynamicRelicIcon(RelicEffect.BOUNCERS_COUNT, navigating: false);
            new DynamicRelicIcon(RelicEffect.CONFUSION_RELIC, navigating: false);
            new DynamicRelicIcon(RelicEffect.CRITS_STACK, navigating: false);
            new DynamicRelicIcon(RelicEffect.CRIT_ALSO_REFRESH, navigating: false);
            new DynamicRelicIcon(RelicEffect.CRIT_BONUS_DMG, true, false, false);
            new DynamicRelicIcon(RelicEffect.DAMAGE_BONUS_PLANT_FLAT, navigating: false);
            new DynamicRelicIcon(RelicEffect.DAMAGE_BONUS_SLIME_FLAT, navigating: false);
            new DynamicRelicIcon(RelicEffect.DAMAGE_ENEMIES_ON_RELOAD, true, false, false);
            new DynamicRelicIcon(RelicEffect.DAMAGE_RETURN, navigating: false);
            new DynamicRelicIcon(RelicEffect.DAMAGE_TARGETED_ON_HEAL, navigating: false);
            new DynamicRelicIcon(RelicEffect.DAMAGE_TARGETED_PEG_HITS, navigating: false);
            new DynamicRelicIcon(RelicEffect.DOUBLE_BOMBS_ON_MAP, false, false, false);
            new DynamicRelicIcon(RelicEffect.DOUBLE_DAMAGE_HURT_ON_PEG);
            new DynamicRelicIcon(RelicEffect.EVADE_CHANCE, navigating: false);
            new DynamicRelicIcon(RelicEffect.FLYING_HORIZONTAL_PIERCE, true, false, false);
            new DynamicRelicIcon(RelicEffect.FREE_RELOAD, true, true, false);
            new DynamicRelicIcon(RelicEffect.HEAL_ON_PEG_HITS, navigating: false);
            new DynamicRelicIcon(RelicEffect.HEAL_ON_REFRESH_POTION, navigating: false);
            new DynamicRelicIcon(RelicEffect.HEAL_ON_RELOAD, navigating: false);
            new DynamicRelicIcon(RelicEffect.HEDGE_BETS, navigating: false);
            new DynamicRelicIcon(RelicEffect.INCREASE_STRENGTH_SMALL, true, false, false);
            new DynamicRelicIcon(RelicEffect.INFLIGHT_DAMAGE, navigating: false);
            new DynamicRelicIcon(RelicEffect.LIFESTEAL_PEG_HITS, navigating: false);
            new DynamicRelicIcon(RelicEffect.LONGER_AIMER, true, false, true, true);
            new DynamicRelicIcon(RelicEffect.LOW_HEALTH_GUARANTEED_CRIT, true, false, false);
            new DynamicRelicIcon(RelicEffect.LOW_HEALTH_INCREASED_DAMAGE, true, false, false);
            new DynamicRelicIcon(RelicEffect.MATRYOSHKA, true, false, false);
            new DynamicRelicIcon(RelicEffect.MAX_HEALTH_LARGE, false, false, false);
            new DynamicRelicIcon(RelicEffect.MAX_HEALTH_MEDIUM, false, false, false);
            new DynamicRelicIcon(RelicEffect.MAX_HEALTH_SMALL, false, false, false);
            new DynamicRelicIcon(RelicEffect.MINIMUM_PEGS, true, false, false);
            new DynamicRelicIcon(RelicEffect.NON_CRIT_BONUS_DMG, true, false, false);
            new DynamicRelicIcon(RelicEffect.NORMAL_ATTACKS_OVERFLOW, navigating: false);
            new DynamicRelicIcon(RelicEffect.NO_DAMAGE_ON_RELOAD, true, false, false);
            new DynamicRelicIcon(RelicEffect.NO_DISCARD, true, false, false);
            new DynamicRelicIcon(RelicEffect.PEG_CLEAR_DAMAGE_SCALING, navigating: false);
            new DynamicRelicIcon(RelicEffect.PEG_MAGNET, true, true, true, true);
            new DynamicRelicIcon(RelicEffect.PEG_TO_BOMB, navigating: false);
            new DynamicRelicIcon(RelicEffect.POTION_PEGS_COUNT, navigating: false);
            new DynamicRelicIcon(RelicEffect.PREVENT_FIRST_DAMAGE, navigating: false);
            new DynamicRelicIcon(RelicEffect.REDUCE_CRIT, true, false, false);
            new DynamicRelicIcon(RelicEffect.REDUCE_LOST_HEALTH, true, true, true);
            new DynamicRelicIcon(RelicEffect.REDUCE_REFRESH, true, false, false);
            new DynamicRelicIcon(RelicEffect.REFRESH_ALSO_CRIT, navigating: false);
            new DynamicRelicIcon(RelicEffect.REFRESH_BOARD_ON_ENEMY_KILLED, navigating: false);
            new DynamicRelicIcon(RelicEffect.REFRESH_BOARD_ON_RELOAD, true, false, false);
            new DynamicRelicIcon(RelicEffect.REFRESH_DAMAGES_PEG_COUNT, navigating: false);
            new DynamicRelicIcon(RelicEffect.REFRESH_PEGS_SPLASH, true, true, false);
            new DynamicRelicIcon(RelicEffect.SHUFFLE_REFRESH_PEG, navigating: false);
            new DynamicRelicIcon(RelicEffect.SLOT_MULTIPLIERS, navigating: false);
            new DynamicRelicIcon(RelicEffect.SLOT_PORTAL, navigating: false);
            new DynamicRelicIcon(RelicEffect.SPAWN_BOMB_ON_PEG_HITS, navigating: false);
            new DynamicRelicIcon(RelicEffect.START_WITH_STR, false, false, false);
            new DynamicRelicIcon(RelicEffect.STR_ON_RELOAD, true, false, false);
            new DynamicRelicIcon(RelicEffect.SUPER_BOOTS, false, false, true, false);
            new DynamicRelicIcon(RelicEffect.UNPOPPABLE_PEGS, treasureNavigation: true);
            new DynamicRelicIcon(RelicEffect.WALL_BOUNCES_COUNT, true, true, false);

            new DynamicRelicIcon(CustomRelicEffect.HOLSTER, true, false, false);
            new DynamicRelicIcon(CustomRelicEffect.MINI, true, false, false);
            new DynamicRelicIcon(CustomRelicEffect.WUMBO, true, false, false);
            new DynamicRelicIcon(CustomRelicEffect.KILL_BUTTON, navigating: false);
            new DynamicRelicIcon(CustomRelicEffect.PLASMA_BALL, navigating: false);
            new DynamicRelicIcon(CustomRelicEffect.SINGLE_ITEM_POOL, false, false, false);
            new DynamicRelicIcon(CustomRelicEffect.WEIGHTED_ITEM_POOL, false, false, false);
            new DynamicRelicIcon(CustomRelicEffect.REDUCED_GRAVITY, true, true, true, true);
            new DynamicRelicIcon(CustomRelicEffect.GRAVITY_CHANGE, navigating: false);
        }

        private void AddRelicsToPools()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<CustomRelic> relics = CustomRelic.AllCustomRelics;

            RelicSet[] pools = Resources.FindObjectsOfTypeAll<RelicSet>();
            RelicSet commonPool = null;
            RelicSet rarePool = null;
            RelicSet bossPool = null;
            RelicSet rareScenarioPool = null;

            foreach (RelicSet pool in pools)
            {
                if (pool.name == "CommonRelics") commonPool = pool;
                else if (pool.name == "RareRelics") rarePool = pool;
                else if (pool.name == "BossRelics") bossPool = pool;
                else if (pool.name == "RareRelicsScenarioOnly") rareScenarioPool = pool;
            }

            foreach (CustomRelic relic in relics)
            {
                if (!relic.IsEnabled())
                {
                    if (rareScenarioPool != null)
                    {
                        rareScenarioPool.relics.Add(relic);
                    }
                }
                switch (relic.GetPoolType())
                {
                    case RelicPool.COMMON:
                        if (commonPool != null)
                            if (!commonPool.relics.Contains(relic))
                                commonPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE:
                        if (rarePool != null)
                            if (!rarePool.relics.Contains(relic))
                                rarePool.relics.Add(relic);
                        break;
                    case RelicPool.BOSS:
                        if (bossPool != null)
                            if (!bossPool.relics.Contains(relic))
                                bossPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE_SCENARIO:
                    case RelicPool.CURSE:
                        if (rareScenarioPool != null)
                            if (!rareScenarioPool.relics.Contains(relic))
                                rareScenarioPool.relics.Add(relic);
                        break;
                }
            }

            stopwatch.Stop();
            Plugin.Log.LogInfo($"Custom relics injected into relic pool! Took {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
