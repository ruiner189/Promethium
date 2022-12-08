using Battle;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using ProLib.Relics;
using Promethium.Patches.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Promethium.Patches.Status_Effect;
using Relics;
using System;
using UnityEngine;

namespace Promethium.Components
{
    [HarmonyPatch]
    public class ArmorManager : MonoBehaviour
    {
        public FloatVariable CurrentArmor;
        public FloatVariable MaxArmor;

        private PlayerStatusEffectController _playerStatusEffectController;
        private RelicManager _relicManager;
        private CruciballManager _cruciballManager;

        public static ArmorManager Instance { get; private set; }

        public void Awake()
        {
            if (Instance == null) Instance = this;
            if (this != Instance) Destroy(this);

            CurrentArmor = ScriptableObject.CreateInstance<FloatVariable>();
            CurrentArmor.name = "CurrentArmor";
            CurrentArmor._initialValue = 0;
            CurrentArmor._value = 0;
            CurrentArmor.min = 0;

            MaxArmor = ScriptableObject.CreateInstance<FloatVariable>();
            MaxArmor.name = "MaxArmor";
            MaxArmor._initialValue = 0;
            MaxArmor._value = 0;
            MaxArmor.min = 0;

            CurrentArmor._maxVariable = MaxArmor;
        }


        public void Init(RelicManager relicManager, CruciballManager cruciballManager, PlayerStatusEffectController playerStatusEffectController)
        {
            SoftInit(relicManager, cruciballManager, playerStatusEffectController);
            ResetArmor();
        }

        public void SoftInit(RelicManager relicManager, CruciballManager cruciballManager, PlayerStatusEffectController playerStatusEffectController)
        {
            _playerStatusEffectController = playerStatusEffectController;
            _relicManager = relicManager;
            _cruciballManager = cruciballManager;
        }

        public void AddArmor(float amount)
        {
            if (_playerStatusEffectController == null)
            {
                Plugin.Log.LogWarning("Attempted to Add armor, but PlayerStatusEffetController is null! Aborting");
                return;
            }

            float start = CurrentArmor._value;
            int change = (int)(CurrentArmor.Add(amount) - start);

            if (change != 0)
            {
                StatusEffect armorEffect = new StatusEffect((StatusEffectType)CustomStatusEffect.Armor, change);
                _playerStatusEffectController.ApplyStatusEffect(armorEffect);
            }
        }

        public void RemoveArmor(float amount)
        {
            AddArmor(-amount);
        }

        public void AddMaxArmor(float amount)
        {
            MaxArmor.Add(amount);
        }

        public void RemoveMaxArmor(float amount)
        {
            MaxArmor.Subtract(amount);
            AddArmor(0);
        }

        public void ResetArmor()
        {
            CurrentArmor.Reset();
            MaxArmor.Reset();
            MaxArmor.Add(GetMaxArmorFromRelics());
        }

        public int GetArmorPerTurnFromRelics()
        {
            int total = 0;

            if (CustomRelicManager.Instance.AttemptUseRelic(RelicNames.CURSE_TWO_EQUIP))
                total += 1;
            if (CustomRelicManager.Instance.AttemptUseRelic(RelicNames.CURSE_FOUR_EQUIP))
                total += 1;
            if (ModifiedRelic.HasRelicEffect(RelicEffect.DAMAGE_BONUS_PLANT_FLAT) && _relicManager.AttemptUseRelic(RelicEffect.DAMAGE_BONUS_PLANT_FLAT))
                total += 1;

            return total;
        }

        public int GetMaxArmorFromRelics()
        {
            int total = 0;
            if (CustomRelicManager.Instance.AttemptUseRelic(RelicNames.CURSE_TWO_ARMOR))
                total += 5;
            if (CustomRelicManager.Instance.AttemptUseRelic(RelicNames.CURSE_FOUR_ARMOR))
                total += 5;
            if (ModifiedRelic.HasRelicEffect(RelicEffect.DAMAGE_BONUS_PLANT_FLAT) && _relicManager.AttemptUseRelic(RelicEffect.DAMAGE_BONUS_PLANT_FLAT))
                total += 5;
            return total;
        }

        #region Harmony Patches

        [HarmonyPatch(typeof(PlayerHealthController), nameof(PlayerHealthController.Damage))]
        [HarmonyPrefix]
        private static void PatchDamage(ref float damage)
        {
            ArmorManager armor = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armor != null)
            {
                float originalDamage = damage;
                damage = Math.Max(damage - armor.CurrentArmor.Value, 0);
                float difference = originalDamage - damage;
                armor.RemoveArmor(difference);
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Start))]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.First)]
        private static void PatchBattleStart(BattleController __instance)
        {
            Plugin.PromethiumManager.GetComponent<ArmorManager>()?.Init(__instance._relicManager, __instance._cruciballManager, __instance._playerStatusEffectController);
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.EnemyTurnComplete))]
        [HarmonyPostfix]
        private static void PatchEnemyTurnComplete()
        {
            ArmorManager armorManager = Plugin.PromethiumManager.GetComponent<ArmorManager>();
            if (armorManager != null)
            {
                armorManager.AddArmor(armorManager.GetArmorPerTurnFromRelics());
            }
        }
    }

    #endregion
}