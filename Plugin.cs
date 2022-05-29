using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Promethium.Components;
using Promethium.Components.Loaders;
using Promethium.Loaders;
using Promethium.Patches.Language;
using Promethium.Patches.Orbs.ModifiedOrbs;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UI;
using UnityEngine;

namespace Promethium
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {

        public const String GUID = "com.ruiner.promethium";
        public const String Name = "Promethium";
        public const String Version = "1.1.3";

        private Harmony _harmony;
        public static ManualLogSource Log;
        public static ConfigFile ConfigFile;

        public static GameObject PromethiumManager;

        // Sprites
        public static Sprite ArmorEffect;
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

        //Localization
        public static List<String[]> LocalizationTerms;
        public static List<String> LocalizationKeys = new List<string>();

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


        public static bool EnemyAttackOnReload => EnemyAttackOnShuffleConfig.Value;
        public static bool CurseRunOn => CurseRunOnConfig.Value;
        public static bool PruneRelicsOnNewCurseRunOn => PruneRelicsOnNewCurseRunConfig.Value;
        public static bool PruneOrbsOnNewCurseRunOn => PruneOrbsOnNewCurseRunConfig.Value;
        public static float TierOneHealthMultiplier => TierOneCurseHealth.Value;
        public static float ExponentialCurseHealthMultiplier => ExponentialCurseHealth.Value;

        public static bool SpeedUpOn => SpeedUpOnConfig.Value;
        public static float SpeedUpDelay => SpeedUpDelayConfig.Value;
        public static float SpeedUpMax => SpeedUpMaxConfig.Value;
        public static float SpeedUpRate => SpeedUpRateConfig.Value;

        private void Awake()
        {
            Log = Logger;
            ConfigFile = Config;

            LocalizationTerms = ReadTSVFile("Localization.tsv");

            LoadSprites();
            RegisterModifiedOrbs();
            RegisterModifiedRelics();

            EnemyAttackOnShuffleConfig = Config.Bind<bool>("Mechanics", "EnemyAttackOnShuffle", true, "Disabling this will prevent enemies from taking two turns in certain circumstances");
            SpeedUpOnConfig = Config.Bind<bool>("Mechanics", "SpeedUpOn", true, "Speeds up game after a set amount of time");
            SpeedUpDelayConfig = Config.Bind<float>("Mechanics", "SpeedUpDelay", 10, "Delay for speed up. In seconds");
            SpeedUpMaxConfig = Config.Bind<float>("Mechanics", "SpeedUpMax", 3, "How much it speeds up the game at max value");
            SpeedUpRateConfig = Config.Bind<float>("Mechanics", "SpeedUpRate", 1, "How fast the mod transitions the speed-up. Higher values means the game will speed up faster");

            CurseRunOnConfig = Config.Bind<bool>("Curse Run", "CurseRunOn", true, "Finish a game to increase your curse level. How far can you go?");
            PruneRelicsOnNewCurseRunConfig = Config.Bind<bool>("Curse Run", "PruneRelicOnCurseRun", false, "Reduces the amount of relics when starting a new curse run. Disabling lets you keep all relics.");
            PruneOrbsOnNewCurseRunConfig = Config.Bind<bool>("Curse Run", "PruneOrbsOnCurseRun", false, "Reduces the amount of orbs to four when starting a new curse run. Disabling lets you keep all orbs.");
            TierOneCurseHealth = Config.Bind<float>("Curse Run", "TierOneHealthMultiplier", 3f, "Amount of health to multiply for tier 1 of chaos relics");
            ExponentialCurseHealth = Config.Bind<float>("Curse Run", "ExponentialCurseHealth", 2f, "Amount of health to multiple after tier 1. This is exponential");

            _harmony = new Harmony(GUID);
            _harmony.PatchAll();

            PromethiumManager = new GameObject("Promethium Mod");
            PromethiumManager.AddComponent<LanguageLoader>();
            PromethiumManager.AddComponent<RelicLoader>();
            PromethiumManager.AddComponent<OrbLoader>();
            PromethiumManager.AddComponent<RestartButtonActivator>();
            PromethiumManager.AddComponent<ArmorManager>();
            DontDestroyOnLoad(PromethiumManager);
            PromethiumManager.hideFlags = HideFlags.HideAndDontSave;
        }

        private void LoadSprites()
        {
            ArmorEffect = LoadSprite("ArmorEffect.png");
            Holster = LoadSprite("Relics.Holster.png");
            WumboBelt = LoadSprite("Relics.WumboBelt.png");
            MiniBelt = LoadSprite("Relics.MiniBelt.png");

            CurseOne = LoadSprite("Relics.Curse_One.png");
            CurseTwo = LoadSprite("Relics.Curse_Two.png");
            CurseThree = LoadSprite("Relics.Curse_Three.png");
            CurseFour = LoadSprite("Relics.Curse_Four.png");
            CurseFive = LoadSprite("Relics.Curse_Five.png");

            KillButtonRelic = LoadSprite("Relics.KillButton.png");
            KillButton = LoadSprite("KillButton.png");
        }

        private void RegisterModifiedOrbs()
        {
            ModifiedBouldorb.Register();
            ModifiedOrbelisk.Register();
            ModifiedStone.Register();
            ModifiedDoctorb.Register();
            ModifiedNosforbatu.Register();
            ModifiedRefreshOrb.Register();
            ModifiedShuffleOrb.Register();
        }

        private void RegisterModifiedRelics()
        {
            ModifiedRelic.AddRelic(RelicEffect.DAMAGE_BONUS_PLANT_FLAT); // Gardening Gloves
            ModifiedRelic.AddRelic(RelicEffect.NO_DISCARD, true);
        }



        public List<String[]> ReadTSVFile(String filePath)
        {
            filePath = $"{Name}.Resources.{filePath}";
            List<String[]> results = new List<String[]>();
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath)))
            {
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    String[] split = line.Split('\t');
                    results.Add(split);
                    if (split.Length > 1 && !LocalizationKeys.Contains(split[0]))
                    {
                        LocalizationKeys.Add(split[0]);
                    }
                }
            }
            return results;
        }

        public static Texture2D LoadTexture(string filePath)
        {
            Texture2D texture = null;
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
            texture = new Texture2D(10, 20, TextureFormat.ARGB32, false);
            texture.LoadImage(ReadToEnd(stream));
            texture.filterMode = FilterMode.Point;
            return texture;
        }

        public static Sprite LoadSprite(string filePath)
        {
            filePath = $"{Name}.Resources.{filePath}";
            Texture2D texture = LoadTexture(filePath);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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

        private void SendOrbsToConsole()
        {
            foreach (GameObject obj in Resources.LoadAll<GameObject>("Prefabs/Orbs/"))
            {
                Attack attack = obj.GetComponent<Attack>();
                if (attack != null)
                {
                    Logger.LogInfo($"{obj.name} ({attack.locNameString})");
                }
            }
        }
    }

    [HarmonyPatch(typeof(VersionDisplay), "Start")]
    public static class ModVersionDisplay
    {
        public static void Postfix(VersionDisplay __instance)
        {
            TMP_Text text = __instance.GetComponent<TMP_Text>();
            if (text.text.StartsWith("v"))
            {
                text.text = $"Peglin {text.text}\n{Plugin.Name} v{Plugin.Version}";
                __instance.transform.position += new Vector3(0, 0.5f, 0);
            }
        }
    }
}

