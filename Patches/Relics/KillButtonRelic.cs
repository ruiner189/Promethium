using HarmonyLib;
using Promethium.Components;
using Promethium.Extensions;
using Promethium.Extensions.UI;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public static class KillButtonRelic
    {
        private static GameObject currentButton;

        [HarmonyPatch(typeof(BattleController), "ShuffleDeck")]
        public static class RestoreButtonOnReload
        {
            public static void Prefix(RelicManager ____relicManager)
            {
                if (____relicManager != null)
                {
                    if (____relicManager.AttemptUseRelic(CustomRelicEffect.KILL_BUTTON))
                    {
                        if (currentButton != null)
                        {
                            currentButton.SetActive(true);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), "Awake")]
        public static class CreateKillButtonOnLoad
        {
            public static void Postfix(RelicManager ____relicManager)
            {
                if (____relicManager.RelicEffectActive(CustomRelicEffect.KILL_BUTTON))
                    currentButton = KillButton.CreateButton(new Vector3(12, -4.5f, 0));
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.ArmBallForShot))]
        public static class AddKillComponent
        {
            public static void Prefix(RelicManager ____relicManager, GameObject ____ball)
            {
                if (____relicManager.RelicEffectActive(CustomRelicEffect.KILL_BUTTON))
                {
                    ____ball.AddComponent<KillOnCommand>();
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
