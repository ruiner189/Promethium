using UnityEngine;
using Relics;
using Promethium.Components;
using I2.Loc;
using ProLib.Orbs;
using BepInEx.Configuration;
using ProLib.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Battle.Attacks;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedBouldorb : ModifiedOrb
    {
        private static ModifiedBouldorb _instance;
        private static readonly string _name = OrbNames.Bouldorb;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");

        private ModifiedBouldorb() : base(_name)
        {
            LocalVariables = true;
        }
        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
        }

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            localParams.SetParameterValue(ParamKeys.ARMOR_DISCARD, $"{GetArmorOnDiscard()}", true);
            localParams.SetParameterValue(ParamKeys.ARMOR_WHILE_HELD, $"{GetArmorWhileInHolster()}", true);
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (CustomRelicManager.RelicActive(RelicNames.HOLSTER))
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
