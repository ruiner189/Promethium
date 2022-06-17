using HarmonyLib;
using Relics;
using System.Collections.Generic;

namespace Promethium.Patches.Fixes
{
    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.GetMultipleRelicsOfRarity))]
    public static class MaxRelicFix
    {
        public static bool Prefix(ref Relic[] __result, RelicManager __instance, int number, RelicRarity rarity, bool fallback)
        {
            List<Relic> availableRelics = new List<Relic>();
            if (rarity == RelicRarity.BOSS) availableRelics.AddRange(__instance._availableBossRelics);
            if (rarity == RelicRarity.RARE || (rarity == RelicRarity.BOSS && fallback && availableRelics.Count < number)) availableRelics.AddRange(__instance._availableRareRelics);
            if (rarity == RelicRarity.COMMON || (fallback && availableRelics.Count < number)) availableRelics.AddRange(__instance._availableCommonRelics);

            if (availableRelics.Count < number)
            {
                if(availableRelics.Count > 0)
                {
                    __result = availableRelics.ToArray();
                    return false;
                } else
                {
                    for(int i = 0; i < number; i++)
                    {
                        availableRelics.Add(__instance.consolationPrize);
                    }
                    __result = availableRelics.ToArray();
                    return false;
                }

            }
            return true;
        }
    }
}
