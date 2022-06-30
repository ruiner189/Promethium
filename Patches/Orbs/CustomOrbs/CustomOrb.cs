using HarmonyLib;
using PeglinUI.MainMenu;
using Promethium.Patches.Orbs.ModifiedOrbs;
using System;
using System.Collections.Generic;
using System.Reflection;
using ToolBox.Serialization;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public abstract class CustomOrb : ModifiedOrb
    {
        public static List<CustomOrb> AllCustomOrbs = new List<CustomOrb>();
        public CustomOrb(String orbName) : base(orbName)
        {
            if (Registered)
            {
                AllCustomOrbs.Add(this);
                CreatePrefabs();
            }
        }

        public static CustomOrb GetCustomOrbByName(String name)
        {
            return AllCustomOrbs.Find(orb => orb.GetName().ToLower() == name.ToLower());
        }

        public abstract GameObject GetPrefab(int level);

        public abstract void CreatePrefabs();
    }

    //[HarmonyPatch(typeof(DeckManager), nameof(DeckManager.LoadDeckData))]
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

                        if(gameObject == null)
                            Plugin.Log.LogWarning($"Found custom orb but could not find level {name[1]}!");
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
                catch (Exception e)
                {
                    Plugin.Log.LogError(e.StackTrace);
                }
            }

            __instance.InstantiateDeck(list);

            return false;
        }
    }

    [HarmonyPatch(typeof(MainMenuRandomOrbDrop), nameof(MainMenuRandomOrbDrop.FirePachinkoBall))]
    public static class FixMenuDrop
    {
        public static void Prefix(PachinkoBall pBall)
        {
            pBall.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(PachinkoBall), nameof(PachinkoBall.SetTrajectorySimulationRadius))]
    public static class FixTrajectorySimulation
    {
        public static void Prefix(PachinkoBall __instance)
        {
            if (__instance._trajectorySimulation == null)
                __instance._trajectorySimulation = __instance.GetComponent<TrajectorySimulation>();
        }
    }

    [HarmonyPatch(typeof(Resources))]
    public static class LoadOrbs
    {
        [HarmonyTargetMethod]
        public static MethodBase CalculateMethod()
        {
            List<MethodInfo> methods = AccessTools.GetDeclaredMethods(typeof(Resources));
            foreach (MethodInfo method in methods)
            {
                if (method.Name == "Load")
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 2 && parameters[1].ParameterType == typeof(Type))
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        public static bool Prefix(ref String path, ref UnityEngine.Object __result)
        {

            
            if (path.StartsWith("Prefabs/Orbs/"))
            {
                String str = path.Remove(0, 13);
                String[] name = str.Split(new string[] { "-Lvl" }, 2, StringSplitOptions.RemoveEmptyEntries);

                CustomOrb customOrb = CustomOrb.GetCustomOrbByName(name[0]);
                if (customOrb != null)
                {
                    try
                    {
                        GameObject gameObject = customOrb.GetPrefab(Int32.Parse(name[1]));
                        if (gameObject != null)
                        {
                            __result = gameObject;
                            return false;
                        }
                    }
                    catch (Exception){}
                    Plugin.Log.LogWarning($"Found custom orb but could not find level {name[1]}!");
                }

            }
            else if (path.StartsWith("$Prefabs/Orbs/"))
            {
                // Used to load vanilla orbs that we modify. This is to prevent a loop.
                path = path.Remove(0, 1);
            }
            return true;
        }
    }

}
