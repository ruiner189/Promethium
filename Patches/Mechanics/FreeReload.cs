using HarmonyLib;

namespace Promethium.Patches
{
    [HarmonyPatch(typeof(BattleController), "ChooseShuffleOrDrawAtEndOfTurn")]
    public static class TurnEnd
    {
        public static void Postfix(ref bool ____skipPlayersTurn)
        {
            if (!Plugin.EnemyAttackOnReload)
                ____skipPlayersTurn = false;
        }
    }
}
