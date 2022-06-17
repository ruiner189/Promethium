using Battle;
using HarmonyLib;
using Worldmap;

namespace Promethium.Patches.Fixes
{
    /*
     * Swapping when health and max health are set. This is because max health clamps health, and any health above 100 will be set back to 100.
     */
    [HarmonyPatch(typeof(MapController), nameof(MapController.LoadPlayerData))]
    public static class MaxHealthFix
    {
        public static bool Prefix(MapController __instance, PlayerHealthController.PlayerStatsSaveData saveData)
        {
            if (saveData != null)
            {
                __instance._playerMaxHealth.Set(saveData.PlayerMaxHealth);
                __instance._playerHealth.Set(saveData.PlayerHealth);
                return false;
            }
            Plugin.Log.LogWarning("Attempted to load player data and it was missing from our saves");
            return false;
        }
    }
}
