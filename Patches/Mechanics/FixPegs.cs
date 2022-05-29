using Battle;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(PegManager), nameof(PegManager.ResetPegs))]
    public static class FixPegs
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
