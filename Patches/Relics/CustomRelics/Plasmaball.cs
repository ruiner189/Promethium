using HarmonyLib;
using Promethium.Components;
using Promethium.Extensions;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public sealed class Plasmaball : CustomRelic
    {
        public override void OnArmBallForShot(BattleController battleController)
        {
            battleController._ball.AddComponent<Plasma>();
        }
    }
}
