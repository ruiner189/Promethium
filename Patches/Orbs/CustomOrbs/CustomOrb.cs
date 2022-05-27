﻿using HarmonyLib;
using Promethium.Patches.Orbs.ModifiedOrbs;
using System;
using System.Collections.Generic;
using ToolBox.Serialization;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public abstract class CustomOrb : ModifiedOrb
    {
        public static List<CustomOrb> AllCustomOrbs = new List<CustomOrb>();
        public CustomOrb(String orbName) : base(orbName){
            AllCustomOrbs.Add(this);
            CreatePrefabs();
        }

        public static CustomOrb GetCustomOrbByName(String name)
        {
            return AllCustomOrbs.Find(orb => orb.GetName() == name);
        }

        public abstract GameObject GetPrefab(int level);

        public abstract void CreatePrefabs();
    }

    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.LoadDeckData))]
    public static class LoadOrbData
    {
        public static bool Prefix(DeckManager __instance)
        {
            if (DeckManager.completeDeck == null)
            {
                Plugin.Log.LogWarning($"DeckManager::LoadDeckData(): Attempted to load a deck, but completeDeck was null. Bailing.");
                return false;
            }
            DeckManager.DeckManagerSaveData deckManagerSaveData = (DeckManager.DeckManagerSaveData)DataSerializer.Load<SaveObjectData>(DeckManager.DeckManagerSaveData.KEY);
            List<GameObject> list = new List<GameObject>();
            foreach (string str in deckManagerSaveData.PrefabNames)
            {
                try
                {
                    GameObject gameObject = null;

                    String[] name = str.Split(new string[] { "-Lvl" }, 2, StringSplitOptions.RemoveEmptyEntries);
                    CustomOrb customOrb = CustomOrb.GetCustomOrbByName(name[0]);
                    if (customOrb != null)
                    {
                        gameObject = customOrb.GetPrefab(Int32.Parse(name[1]));
                    }
                    else
                    {
                        gameObject = Resources.Load<GameObject>("Prefabs/Orbs/" + str);
                    }

                    if (gameObject != null)
                    {
                        list.Add(gameObject);
                    }
                    else
                    {
                        Plugin.Log.LogWarning($"Could not load the orb {str}. Perhaps this is a different version or a mod is missing?");
                    }
                }
                catch (Exception e){
                    Plugin.Log.LogError(e.StackTrace);
                }
            }

            __instance.InstantiateDeck(list);

            return false;
        }
    }
}
