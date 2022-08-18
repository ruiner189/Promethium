﻿using DG.Tweening;
using ProLib.Relics;
using HarmonyLib;
using Promethium.Components;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public static class BeltScale
    {
        public static Vector3 Scale;

        private const float TargetShrink = 0.5f;
        private const float TargetEnlarge = 2f;
        private const float Time = 1f;

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.ArmBallForShot))]
        public static class ChangeBallSizeOnSetup
        {
            [HarmonyPriority(Priority.Low)]
            public static void Prefix(BattleController __instance, RelicManager ____relicManager)
            {
                if (____relicManager == null) return;
                Scale = __instance._activePachinkoBall.transform.localScale;

                if (CustomRelicManager.RelicActive(RelicNames.WUMBO) && !CustomRelicManager.RelicActive(RelicNames.MINI))
                {
                    CustomRelicManager.AttemptUseRelic(RelicNames.WUMBO);
                    __instance._activePachinkoBall.transform.DOScale(new Vector3(Scale.x * TargetEnlarge, Scale.y * TargetEnlarge, Scale.z), Time);
                }

                else if (CustomRelicManager.RelicActive(RelicNames.MINI) && !CustomRelicManager.RelicActive(RelicNames.WUMBO))
                {
                    CustomRelicManager.AttemptUseRelic(RelicNames.MINI);
                    __instance._activePachinkoBall.transform.DOScale(new Vector3(Scale.x * TargetShrink, Scale.y * TargetShrink, Scale.z), Time);
                }
                else if (CustomRelicManager.RelicActive(RelicNames.MINI) && CustomRelicManager.RelicActive(RelicNames.WUMBO))
                {
                    if (__instance._activePachinkoBall.GetComponent<AutoScaler>() == null)
                        __instance._activePachinkoBall.AddComponent<AutoScaler>();
                }
            }
        }
    }
}
