using Battle;
using HarmonyLib;
using PeglinUtils;
using Promethium.Utility;
using Relics;
using System;
using TMPro;

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

        private static void SetList()
        {
            if (Multipliers == null)
            {
                Multipliers = new WeightedList<float[]>();
                Multipliers.Add(new float[] { 0.5f, 0.5f, 1, 1, 2 }, 10);
                Multipliers.Add(new float[] { 0.25f, 0.25f, 0.5f, 1.5f, 3 }, 10);
                Multipliers.Add(new float[] { 0.25f, 0.5f, 1, 2, 2 }, 10);
                Multipliers.Add(new float[] { 1, 1, 1, 1, 1 }, 10);
                Multipliers.Add(new float[] { 1.25f, 1.25f, 1, 0.75f, 0.75f }, 10);
                Multipliers.Add(new float[] { 10, 0, 0, 0, 10 }, 5);
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
