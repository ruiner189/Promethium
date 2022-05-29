using Battle;
using HarmonyLib;
using System.Reflection;

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
        public static void Prefix(BattleController __instance, TargetingManager ____targetingManager)
        {
            __instance._targetingManager._canTarget =
                BattleController._battleState == BattleController.BattleState.AWAITING_SHOT ||
                BattleController._battleState == BattleController.BattleState.AWAITING_SHOT_COMPLETION;
        }
    }
}
