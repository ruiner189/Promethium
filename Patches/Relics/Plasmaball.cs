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

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.ArmBallForShot))]
    public static class AddPlasmaComponentOnLaunch
    {
        [HarmonyPriority(Priority.Low)]
        public static void Prefix(RelicManager ____relicManager, GameObject ____ball)
        {
            if (____relicManager.RelicEffectActive(CustomRelicEffect.PLASMA_BALL))
            {
                ____ball.AddComponent<Plasma>();
            }
        }
    }

}
