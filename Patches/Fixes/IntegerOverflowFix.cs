using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Fixes
{
    [HarmonyPatch(typeof(Mathf), nameof(Mathf.FloorToInt))]
    public static class IntegerOverflowFix
    {
        public static void Postfix(float f, ref int __result)
        {
            if(__result == int.MinValue && f > 0)
            {
                __result = int.MaxValue;
            }      
        }
    }
}
