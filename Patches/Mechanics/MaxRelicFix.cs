using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetMultipleRelicsOfRarity))]
    public static class MaxRelicFix
    {
        public static bool Prefix(Relic[] __result, RelicManager __instance, int number, RelicRarity rarity, bool fallback, List<Relic> ____availableCommonRelics, List<Relic> ____availableRareRelics, List<Relic> ____availableBossRelics)
        {
            List<Relic> availableRelics = new List<Relic>();
            if (rarity == RelicRarity.BOSS) availableRelics.AddRange(____availableBossRelics);
            if (rarity == RelicRarity.RARE || (rarity == RelicRarity.BOSS && fallback && availableRelics.Count < number)) availableRelics.AddRange(____availableRareRelics);
            if (rarity == RelicRarity.COMMON || (fallback && availableRelics.Count < number)) availableRelics.AddRange(____availableCommonRelics);

            if (availableRelics.Count < number)
            {
                __result = availableRelics.ToArray();
                return false;
            }
            return true;
        }
    }
}
