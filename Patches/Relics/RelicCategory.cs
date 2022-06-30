using HarmonyLib;
using Promethium.Utility;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Relics
{
    public class RelicCategory
    {
        private static readonly Dictionary<String, RelicCategory> RelicCategories = new Dictionary<string, RelicCategory>();
        public HashSet<RelicEffect> effects { get; private set; }
        public float CurrentWeight = 10;
        public readonly String Name;

        private RelicCategory(String name)
        {
            Name = name;
            RelicCategories.Add(name, this);
            effects = new HashSet<RelicEffect>();
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

        public static void AddCategories(CustomRelicEffect effect, params String[] categories)
        {
            AddCategories((RelicEffect)effect, categories);
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
                foreach(RelicCategory category in RelicCategories.Values)
                {
                    if (category.effects.Contains(relic.effect))
                    {
                        category.CurrentWeight += 15;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.RemoveRelic))]
        public static class RelicRemoved
        {
            public static void Prefix(RelicManager __instance, RelicEffect re)
            {
                foreach (RelicCategory category in RelicCategories.Values)
                {
                    if (category.effects.Contains(re))
                    {
                        category.CurrentWeight -= 15;
                    }
                }
            }
        }
    }
}
