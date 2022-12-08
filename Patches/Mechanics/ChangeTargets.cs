using Battle;
using HarmonyLib;

namespace Promethium.Patches
{
    [HarmonyPatch]
    public static class ChangeTargets
    {
        [HarmonyPatch(typeof(TargetingManager), nameof(TargetingManager.StopTargetingOnFire))]
        [HarmonyPrefix]
        public static bool PatchStopTargeting()
        {
            return false;
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Update))]
        [HarmonyPrefix]
        public static void PatchUpdate(BattleController __instance)
        {
            __instance._targetingManager._canTarget =
                BattleController._battleState == BattleController.BattleState.AWAITING_SHOT ||
                BattleController._battleState == BattleController.BattleState.AWAITING_SHOT_COMPLETION;
        }
    }
}
