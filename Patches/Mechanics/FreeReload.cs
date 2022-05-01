using HarmonyLib;


namespace Promethium.Patches
{
    [HarmonyPatch(typeof(BattleController), "ChooseShuffleOrDrawAtEndOfTurn")]
    public static class TurnEnd
    {
		private static void Postfix(BattleController __instance, ref bool ____skipPlayersTurn)
		{
			if(!Plugin.EnemyAttackOnReload)
				____skipPlayersTurn = false;
		}
	}
}
