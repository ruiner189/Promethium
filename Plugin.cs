using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
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
        public const String Version = "1.3.3";

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
        public static Sprite Order;
        public static Sprite Chaos;
        public static Sprite PocketMoon;
        public static Sprite DogRelicSprite;
        public static Sprite[] RealityMarble;

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
        public static bool CurseRunOn =>  CurseRunOnConfig.Value && !EndlessPeglinPlugin;
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

            ArmorEffect = LoadSprite("ArmorEffect.png");
            Circle = LoadSprite("Circle.png");
            Holster = LoadSprite("Relics.Holster.png");
            WumboBelt = LoadSprite("Relics.WumboBelt.png");
            MiniBelt = LoadSprite("Relics.MiniBelt.png");
            PlasmaBall = LoadSprite("Relics.Plasmaball.png");
            Order = LoadSprite("Relics.Order.png");
            Chaos = LoadSprite("Relics.Chaos.png");
            PocketMoon = LoadSprite("Relics.PocketMoon.png");
            DogRelicSprite = LoadSprite("Relics.Dog_1.png");

            CurseOne = LoadSprite("Relics.Curse_One.png");
            CurseTwo = LoadSprite("Relics.Curse_Two.png");
            CurseThree = LoadSprite("Relics.Curse_Three.png");
            CurseFour = LoadSprite("Relics.Curse_Four.png");
            CurseFive = LoadSprite("Relics.Curse_Five.png");

            KillButtonRelic = LoadSprite("Relics.KillButton.png");
            KillButton = LoadSprite("KillButton.png");

            OrbOfGreed = LoadSprite("Orbs.OrbOfGreed.png", 8);
            OrbOfGreedAttack = LoadSprite("Orbs.OrbOfGreed.png", 16);

            RealityMarble = new Sprite[] { 
                LoadSprite("Relics.RealityMarble_0.png"),
                LoadSprite("Relics.RealityMarble_1.png"),
                LoadSprite("Relics.RealityMarble_2.png"),
                LoadSprite("Relics.RealityMarble_3.png"),
                LoadSprite("Relics.RealityMarble_4.png"),
            };

            stopwatch.Stop();
            Log.LogInfo($"Sprites loaded! Took {stopwatch.ElapsedMilliseconds}ms");
        }

        public static Texture2D LoadTexture(string filePath)
        {
            Texture2D texture;
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
            texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            texture.LoadImage(ReadToEnd(stream));
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.wrapModeU = TextureWrapMode.Clamp;
            texture.wrapModeV = TextureWrapMode.Clamp;
            texture.wrapModeW = TextureWrapMode.Clamp;
   
            return texture;
        }

        public static Sprite LoadSprite(string filePath, float pixelsPerUnit = 16f)
        {
            filePath = $"{Name}.Resources.{filePath}";
            Texture2D texture = LoadTexture(filePath);
            Sprite sprite =  Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            return sprite;
        }

        public static Sprite LoadSprite(string filePath, float width, float height, float pixelsPerUnit = 16f)
        {
            filePath = $"{Name}.Resources.{filePath}";
            Texture2D texture = LoadTexture(filePath);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            return sprite;
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}

