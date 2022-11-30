using Battle.Attacks;
using BepInEx.Configuration;
using I2.Loc;
using ProLib.Orbs;
using Promethium.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public sealed class Orbgis : CustomOrb
    {

        public ConfigEntry<bool> EnabledConfig { internal set; get; }

        private static Orbgis _instance;

        private Orbgis() : base("orbgis")
        {
            LocalVariables = true;
        }

        public static Orbgis GetInstance()
        {
            if (_instance == null) _instance = new Orbgis();
            return _instance;
        }

        public override void CreatePrefabs()
        {
            GameObject shotPrefab = new CustomShotBuilder()
                .SetName("orbgisThrow")
                .SetSprite(Plugin.OrbgisAttack)
                .Build();

            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("Orbgis")
                .SetSprite(Plugin.Orbgis)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .SetDamage(1, 2)
                .SetLevel(1)
                .SetDescription(new string[] { "armor_max", "armor_on_start", "armor_on_fire" })
                .AddParameter(ParamKeys.MAX_ARMOR_INCREASE, GetMaxArmor(1).ToString())
                .AddParameter(ParamKeys.ARMOR_START, GetArmorOnStart(1).ToString())
                .AddParameter(ParamKeys.ARMOR_SHOT_INCREASE, GetArmorOnShot(1).ToString())
                .IncludeInOrbPool(true)
                .SetRarity(PachinkoBall.OrbRarity.UNCOMMON)
                .SetShot(shotPrefab);

            CustomOrbBuilder levelTwo = levelOne.Clone()
                .SetLevel(2)
                .SetDamage(2, 3)
                .AddParameter(ParamKeys.MAX_ARMOR_INCREASE, GetMaxArmor(2).ToString())
                .AddParameter(ParamKeys.ARMOR_START, GetArmorOnStart(2).ToString())
                .AddParameter(ParamKeys.ARMOR_SHOT_INCREASE, GetArmorOnShot(2).ToString())
                .IncludeInOrbPool(false);

            CustomOrbBuilder levelThree = levelTwo.Clone()
                .SetLevel(3)
                .SetDamage(3, 4)
                .AddParameter(ParamKeys.MAX_ARMOR_INCREASE, GetMaxArmor(3).ToString())
                .AddParameter(ParamKeys.ARMOR_START, GetArmorOnStart(3).ToString())
                .AddParameter(ParamKeys.ARMOR_SHOT_INCREASE, GetArmorOnShot(3).ToString());

            this[1] = levelOne.Build();
            this[2] = levelTwo.Build();
            this[3] = levelThree.Build();

            CustomOrbBuilder.JoinLevels(this[1], this[2], this[3]);
        }

        public override bool IsEnabled()
        {
            if (EnabledConfig == null)
            {
                EnabledConfig = Plugin.ConfigFile.Bind<bool>("Custom Orbs", GetName(), true, "Disable to remove from orb pool");
            }

            return EnabledConfig.Value;
        }

        public int GetMaxArmor(int level)
        {
            int amount = level * 2;
            return amount;
        }


        public int GetArmorOnShot(int level)
        {
            int amount = level * 2;
            return amount;
        }

        public int GetArmorOnStart(int level)
        {
            int amount = level * 1;
            return amount;
        }

        public override void OnBattleStart(BattleController battleController, GameObject orb, Attack attack)
        {
            ArmorManager.Instance?.AddMaxArmor(GetMaxArmor(attack.Level));
            ArmorManager.Instance?.AddArmor(GetArmorOnStart(attack.Level));
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            ArmorManager.Instance?.AddArmor(GetArmorOnShot(attack.Level));
        }
    }
}
