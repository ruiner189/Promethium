using HarmonyLib;

namespace Promethium.Patches
{
    [HarmonyPatch(typeof(BattleController), "ChooseShuffleOrDrawAtEndOfTurn")]
    public static class TurnEnd
    {
        public static void Postfix(BattleController __instance)
        {
            if (!Plugin.EnemyAttackOnReload)
            {
                __instance._skipPlayerTurnCount = 0;
                __instance._pegManager.ShuffleSpecialPegs(false);
            }
        }
    }
}
