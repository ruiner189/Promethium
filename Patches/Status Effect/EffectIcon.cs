using Battle;
using Battle.StatusEffects;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Promethium.Patches.Status_Effect
{

    [HarmonyPatch(typeof(StatusEffectData), nameof(StatusEffectData.GetStatusEffectIcon))]
    public static class GetIcon
    {
        public static bool Prefix(int type, ref Sprite __result)
        {
            if (type == (int)CustomStatusEffect.Armor)
            {
                __result = Plugin.ArmorEffect;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerStatusEffectController), nameof(PlayerStatusEffectController.ResolveStatusEffects))]
    public static class Remove
    {
        public static void Prefix(List<StatusEffect> ____statusEffects)
        {
            ____statusEffects.RemoveAll(effect => effect.Intensity <= 0);
        }
    }

    [HarmonyPatch(typeof(StatusEffectIcon), nameof(StatusEffectIcon.SetData), new[] {typeof(int)})] 
    public static class SetDataOnly
    {
        public static bool Prefix(int intensity, TextMeshProUGUI ____intensityText)
        {
            ____intensityText.text = intensity.ToString();
            return false;
        }
    }
}
