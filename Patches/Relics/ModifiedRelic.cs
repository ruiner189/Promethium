using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Relics
{
    [HarmonyPatch(typeof(Relic), nameof(Relic.descKey), MethodType.Getter)]
    public static class ModifiedRelic
    {
        public static List<RelicEffect> ModifiedRelics = new List<RelicEffect>();
        public static readonly String AltText = $"_{Plugin.Name.ToLower()}";

        public static void Postfix(Relic __instance, ref String __result)
        {
            if (ModifiedRelics.Contains(__instance.effect)) __result += AltText;
        }
    }

}
