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
        private static FieldInfo _canTarget;
        public static void Prefix(BattleController __instance, int ____battleState, TargetingManager ____targetingManager)
        {
            if (_canTarget == null) _canTarget = ____targetingManager.GetType().GetField("_canTarget", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            _canTarget.SetValue(____targetingManager, ____battleState == 2 || ____battleState == 3);
        }
    }
}
