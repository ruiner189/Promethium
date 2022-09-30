using ProLib.Attributes;
using ProLib.Loaders;
using ProLib.Relics;
using ProLib.Utility;
using Promethium.Components;
using Promethium.Patches.Orbs.CustomOrbs;
using Promethium.Patches.Orbs.ModifiedOrbs;
using Promethium.Patches.Relics;
using Promethium.Patches.Relics.CustomRelicIcon;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Promethium
{
    public static class Register
    {
        private static bool _registered = false;

        [Register]
        public static void SetupRegisters()
        {
            if (!_registered)
            {
                _registered = true;

                RelicLoader.Register += new RelicLoader.RelicRegister(RegisterRelics);
                OrbLoader.Register += new OrbLoader.OrbRegister(RegisterOrbs);
                LanguageLoader.RegisterLocalization += new LanguageLoader.LocalizationRegistration(RegisterLocalization);
            }
        }

        private static void RegisterRelics(RelicLoader relicLoader)
        {
            RegisterCustomRelics();
            RegisterModifiedRelics();
            RegisterDynamicRelicIcons();
        }

        private static void RegisterOrbs(OrbLoader orbLoader)
        {
            RegisterModifiedOrbs();
            RegisterCustomOrbs(orbLoader);
        }

        private static void RegisterLocalization(LanguageLoader languageLoader)
        {
            languageLoader.LoadGoogleSheetTSVSource("https://docs.google.com/spreadsheets/d/e/2PACX-1vRe82XVSt8LOUz3XewvAHT5eDDzAqXr5MV0lt3gwvfN_2n9Zxj613jllVPtdPdQweAap2yOSJSgwpPt/pub?gid=0&single=true&output=tsv", "Promethium_Translation.tsv");
            languageLoader.AddDynamicLocalizationParam(GetParameterValue);
        }

        public static string GetParameterValue(string Param)
        {
            if (Param == ParamKeys.ADDITIONAL_LIGHTNING_ZAPS)
                return $"{Plasma.AdditionalZaps}";
            else if (Param == ParamKeys.PLASMA_PEG_HITS)
                return $"{Plasma.PegsToHit}";
            else if (Param == ParamKeys.MULTIBALL_RELIC_MULTIPLIER)
                return $"<style=dmg_negative>{(1 - ModifiedRelic.MATRYOSHKA_SHELL_MULTIPLIER) * 100}%</style>";
            else if (Param == ParamKeys.NO_DISCARD_MULTIPLIER)
                return $"<style=dmg_bonus>{ModifiedRelic.NO_DISCARD_RELIC_MULTIPLIER * 100}%</style>";
            else if (Param == ParamKeys.NO_DISCARD_MULTIPLIER_VALUE)
            {
                float multiplier = ModifiedRelic.CalculateNoDiscardMultiplier() - 1;
                if (multiplier == 0) return "";
                return $"<style=dmg_bonus>({multiplier * 100}%)</style>";
            }
            else if (Param == ParamKeys.GRAVITY_CHANGE_TIME)
                return $"{RealityMarble.GRAVITY_CHANGE_SECONDS}";
            else if (Param == ParamKeys.GRAVITY_REDUCTION)
                return $"{(1 - PocketMoon.GRAVITY_REDUCTION) * 100}%";
            return null;
        }

        private static void RegisterCustomOrbs(OrbLoader orbLoader)
        {
            if (Oreb.GetInstance().IsEnabled())
                orbLoader.AddOrbToPool(Oreb.GetInstance().GetPrefab(1));
            OrbofGreed.GetInstance();
        }

        private static void RegisterModifiedOrbs()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ModifiedBouldorb.Register();
            ModifiedOrbelisk.Register();
            ModifiedStone.Register();
            ModifiedDoctorb.Register();
            ModifiedNosforbatu.Register();
            ModifiedRefreshOrb.Register();
            ModifiedShuffleOrb.Register();
            ModifiedMatryoshka.Register();
            ModifiedLightningBall.Register();
            stopwatch.Stop();
            Plugin.Log.LogInfo($"Vanilla orbs modified! Took {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void RegisterModifiedRelics()
        {
            ModifiedRelic.AddRelic(RelicEffect.DAMAGE_BONUS_PLANT_FLAT); // Gardening Gloves
            ModifiedRelic.AddRelic(RelicEffect.NO_DISCARD);
            ModifiedRelic.AddRelic(RelicEffect.MATRYOSHKA);
        }

        private static void RegisterCustomRelics()
        {
            new CustomRelicBuilder()
                .SetName(RelicNames.HOLSTER)
                .SetAlternativeDescription("2")
                .SetEnabled(IsRelicEnabled(RelicNames.HOLSTER))
                .SetSprite(Plugin.Holster)
                .SetRarity(RelicRarity.BOSS)
                .AlwaysUnlocked(true)
                .Build();

            new CustomRelicBuilder()
                .SetName(RelicNames.MINI)
                .SetEnabled(IsRelicEnabled(RelicNames.MINI))
                .SetSprite(Plugin.MiniBelt)
                .SetRarity(RelicRarity.RARE)
                .AlwaysUnlocked(true)
                .Build();

            new CustomRelicBuilder()
                .SetName(RelicNames.WUMBO)
                .SetEnabled(IsRelicEnabled(RelicNames.WUMBO))
                .SetSprite(Plugin.WumboBelt)
                .SetRarity(RelicRarity.RARE)
                .AlwaysUnlocked(true)
                .Build();

            new CustomRelicBuilder()
                .SetName(RelicNames.KILL_BUTTON)
                .SetEnabled(IsRelicEnabled(RelicNames.KILL_BUTTON))
                .SetSprite(Plugin.KillButtonRelic)
                .SetRarity(RelicRarity.COMMON)
                .AlwaysUnlocked(true)
                .Build();

            new CustomRelicBuilder()
                .SetName(RelicNames.REDUCED_GRAVITY)
                .SetEnabled(IsRelicEnabled(RelicNames.REDUCED_GRAVITY))
                .SetSprite(Plugin.PocketMoon)
                .SetRarity(RelicRarity.RARE)
                .AlwaysUnlocked(true)
                .Build<PocketMoon>();

            new CustomRelicBuilder()
                .SetName(RelicNames.GRAVITY_CHANGE)
                .SetEnabled(IsRelicEnabled(RelicNames.GRAVITY_CHANGE))
                .SetSprite(Plugin.RealityMarble[0])
                .SetRelicIcon(typeof(RealityMarbleIcon))
                .SetRarity(RelicRarity.RARE)
                .AlwaysUnlocked(true)
                .Build<RealityMarble>();

            new CustomRelicBuilder()
                .SetName(RelicNames.PLASMA_BALL)
                .SetEnabled(IsRelicEnabled(RelicNames.PLASMA_BALL))
                .SetSprite(Plugin.PlasmaBall)
                .SetRarity(RelicRarity.RARE)
                .AlwaysUnlocked(true)
                .Build<Plasmaball>();

            new CustomRelicBuilder()
                .SetName(RelicNames.SINGLE_ITEM_POOL)
                .SetEnabled(IsRelicEnabled(RelicNames.SINGLE_ITEM_POOL))
                .SetSprite(Plugin.Chaos)
                .SetRarity(RelicRarity.COMMON)
                .AlwaysUnlocked(true)
                .Build<Chaos>();

            new CustomRelicBuilder()
                .SetName(RelicNames.WEIGHTED_ITEM_POOL)
                .SetEnabled(IsRelicEnabled(RelicNames.WEIGHTED_ITEM_POOL))
                .SetSprite(Plugin.Order)
                .SetRarity(RelicRarity.BOSS)
                .AlwaysUnlocked(true)
                .Build<Order>();

            foreach (String name in new String[] { RelicNames.CURSE_ONE_ATTACK, RelicNames.CURSE_ONE_CRIT, RelicNames.CURSE_ONE_BALANCE })
                new CustomRelicBuilder()
                    .SetName(name)
                    .SetSprite(Plugin.CurseOne)
                    .SetRarity(RelicRarity.UNAVAILABLE)
                    .IncludeInCustomLoadout(false)
                    .Build<CurseRelic>()
                    .CurseLevel = 1;

            foreach (String name in new String[] { RelicNames.CURSE_TWO_HEALTH, RelicNames.CURSE_TWO_ARMOR, RelicNames.CURSE_TWO_EQUIP })
                new CustomRelicBuilder()
                    .SetName(name)
                    .SetSprite(Plugin.CurseTwo)
                    .SetRarity(RelicRarity.UNAVAILABLE)
                    .IncludeInCustomLoadout(false)
                    .Build<CurseRelic>()
                    .CurseLevel = 2;

            foreach (String name in new String[] { RelicNames.CURSE_THREE_BOMB, RelicNames.CURSE_THREE_ATTACK, RelicNames.CURSE_THREE_CRIT })
                new CustomRelicBuilder()
                    .SetName(name)
                    .SetSprite(Plugin.CurseThree)
                    .SetRarity(RelicRarity.UNAVAILABLE)
                    .IncludeInCustomLoadout(false)
                    .Build<CurseRelic>()
                    .CurseLevel = 3;


            foreach (String name in new String[] { RelicNames.CURSE_FOUR_HEALTH, RelicNames.CURSE_FOUR_ARMOR, RelicNames.CURSE_FOUR_EQUIP })
                new CustomRelicBuilder()
                    .SetName(name)
                    .SetSprite(Plugin.CurseFour)
                    .SetRarity(RelicRarity.UNAVAILABLE)
                    .IncludeInCustomLoadout(false)
                    .Build<CurseRelic>()
                    .CurseLevel = 4;

            foreach (String name in new String[] { RelicNames.CURSE_FIVE_A, RelicNames.CURSE_FIVE_B, RelicNames.CURSE_FIVE_C })
                new CustomRelicBuilder()
                    .SetName(name)
                    .SetSprite(Plugin.CurseFive)
                    .SetRarity(RelicRarity.UNAVAILABLE)
                    .IncludeInCustomLoadout(false)
                    .Build<CurseRelic>()
                    .CurseLevel = 5;
        }

        private static bool IsRelicEnabled(String relicName)
        {
            return Plugin.ConfigFile.Bind<bool>("Custom Relic", relicName, true, "Add relic to relic pools.").Value;
        }

        private static void RegisterDynamicRelicIcons()
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

            new DynamicRelicIcon(RelicNames.HOLSTER, true, false, false);
            new DynamicRelicIcon(RelicNames.MINI, true, false, false);
            new DynamicRelicIcon(RelicNames.WUMBO, true, false, false);
            new DynamicRelicIcon(RelicNames.KILL_BUTTON, navigating: false);
            new DynamicRelicIcon(RelicNames.PLASMA_BALL, navigating: false);
            new DynamicRelicIcon(RelicNames.SINGLE_ITEM_POOL, false, false, false);
            new DynamicRelicIcon(RelicNames.WEIGHTED_ITEM_POOL, false, false, false);
            new DynamicRelicIcon(RelicNames.REDUCED_GRAVITY, true, true, true, true);
            new DynamicRelicIcon(RelicNames.GRAVITY_CHANGE, navigating: false);
        }
    }
}
