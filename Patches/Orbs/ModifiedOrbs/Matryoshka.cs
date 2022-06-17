using I2.Loc;
using Promethium.Extensions;
using Promethium.Patches.Relics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedMatryoshka : ModifiedOrb
    {
        private static ModifiedMatryoshka _instance;
        private ModifiedMatryoshka() : base(OrbNames.Matryoshka) {
            LocalVariables = true;
        }

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            localParams.SetParameterValue(ParamKeys.MULTIBALL_HOLD_LEVEL, "1");
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (relicManager != null && relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER))
                AddToDescription(attack, "multiball_on_hold");
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            PachinkoBall pachinko = attackingOrb.GetComponent<PachinkoBall>();
            if (pachinko != null)
            {
                pachinko.multiballLevel++;
            }
              
        }

        public static ModifiedMatryoshka Register()
        {
            if (_instance == null)
                _instance = new ModifiedMatryoshka();
            return _instance;
        }

    }
    
}
