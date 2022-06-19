using Promethium.Patches.Orbs.CustomOrbs;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Promethium.SoftPatches
{
    public static class CustomStartDeck
    {
        public static List<String> wantedRelicEffects;
        public static List<String> wantedOrbs;
        private static List<String> _originalRelicList;
        private static List<String> _originalOrbList;
        private static List<GameObject> _orbsToAdd;
        public static void GetPromethiumRelics(RelicManager __instance)
        {
            if (!Plugin.CustomStartDeckPlugin) return;

            if (wantedRelicEffects != null)
            {
                    _originalRelicList = new List<String>(wantedRelicEffects);
                    List<String> namesToRemove = new List<String>();
                    foreach(String relicName in wantedRelicEffects)
                    {
                        try
                        {
                            CustomRelicEffect relicEffect = (CustomRelicEffect)Enum.Parse(typeof(CustomRelicEffect), relicName);
                            Relic relic = CustomRelic.AllCustomRelics.Find(relic => (CustomRelicEffect)relic.effect == relicEffect);
                            if (relic != null)
                            {
                                __instance.AddRelic(relic);
                                namesToRemove.Add(relicName);
                            }
                        }
                        catch (Exception){}
                    }

                wantedRelicEffects.RemoveAll(name => namesToRemove.Contains(name)); // Removed to get rid of errors on the original mod.
            }
        }

        public static void ResetRelicList() {
            wantedRelicEffects.Clear();
            wantedRelicEffects.AddRange(_originalRelicList);
        }


        public static void GetPromethiumOrbs()
        {
            _originalOrbList = new List<String>(wantedOrbs);
            _orbsToAdd = new List<GameObject>();
            List<String> namesToRemove = new List<String>();
            foreach(String orbName in wantedOrbs)
            {
                String[] name = orbName.Split(new string[] { "-Lvl" }, 2, StringSplitOptions.RemoveEmptyEntries);
                CustomOrb customOrb = CustomOrb.GetCustomOrbByName(name[0]);
                if (customOrb != null)
                {
                    try
                    {
                        GameObject orb = customOrb.GetPrefab(Int32.Parse(name[1]));
                        if (orb != null)
                        {
                            _orbsToAdd.Add(orb);
                            namesToRemove.Add(orbName);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            wantedOrbs.RemoveAll(name => namesToRemove.Contains(name));
        }

        public static void AddPromethiumOrbs(DeckManager ____deckManager)
        {
            foreach(GameObject orb in _orbsToAdd)
            {
                ____deckManager.AddOrbToDeck(orb);
            }
            wantedOrbs.Clear();
            wantedOrbs.AddRange(_originalOrbList);
        }
    }
}
