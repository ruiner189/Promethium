using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;

namespace Promethium.SoftPatches
{
    public static class CustomStartDeck
    {
        public static List<String> wantedRelicEffects;
        private static List<String> _originalList;
        public static void GetPromethiumRelics(RelicManager __instance)
        {
            if (!Plugin.CustomStartDeckPlugin) return;

            if (wantedRelicEffects != null)
            {
                    _originalList = new List<String>(wantedRelicEffects);
                    List<String> namesToRemove = new List<String>();
                    foreach(String relicName in wantedRelicEffects)
                    {
                        CustomRelicEffect relicEffect = (CustomRelicEffect) Enum.Parse(typeof(CustomRelicEffect), relicName);
                        Relic relic = CustomRelic.AllCustomRelics.Find(relic => (CustomRelicEffect) relic.effect == relicEffect);
                        if(relic != null)
                        {
                            __instance.AddRelic(relic);
                            namesToRemove.Add(relicName);
                        }
                    }

                wantedRelicEffects.RemoveAll(name => namesToRemove.Contains(name)); // Removed to get rid of errors on the original mod.
            }
        }

        public static void ResetList() {
            wantedRelicEffects.Clear();
            wantedRelicEffects.AddRange(_originalList);
        }
    }
}
