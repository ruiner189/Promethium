using Battle;
using HarmonyLib;

namespace Promethium.Patches.Fixes
{
    [HarmonyPatch(typeof(PegManager), nameof(PegManager.ResetPegs))]
    public static class PegFix
    {
        public static void Prefix(PegManager __instance)
        {
            foreach (var Peg in __instance._allPegs)
            {
                if (Peg is LongPeg)
                {
                    LongPeg longPeg = Peg as LongPeg;
                    longPeg._beingHit = false;
                }
            }
        }
    }
}
