using Battle.Attacks;
using BepInEx.Configuration;
using I2.Loc;
using ProLib.Orbs;
using ProLib.Relics;
using Promethium.Components;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public sealed class Berserkorb : CustomOrb, IDamageModifier
    {
        public ConfigEntry<bool> EnabledConfig { internal set; get; }
        private static Berserkorb _instance;

        public Berserkorb() : base("berserkorb")
        {
            LocalVariables = true;
        }

        public static Berserkorb GetInstance()
        {
            if (_instance == null)
                _instance = new Berserkorb();
            return _instance;
        }

        public override void CreatePrefabs()
        {
            GameObject shotPrefab = new CustomShotBuilder()
                .SetName("berserkorbThrow")
                .SetSprite(Plugin.BerserkorbAttack)
                .Build();

            ShotBehavior behavior = shotPrefab.GetComponent<ShotBehavior>();
            behavior._shotType = ShotBehavior.ShotType.OVERFLOW;

            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("BerserkOrb")
                .SetSprite(Plugin.Berserkorb)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .SetShot(shotPrefab)
                .SetRarity(PachinkoBall.OrbRarity.RARE)
                .SetDamage(1, 3)
                .IncludeInOrbPool(true)
                .SetLevel(1)
                .SetDescription(new string[] {"overflow2", "damage_increase_missing_health", "armor_damage_multiplier", "armor_remove_on_fire"})
                .AddParameter(ParamKeys.DAMAGE_INCREASE, "+1")
                .AddParameter(ParamKeys.CRIT_DAMAGE_INCREASE, "+1")
                .AddParameter(ParamKeys.HEALTH_THRESHOLD, GetHealthThreshold(1).ToString())
                .AddParameter(ParamKeys.ARMOR_DAMAGE_MULTIPLIER, $"x * {GetDamageShotMultiplier(1, true)}")
                .AddParameter(ParamKeys.ARMOR_REMOVED, $"{GetArmorRemoved(1, true) * 100}%");

            CustomOrbBuilder levelTwo = levelOne.Clone()
                .SetLevel(2)
                .SetDamage(2, 4)
                .IncludeInOrbPool(false)
                .AddParameter(ParamKeys.DAMAGE_INCREASE, "+1")
                .AddParameter(ParamKeys.CRIT_DAMAGE_INCREASE, "+1")
                .AddParameter(ParamKeys.HEALTH_THRESHOLD, GetHealthThreshold(2).ToString())
                .AddParameter(ParamKeys.ARMOR_DAMAGE_MULTIPLIER, $"x * {GetDamageShotMultiplier(2, true)}")
                .AddParameter(ParamKeys.ARMOR_REMOVED, $"{GetArmorRemoved(2, true) * 100}%");
            ;

            CustomOrbBuilder levelThree = levelTwo.Clone()
                .SetLevel(3)
                .SetDamage(3,5)
                .AddParameter(ParamKeys.DAMAGE_INCREASE, "+1")
                .AddParameter(ParamKeys.CRIT_DAMAGE_INCREASE, "+1")
                .AddParameter(ParamKeys.HEALTH_THRESHOLD, GetHealthThreshold(3).ToString())
                .AddParameter(ParamKeys.ARMOR_DAMAGE_MULTIPLIER, $"x * {GetDamageShotMultiplier(3, true)}")
                .AddParameter(ParamKeys.ARMOR_REMOVED, $"{GetArmorRemoved(3, true) * 100}%");

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

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            int level = attack.Level;
            GameObject inventory = GameObject.Find("InventoryView");
            GameObject battleUpgrade = GameObject.Find("BattleUpgradesCanvas");
            if ((inventory != null && inventory.transform.GetChild(0).gameObject.activeInHierarchy) || (battleUpgrade != null && battleUpgrade.activeInHierarchy))
            {
                localParams.SetParameterValue(ParamKeys.ARMOR_DAMAGE_MULTIPLIER, $"x * {GetDamageShotMultiplier(level, true)}");
                localParams.SetParameterValue(ParamKeys.ARMOR_REMOVED, $"{GetArmorRemoved(level, true) * 100}%");
            }
            else
            {
                localParams.SetParameterValue(ParamKeys.ARMOR_DAMAGE_MULTIPLIER, $"{GetDamageShotMultiplier(level)}x");
                localParams.SetParameterValue(ParamKeys.ARMOR_REMOVED, $"{GetArmorRemoved(level)}");
            }
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            float multiplier = GetDamageShotMultiplier(attack.Level);
            if (multiplier > 0 && multiplier != 1)
                battleController._damageMultipliers.Add(multiplier);

            float armorRemoved = GetArmorRemoved(attack.Level);
            ArmorManager armor = ArmorManager.Instance;
            armor.RemoveArmor(armorRemoved);

        }

        public int GetHealthThreshold(int level)
        {
            int threshhold = 20;
            if (level == 2) threshhold = 15;
            else if (level == 3) threshhold = 10;

            return threshhold;
        }

        public float GetDamageShotMultiplier(int level, bool showMath = false)
        {
            float multiplier = 0;
            ArmorManager armor = ArmorManager.Instance;

            if (armor != null)
            {

                if (level == 1) multiplier = 0.08f;
                else if (level == 2) multiplier = 0.1f;
                else if (level == 3) multiplier = 0.12f;

                if (!showMath)
                    multiplier = (multiplier * armor.CurrentArmor.Value) + 1;
            }

            return multiplier;
        }

        public float GetArmorRemoved(int level, bool showMath = false)
        {
            float percentRemoved = 0.5f;
            if (level == 2) percentRemoved = 0.4f;
            if (level == 3) percentRemoved = 0.25f;

            if (showMath) return percentRemoved;
            ArmorManager armor = ArmorManager.Instance;
            int remove = (int)Mathf.Max(0, Mathf.Ceil(armor.CurrentArmor.Value * percentRemoved));

            return remove;
        }

        public float GetDamageModifier(Attack attack, RelicManager relicManager, int critCount, float damage)
        {
            if(attack.locNameString == GetName())
            {
                int level = attack.Level;
                float missingHealth = relicManager._maxPlayerHealth.Value - relicManager._playerHealth.Value;
                int threshold = GetHealthThreshold(level);

                return Mathf.Floor(missingHealth / threshold);
            }

            return 0f;
        }
    }
}
