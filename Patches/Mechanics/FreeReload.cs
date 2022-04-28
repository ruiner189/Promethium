using Battle;
using Battle.Enemies;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace PeglinMod.Patches
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
