using HarmonyLib;

namespace Promethium.Patches
{
    [HarmonyPatch(typeof(BattleController), nameof(BattleController.ChooseShuffleOrDrawAtEndOfTurn))]
    public static class TurnEnd
    {
        public static void Postfix(BattleController __instance)
        {
            if (!Plugin.EnemyAttackOnReload)
            {
                __instance._skipPlayerTurnCount--;
                __instance._pegManager.ShuffleSpecialPegs(false);
            }
        }
    }
}
