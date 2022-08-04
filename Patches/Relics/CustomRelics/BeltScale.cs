using DG.Tweening;
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
            public static void Prefix(RelicManager ____relicManager, GameObject ____ball)
            {
                if (____relicManager == null) return;
                Scale = ____ball.transform.localScale;

                if (CustomRelicManager.RelicActive(RelicNames.WUMBO) && !CustomRelicManager.RelicActive(RelicNames.MINI))
                {
                    CustomRelicManager.AttemptUseRelic(RelicNames.WUMBO);
                    ____ball.transform.DOScale(new Vector3(Scale.x * TargetEnlarge, Scale.y * TargetEnlarge, Scale.z), Time);
                }

                else if (CustomRelicManager.RelicActive(RelicNames.MINI) && !CustomRelicManager.RelicActive(RelicNames.WUMBO))
                {
                    CustomRelicManager.AttemptUseRelic(RelicNames.MINI);
                    ____ball.transform.DOScale(new Vector3(Scale.x * TargetShrink, Scale.y * TargetShrink, Scale.z), Time);
                }
                else if (CustomRelicManager.RelicActive(RelicNames.MINI) && CustomRelicManager.RelicActive(RelicNames.WUMBO))
                {
                    if (____ball.GetComponent<AutoScaler>() == null)
                        ____ball.AddComponent<AutoScaler>();
                }
            }
        }
    }
}
