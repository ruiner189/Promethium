using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ProLib.Utility;
using Promethium.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TMPro;
using UI;
using UnityEngine;

namespace Promethium
{
    [BepInPlugin(GUID, Name, Version)]

    [BepInDependency("com.ruiner.prolib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("me.bo0tzz.peglinmods.endless", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ruiner.customchallenges", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {

        public const String GUID = "com.ruiner.promethium";
        public const String Name = "Promethium";
        public const String Version = "1.4.1";

        private Harmony _harmony;
        public static ManualLogSource Log;
        public static ConfigFile ConfigFile;

        public static GameObject PromethiumManager;
        public static GameObject PromethiumPrefabHolder;

        // Sprites
        public static Sprite ArmorEffect;
        public static Sprite Circle;
        public static Sprite CurseOne;
        public static Sprite CurseTwo;
        public static Sprite CurseThree;
        public static Sprite CurseFour;
        public static Sprite CurseFive;
        public static Sprite Holster;
        public static Sprite WumboBelt;
        public static Sprite MiniBelt;
        public static Sprite KillButtonRelic;
        public static Sprite KillButton;
        public static Sprite PlasmaBall;
        public static Sprite OrbOfGreed;
        public static Sprite OrbOfGreedAttack;
        public static Sprite Orbgis;
        public static Sprite OrbgisAttack;
        public static Sprite[] Lasorb;
        public static Sprite[] LasorbAttack;
        public static Sprite Berserkorb;
        public static Sprite BerserkorbAttack;
        public static Sprite Order;
        public static Sprite Chaos;
        public static Sprite PocketMoon;
        public static Sprite DogRelicSprite;
        public static Sprite[] RealityMarble;
        public static Sprite Capsule;
        public static Sprite Anvil;

        // Config
        private static ConfigEntry<bool> EnemyAttackOnShuffleConfig;
        private static ConfigEntry<bool> CurseRunOnConfig;
        private static ConfigEntry<bool> PruneRelicsOnNewCurseRunConfig;
        private static ConfigEntry<bool> PruneOrbsOnNewCurseRunConfig;
        private static ConfigEntry<float> TierOneCurseHealth;
        private static ConfigEntry<float> ExponentialCurseHealth;
        private static ConfigEntry<bool> SpeedUpOnConfig;
        private static ConfigEntry<float> SpeedUpDelayConfig;
        private static ConfigEntry<float> SpeedUpMaxConfig;
        private static ConfigEntry<float> SpeedUpRateConfig;
        private static ConfigEntry<bool> DynamicIconActiveConfig;
        private static ConfigEntry<int> DynamicIconMinimumConfig;
        private static ConfigEntry<bool> UseCustomPredictionConfig;
        private static ConfigEntry<bool> HoldDiscardConfig;

        // Soft Dependencies
        public static bool EndlessPeglinPlugin = false;
        public static bool CustomChallengesPlugin = false;

        public static bool EnemyAttackOnReload => EnemyAttackOnShuffleConfig.Value;
        public static bool CurseRunOn => CurseRunOnConfig.Value && !EndlessPeglinPlugin;
        public static bool PruneRelicsOnNewCurseRunOn => PruneRelicsOnNewCurseRunConfig.Value;
        public static bool PruneOrbsOnNewCurseRunOn => PruneOrbsOnNewCurseRunConfig.Value;
        public static float TierOneHealthMultiplier => TierOneCurseHealth.Value;
        public static float ExponentialCurseHealthMultiplier => ExponentialCurseHealth.Value;

        public static bool SpeedUpOn => SpeedUpOnConfig.Value;
        public static float SpeedUpDelay => SpeedUpDelayConfig.Value;
        public static float SpeedUpMax => SpeedUpMaxConfig.Value;
        public static float SpeedUpRate => SpeedUpRateConfig.Value;

        public static bool DynamicIconActive => DynamicIconActiveConfig.Value;
        public static int DynamicIconMinimum => DynamicIconMinimumConfig.Value;
        public static bool UseCustomPrediction => UseCustomPredictionConfig.Value;
        public static bool HoldDiscard => HoldDiscardConfig.Value;

        private void Awake()
        {
            Log = Logger;
            ConfigFile = Config;

            LoadSprites();

            EnemyAttackOnShuffleConfig = Config.Bind<bool>("Mechanics", "EnemyAttackOnShuffle", true, "Disabling this will prevent enemies from taking two turns in certain circumstances");
            SpeedUpOnConfig = Config.Bind<bool>("Mechanics", "SpeedUpOn", false, "Speeds up game after a set amount of time");
            SpeedUpDelayConfig = Config.Bind<float>("Mechanics", "SpeedUpDelay", 10, "Delay for speed up. In seconds");
            SpeedUpMaxConfig = Config.Bind<float>("Mechanics", "SpeedUpMax", 3, "How much it speeds up the game at max value");
            SpeedUpRateConfig = Config.Bind<float>("Mechanics", "SpeedUpRate", 1, "How fast the mod transitions the speed-up. Higher values means the game will speed up faster");
            HoldDiscardConfig = Config.Bind<bool>("Mechanics", "HoldToDiscard", false, "Hold to discard instead of pressing. Will auto-enable if you have the relic Holster");

            UseCustomPredictionConfig = Config.Bind<bool>("Mechanics", "UseCustomPrediction", true, "Use Promethium's Custom Prediction. Setting this to false will use the default prediction system.");

            DynamicIconActiveConfig = Config.Bind<bool>("Dynamic Relic Icon", "DynamicIconActive", true, "Relic icons are hidden under certain conditions. This is to reduce screen clutter when you have a lot of relics.");
            DynamicIconMinimumConfig = Config.Bind<int>("Dynamic Relic Icon", "DynamicIconMinimum", 16, "How many relics you need before they start hiding under certain conditions");

            CurseRunOnConfig = Config.Bind<bool>("Curse Run", "CurseRunOn", true, "Finish a game to increase your curse level. How far can you go?");
            PruneRelicsOnNewCurseRunConfig = Config.Bind<bool>("Curse Run", "PruneRelicOnCurseRun", true, "Reduces the amount of relics when starting a new curse run. Disabling lets you keep all relics.");
            PruneOrbsOnNewCurseRunConfig = Config.Bind<bool>("Curse Run", "PruneOrbsOnCurseRun", true, "Reduces the amount of orbs to four when starting a new curse run. Disabling lets you keep all orbs.");
            TierOneCurseHealth = Config.Bind<float>("Curse Run", "TierOneHealthMultiplier", 3f, "Amount of health to multiply for tier 1 of chaos relics");
            ExponentialCurseHealth = Config.Bind<float>("Curse Run", "ExponentialCurseHealth", 2f, "Amount of health to multiple after tier 1. This is exponential");

            _harmony = new Harmony(GUID);
            _harmony.PatchAll();
            LoadSoftDependencies();

            PromethiumManager = new GameObject("Promethium Mod");
            PromethiumManager.AddComponent<RestartButtonActivator>();
            PromethiumManager.AddComponent<ArmorManager>();
            DontDestroyOnLoad(PromethiumManager);
            PromethiumManager.hideFlags = HideFlags.HideAndDontSave;

            PromethiumPrefabHolder = new GameObject("Promethium Prefab Holder");
            DontDestroyOnLoad(PromethiumPrefabHolder);
            PromethiumPrefabHolder.hideFlags = HideFlags.HideAndDontSave;
            PromethiumPrefabHolder.SetActive(false);
        
        }


        private void LoadSoftDependencies()
        {
            // Check Dependencies
            EndlessPeglinPlugin = Chainloader.PluginInfos.TryGetValue("me.bo0tzz.peglinmods.endless", out _);
            CustomChallengesPlugin = Chainloader.PluginInfos.TryGetValue("com.ruiner.customchallenges", out _);

            // Messages about incompatability
            if (EndlessPeglinPlugin && CurseRunOnConfig.Value)
            {
                Log.LogWarning("Endless Peglin Mod detected! Automatically turning off curse runs.");
            }
        }

        private void LoadSprites()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Assembly assembly = Assembly.GetExecutingAssembly();

            ArmorEffect = assembly.LoadSprite("Resources.ArmorEffect.png");
            Circle = assembly.LoadSprite("Resources.Circle.png");
            Holster = assembly.LoadSprite("Resources.Relics.Holster.png");
            WumboBelt = assembly.LoadSprite("Resources.Relics.WumboBelt.png");
            MiniBelt = assembly.LoadSprite("Resources.Relics.MiniBelt.png");
            PlasmaBall = assembly.LoadSprite("Resources.Relics.Plasmaball.png");
            Order = assembly.LoadSprite("Resources.Relics.Order.png");
            Chaos = assembly.LoadSprite("Resources.Relics.Chaos.png");
            PocketMoon = assembly.LoadSprite("Resources.Relics.PocketMoon.png");
            Capsule = assembly.LoadSprite("Resources.Relics.GachaBall.png");
            Anvil = assembly.LoadSprite("Resources.Relics.Anvil.png");


            DogRelicSprite = assembly.LoadSprite("Resources.Relics.Dog_1.png");

            CurseOne = assembly.LoadSprite("Resources.Relics.Curse_One.png");
            CurseTwo = assembly.LoadSprite("Resources.Relics.Curse_Two.png");
            CurseThree = assembly.LoadSprite("Resources.Relics.Curse_Three.png");
            CurseFour = assembly.LoadSprite("Resources.Relics.Curse_Four.png");
            CurseFive = assembly.LoadSprite("Resources.Relics.Curse_Five.png");

            KillButtonRelic = assembly.LoadSprite("Resources.Relics.KillButton.png");
            KillButton = assembly.LoadSprite("Resources.KillButton.png");

            OrbOfGreed = assembly.LoadSprite("Resources.Orbs.OrbOfGreed.png", 8);
            OrbOfGreedAttack = assembly.LoadSprite("Resources.Orbs.OrbOfGreed.png", 16);

            Orbgis = assembly.LoadSprite("Resources.Orbs.Orbgis.png", 8);
            OrbgisAttack = assembly.LoadSprite("Resources.Orbs.Orbgis.png", 16);

            Lasorb = LoadMultipleSprites("Resources.Orbs.Lasorb", 9, pixelsPerUnit: 8);
            LasorbAttack = LoadMultipleSprites("Resources.Orbs.LaserAttack.LaserAttack", 15);

            Berserkorb = assembly.LoadSprite("Resources.Orbs.Berserkorb.png", 8);
            BerserkorbAttack = assembly.LoadSprite("Resources.Orbs.Berserkorb.png", 16);

            RealityMarble = LoadMultipleSprites("Resources.Relics.RealityMarble", 5);    

            stopwatch.Stop();
            Log.LogInfo($"Sprites loaded! Took {stopwatch.ElapsedMilliseconds}ms");
        }

        public static Sprite[] LoadMultipleSprites(string filePath,  int amount, string extension = ".png", float pixelsPerUnit = 16f)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Sprite[] sprites = new Sprite[amount];
            for (int i = 0; i < amount; i++)
            {
                String path = $"{filePath}_{i}{extension}";
                sprites[i] = assembly.LoadSprite(path, pixelsPerUnit);
            }
            return sprites;
        }
    }
}

