using ProLib.Relics;
using Promethium.Components;

namespace Promethium.Patches.Relics
{
    public sealed class Plasmaball : CustomRelic
    {
        public override void OnArmBallForShot(BattleController battleController)
        {
            battleController._activePachinkoBall.AddComponent<Plasma>();
        }
    }
}
