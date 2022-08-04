using Battle;
using ProLib.Utility;
using HarmonyLib;
using PeglinUtils;
using Relics;
using System;
using TMPro;
using System.Collections.Generic;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(SpecialSlotController), nameof(SpecialSlotController.BattleTurnComplete))]
    public static class SlotMulitiplier
    {
        public static WeightedList<float[]> Multipliers;
        public static void Prefix(SpecialSlotController __instance)
        {
            if (__instance.relicManager != null && __instance.slotTriggers != null && __instance._slotMultipliersRelicAmounts != null)
                if (__instance.relicManager.RelicEffectActive(RelicEffect.SLOT_MULTIPLIERS))
                {
                    float[] multipliers = GetMultipliers();
                    multipliers.Shuffle<float>();
                    for (int i = 0; i < __instance._slotMultipliersRelicAmounts.Length; i++)
                    {
                        try
                        {
                            __instance._slotMultipliersRelicAmounts[i] = multipliers[i];
                            __instance.slotTriggers[i].GetComponentInChildren<TextMeshProUGUI>().fontSize = 1;
                        }
                        catch (Exception){}
                    }
                }
        }

        public static void Postfix(SpecialSlotController __instance)
        {
            List<int> values = new List<int>();
            float lowestValue = float.MaxValue;

            for(int i = 0; i < __instance._slotMultipliersRelicAmounts.Length; i++)
            {
                float value = __instance._slotMultipliersRelicAmounts[i];
                if(lowestValue > value)
                {
                    lowestValue = value;
                    values.Clear();
                    values.Add(i);
                } else if (lowestValue == value)
                {
                    values.Add(i);
                }

            }
            if (__instance.relicManager.RelicEffectActive(RelicEffect.SLOT_PORTAL))
            {
                Random rand = new Random();
                int slot = values[rand.Next(0, values.Count)];

                for (int i = 0; i < __instance.slotTriggers.Length; i++)
                {
                    __instance.slotTriggers[i].TogglePortal(i == slot, __instance.bottomPortalColor);
                }
            }
        }

        private static void SetList()
        {
            if (Multipliers == null)
            {
                Multipliers = new WeightedList<float[]>();
                Multipliers.Add(new float[] { -1, 0.5f, 1, 1, 2 }, 10);
                Multipliers.Add(new float[] { -2, -2, 0.5f, 1.5f, 3 }, 10);
                Multipliers.Add(new float[] { -2f, 0.5f, 1, 2, 2 }, 10);
                Multipliers.Add(new float[] { 1, 1, 1, 1, 1 }, 10);
                Multipliers.Add(new float[] { 1.25f, 1.25f, 1, 0.75f, 0.75f }, 10);
                Multipliers.Add(new float[] { 10, 0, -2, 0, 10 }, 5);
                Multipliers.Add(new float[] { 0, 0, 100, 0, 0 }, 1);
            }
        }

        public static float[] GetMultipliers()
        {
            SetList();
            return Multipliers.GetRandomItem();
        }
    }
}
