using BepInEx.Configuration;
using ProLib.Orbs;
using ProLib.Relics;
using I2.Loc;
using Promethium.Extensions;
using Promethium.Patches.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedMatryoshka : ModifiedOrb
    {
        private static ModifiedMatryoshka _instance;

        private static readonly string _name = OrbNames.Matryoshka;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");

        private ModifiedMatryoshka() : base(_name) {
            LocalVariables = true;
        }

        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
        }

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            localParams.SetParameterValue(ParamKeys.MULTIBALL_HOLD_LEVEL, "1");
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (CustomRelicManager.RelicActive(RelicNames.HOLSTER))
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
