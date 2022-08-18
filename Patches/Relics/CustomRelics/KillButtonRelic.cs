using ProLib.Relics;
using HarmonyLib;
using Promethium.Components;
using Promethium.Patches.Relics.CustomRelics;
using Promethium.UI;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public static class KillButtonRelic
    {
        [HarmonyPatch(typeof(BattleController), "ShuffleDeck")]
        public static class RestoreButtonOnReload
        {
            public static void Prefix()
            {
                if (CustomRelicManager.AttemptUseRelic(RelicNames.KILL_BUTTON))
                {
                    if (KillButton.currentButton != null)
                    {
                        KillButton.currentButton.SetActive(true);
                    }
                } 
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.ArmBallForShot))]
        public static class AddKillComponent
        {
            public static void Prefix(BattleController __instance)
            {
                if (CustomRelicManager.RelicActive(RelicNames.KILL_BUTTON))
                {
                    __instance._activePachinkoBall.AddComponent<KillOnCommand>();
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), "Update")]
        public static class DisableKillAction
        {
            public static void Postfix(int ____battleState)
            {
                if (____battleState != 3) KillOnCommand.Kill = false;
            }
        }
    }
}
