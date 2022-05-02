using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Promethium.Patches.Balls;
using Promethium.Patches.Relics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Promethium
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {

        public const String GUID = "com.ruiner.promethium";
        public const String Name = "Promethium";
        public const String Version = "1.0.1";

        private Harmony _harmony;
        public static ManualLogSource Log;

        // Sprites
        public static Sprite ArmorEffect;
        public static Sprite Curse_One;
        public static Sprite Curse_Two;
        public static Sprite Curse_Three;
        public static Sprite Curse_Four;
        public static Sprite Curse_Five;


        // Config
        private static ConfigEntry<bool> EnemyAttackOnShuffleConfig;
        public static bool EnemyAttackOnReload => EnemyAttackOnShuffleConfig.Value;

        private void Awake()
        {
            Log = Logger;

            LoadSprites();
            RegisterModifiedOrbs();
            RegisterCustomRelics();

            _harmony = new Harmony(GUID);
            _harmony.PatchAll();

            EnemyAttackOnShuffleConfig = Config.Bind<bool>("Mechanics","EnemyAttackOnShuffle", true, "Disabling this will prevent enemies from taking two turns in certain circumstances");

            //SendOrbsToConsole();
        }

        private void LoadSprites()
        {
            ArmorEffect = LoadSprite("ArmorEffect.png");
            Curse_One = LoadSprite("Relics.Curse_One.png");
            Curse_Two = LoadSprite("Relics.Curse_Two.png");
            Curse_Three = LoadSprite("Relics.Curse_Three.png");
            Curse_Four = LoadSprite("Relics.Curse_Four.png");
            Curse_Five = LoadSprite("Relics.Curse_Five.png");

        }

        private void RegisterModifiedOrbs()
        {
            ModifiedBouldorb.Register();
            ModifiedOrbelisk.Register();
            ModifiedStone.Register();
        }

        private void RegisterCustomRelics()
        {
            CustomRelicFactory.Build("curse_one", Curse_One, CustomRelicEffect.CURSE_ONE);
            CustomRelicFactory.Build("curse_two", Curse_Two, CustomRelicEffect.CURSE_TWO);
            CustomRelicFactory.Build("curse_three", Curse_Three, CustomRelicEffect.CURSE_THREE);
            CustomRelicFactory.Build("curse_four", Curse_Four, CustomRelicEffect.CURSE_FOUR);
            CustomRelicFactory.Build("curse_five", Curse_Five, CustomRelicEffect.CURSE_FIVE);
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
            foreach(GameObject obj in Resources.LoadAll<GameObject>("Prefabs/Orbs/"))
            {
                Attack attack = obj.GetComponent<Attack>();
                if(attack != null)
                {
                    Logger.LogInfo($"{obj.name} ({attack.locName})");
                }
            }
        }




    }
}

