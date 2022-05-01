using Battle;
using HarmonyLib;


namespace Promethium.Patches
{
    [HarmonyPatch(typeof(TargetingManager), "StopTargetingOnFire")]
    public static class ChangeTargetWhileAttacking
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(BattleController), "Update")]
    public static class StopTargetingWhenAttackStarts
    {
        public static void Prefix(BattleController __instance, int ____battleState, TargetingManager ____targetingManager)
        {
            if(____battleState == 4)
            {
                ____targetingManager.GetType().GetField("_canTarget", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(____targetingManager, false);
            }
        }
    }


}
