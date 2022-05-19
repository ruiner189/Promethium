using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using Promethium.Components;
using Promethium.Extensions;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public static class BeltScale
    {
        public static Vector3 Scale;

        private const float TargetShrink = 0.5f;
        private const float TargetEnlarge = 2f;
        private const float Time = 1f;

        [HarmonyPatch(typeof(BattleController),nameof(BattleController.ArmBallForShot))]
        public static class ChangeBallSizeOnSetup
        {
            public static void Postfix(RelicManager ____relicManager, GameObject ____ball)
            {
                if (____relicManager == null) return;
                Scale = ____ball.transform.localScale;
                Plugin.Log.LogMessage("How many calls?");


                if (____relicManager.RelicEffectActive(CustomRelicEffect.WUMBO) && !____relicManager.RelicEffectActive(CustomRelicEffect.MINI)){
                    ____relicManager.AttemptUseRelic(CustomRelicEffect.WUMBO);
                    ____ball.transform.DOScale(new Vector3(Scale.x * TargetEnlarge, Scale.y * TargetEnlarge, Scale.z), Time);
                }

                else if (____relicManager.RelicEffectActive(CustomRelicEffect.MINI) && !____relicManager.RelicEffectActive(CustomRelicEffect.WUMBO))
                {
                    ____relicManager.AttemptUseRelic(CustomRelicEffect.MINI);
                    ____ball.transform.DOScale(new Vector3(Scale.x * TargetShrink, Scale.y * TargetShrink, Scale.z), Time);
                }
                else if (____relicManager.RelicEffectActive(CustomRelicEffect.MINI) && ____relicManager.RelicEffectActive(CustomRelicEffect.WUMBO))
                {
                    if (____ball.GetComponent<AutoScaler>() == null)
                        ____ball.AddComponent<AutoScaler>();
                }
            }
        }
    }
}
