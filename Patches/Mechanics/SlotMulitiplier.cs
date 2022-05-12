using Battle;
using HarmonyLib;
using PeglinUtils;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(SpecialSlotController), "UpdateSlotMultipliers")]
    public static class SlotMulitiplier
    {
        public static void Prefix(SpecialSlotController __instance, float[] ____slotMultipliersRelicAmounts)
        {
            if(__instance.relicManager != null)
                if (__instance.relicManager.RelicEffectActive(RelicEffect.SLOT_MULTIPLIERS))
                {
                    float[] multipliers = GetMultipliers();
                    multipliers.Shuffle<float>();
                    for (int i = 0; i < ____slotMultipliersRelicAmounts.Length; i++)
                    {
                        ____slotMultipliersRelicAmounts[i] = multipliers[i];
                        __instance.slotTriggers[i].GetComponentInChildren<TextMeshProUGUI>().fontSize = 1;
                    }
                }
        }

        public static float[] GetMultipliers()
        {
            Random rand = new Random();
            int choice = rand.Next(0, 56);
            if (choice >= 0 && choice < 10) return new float[] { 0.5f, 0.5f, 1, 1, 2 };
            if (choice >= 10 && choice < 20) return new float[] {0.25f, 0.25f, 0.5f, 1.5f, 3};
            if (choice >= 20 && choice < 30) return new float[] { 0.25f, 0.5f, 1, 2, 2 };
            if (choice >= 30 && choice < 40) return new float[] { 1, 1, 1, 1, 1 };
            if (choice >= 40 && choice < 50) return new float[] { 1.25f, 1.25f, 1, 0.75f, 0.75f };
            if (choice >= 50 && choice < 55) return new float[] { 10, 0, 0, 0, 10};
            if (choice >= 56 && choice < 57) return new float[] { 0, 0, 100, 0, 0 };
            return new float[] { 0.25f, 0.5f, 1, 2, 2 };
        }
    }



}
