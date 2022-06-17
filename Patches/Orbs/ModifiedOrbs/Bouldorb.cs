using Promethium.Extensions;
using UnityEngine;
using Relics;
using Promethium.Components;
using I2.Loc;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{

    public sealed class ModifiedBouldorb : ModifiedOrb
    {
        private static ModifiedBouldorb _instance;


        private ModifiedBouldorb() : base(OrbNames.Bouldorb)
        {
            LocalVariables = true;
        }

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            localParams.SetParameterValue(ParamKeys.ARMOR_DISCARD, $"{GetArmorOnDiscard()}", true);
            localParams.SetParameterValue(ParamKeys.ARMOR_WHILE_HELD, $"{GetArmorWhileInHolster()}", true);
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (relicManager != null && relicManager.RelicEffectActive(Relics.CustomRelicEffect.HOLSTER))
            {
                ReplaceDescription(attack, "armor_hold", 3);
            }
            else
            {
                ReplaceDescription(attack, "armor_discard", 3);
            }
        }

        public float GetArmorOnDiscard()
        {
            return 10;
        }

        public float GetArmorWhileInHolster()
        {
            return 3;
        }

        public override void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                armor.AddArmor(10);
            }
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                armor.AddArmor(3);
            }
        }


        public static ModifiedBouldorb Register()
        {
            if (_instance == null)
                _instance = new ModifiedBouldorb();
            return _instance;
        }
    }
}
