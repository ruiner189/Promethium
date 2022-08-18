using BepInEx.Configuration;
using ProLib.Orbs;
using I2.Loc;
using Promethium.Components;
using Relics;
using UnityEngine;
using Battle.Attacks;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedStone : ModifiedOrb
    {
        private static ModifiedStone _instance;
        private static readonly string _name = OrbNames.StoneOrb;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");
        private ModifiedStone() : base(_name)
        {
            LocalVariables = true;
        }
        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
        }

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            int level = attack.Level;
            int cruciballLevel = attack._cruciballManager != null ? attack._cruciballManager.currentCruciballLevel : -1;

            localParams.SetParameterValue(ParamKeys.ARMOR_PER_RELOAD, $"{GetArmorPerReload(level, cruciballLevel)}");
            localParams.SetParameterValue(ParamKeys.MAX_ARMOR_INCREASE, $"{GetMaxArmor(level, cruciballLevel)}");
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (attack.Level > 1)
            {
                ReplaceDescription(attack, new string[] { "armor_max", "armor_turn" });
            }
        }
        public static ModifiedStone Register()
        {
            if (_instance == null)
                _instance = new ModifiedStone();
            return _instance;
        }

        public int GetMaxArmor(int level, int cruciballLevel = -1)
        {
            int amount = (level - 1) * 3;

            if (cruciballLevel >= 3)
                amount = (level - 1) * 2;

            return amount;
        }

        public int GetArmorPerReload(int level, int cruciballLevel = -1)
        {
            int amount = (level - 1) * 2;

            if (cruciballLevel >= 3)
                amount = level - 1;

            return amount;
        }

        public override void OnDeckShuffle(BattleController battleController, GameObject orb, Attack attack)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                int cruciballLevel = attack._cruciballManager != null ? attack._cruciballManager.currentCruciballLevel : -1;
                armor.AddArmor(GetArmorPerReload(attack.Level, cruciballLevel));
            }
        }

        public override void OnBattleStart(BattleController battleController, GameObject orb, Attack attack)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                int cruciballLevel = attack._cruciballManager != null ? attack._cruciballManager.currentCruciballLevel : -1;
                armor.AddMaxArmor(GetMaxArmor(attack.Level, cruciballLevel));
                armor.AddArmor(GetArmorPerReload(attack.Level, cruciballLevel));
            }
        }
    }
}
