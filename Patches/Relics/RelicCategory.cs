using ProLib.Relics;
using ProLib.Utility;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;

namespace Promethium.Patches.Relics
{
    public class RelicCategory
    {
        private static readonly Dictionary<String, RelicCategory> RelicCategories = new Dictionary<string, RelicCategory>();
        public HashSet<RelicEffect> effects { get; private set; }
        public HashSet<CustomRelic> customRelics { get; private set; }
        public float CurrentWeight = 10;
        public readonly String Name;

        private RelicCategory(String name)
        {
            Name = name;
            RelicCategories.Add(name, this);
            effects = new HashSet<RelicEffect>();
            customRelics = new HashSet<CustomRelic>();
        }

        public static void AddCategories(RelicEffect effect, params String[] categories)
        {
            foreach(String category in categories)
            {
                if (!RelicCategories.ContainsKey(category))
                    new RelicCategory(category);
                RelicCategories[category].effects.Add(effect);
            }
        }

        public static void AddCategories(CustomRelic relic, params String[] categories)
        {
            foreach (String category in categories)
            {
                if (!RelicCategories.ContainsKey(category))
                    new RelicCategory(category);
                RelicCategories[category].customRelics.Add(relic);
            }
        }

        public static void AddCategories(String relicId, params String[] categories)
        {
            if(CustomRelic.TryGetCustomRelic(relicId, out CustomRelic relic))
            {
                AddCategories(relic, categories);
            }
        }

        public static float GetWeight(RelicEffect effect)
        {
            float weight = 10;
            int categories = 1;

            foreach (RelicCategory category in RelicCategories.Values)
            {
                if (category.effects.Contains(effect))
                {
                    weight += category.CurrentWeight;
                    categories++;
                }
            }

            return weight / categories;
        }

        public List<RelicCategory> GetRelicCategories(RelicEffect effect)
        {
            List<RelicCategory> relicCategories = new List<RelicCategory>();
            foreach(RelicCategory category in RelicCategories.Values)
            {
                if (category.effects.Contains(effect))
                {
                    relicCategories.Add(category);
                }
            }
            return relicCategories;
        }

        public List<RelicCategory> GetRelicCategories(CustomRelic relic)
        {
            List<RelicCategory> relicCategories = new List<RelicCategory>();
            foreach (RelicCategory category in RelicCategories.Values)
            {
                if (category.customRelics.Contains(relic))
                {
                    relicCategories.Add(category);
                }
            }
            return relicCategories;
        }

        public List<RelicCategory> GetRelicCategories(String relicId)
        {
            if(CustomRelic.TryGetCustomRelic(relicId, out CustomRelic relic))
            {
                return GetRelicCategories(relic);
            }

            return new List<RelicCategory>();
        }

        public static float GetCategoryWeight(RelicCategory category, RelicManager relicManager)
        {
            float weight = 10;
            foreach(RelicEffect effect in category.effects)
            {
                if (relicManager.RelicEffectActive(effect))
                {
                    weight += 15;
                }
            }

            foreach(CustomRelic relic in category.customRelics)
            {
                if (CustomRelicManager.RelicActive(relic))
                {
                    weight += 15;
                }
            }

            category.CurrentWeight = weight;
            return weight;
        }

        public static void CalculateAllCategoryWeights(RelicManager relicManager)
        {
            foreach(RelicCategory category in RelicCategories.Values)
            {
                GetCategoryWeight(category, relicManager);
            }
        }

        public static Relic GetRandomRelicWithWeights(List<Relic> relics)
        {
            WeightedList<Relic> set = new WeightedList<Relic>();
            foreach(Relic relic in relics)
            {
                set.Add(relic, GetWeight(relic.effect));
            }
            return set.GetRandomItem();
        }


        [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.AddRelic))]
        public static class RelicAdded
        {
            public static void Postfix(RelicManager __instance, Relic relic)
            {
                CustomRelic customRelic = relic as CustomRelic;
                foreach(RelicCategory category in RelicCategories.Values)
                {
                    if (category.effects.Contains(relic.effect))
                    {
                        category.CurrentWeight += 15;
                    }
                    if(customRelic != null && category.customRelics.Contains(customRelic))
                    {
                        category.CurrentWeight += 15;
                    }
                }
            }
        }
    }
}
