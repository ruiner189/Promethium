using Promethium.Components;

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
