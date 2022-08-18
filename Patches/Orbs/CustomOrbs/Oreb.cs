using HarmonyLib;
using I2.Loc;
using Promethium.Components;
using Promethium.Extensions;
using System;
using UnityEngine;
using ProLib.Orbs;
using BepInEx.Configuration;
using Battle.Attacks;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public sealed class Oreb : CustomOrb
    {
        private static GameObject _levelOne;
        private static GameObject _levelTwo;
        private static GameObject _levelThree;
        private static Oreb _instance;

        public const String Name = "oreb";

        private Oreb() : base(Name) { }
        public ConfigEntry<bool> EnabledConfig { internal set; get; }
        public override bool IsEnabled()
        {
            if (EnabledConfig == null)
            {
                EnabledConfig = Plugin.ConfigFile.Bind<bool>("Custom Orbs", GetName(), true, "Disable to remove from orb pool");
            }

            return EnabledConfig.Value;
        }

        public static Oreb GetInstance()
        {
            if (_instance == null)
                _instance = new Oreb();
            return _instance;
        }

        public override GameObject GetPrefab(int level)
        {
            if (_levelOne == null || _levelTwo == null || _levelThree == null)
                CreatePrefabs();
            if (level == 1)
                return _levelOne;
            else if (level == 2)
                return _levelTwo;
            else if (level == 3)
                return _levelThree;
            return null;
        }

        public override void CreatePrefabs()
        {
            _levelOne = Resources.Load<GameObject>("$Prefabs/Orbs/Oreb-Lvl1");
            _levelOne.name = "Oreb-Lvl1";

            Fragile fragileOne = _levelOne.AddComponent<Fragile>();
            fragileOne.HitToSplitCount = 3;

            Attack attackOne = _levelOne.GetComponent<Attack>();
            attackOne.locDescStrings = new string[] {
                "wrong_shape",
                "fragile_on_hit"
            };
            attackOne.Level = 1;
            attackOne.DamagePerPeg = 1;
            attackOne.CritDamagePerPeg = 2;

            _levelOne.AddComponent<LocalizationParamsManager>();

            fragileOne.OnValidate();

            ////////////////////

            _levelTwo = GameObject.Instantiate(_levelOne);
            _levelTwo.name = "Oreb-Lvl2";
            Attack attackTwo = _levelTwo.GetComponent<Attack>();
            attackTwo.locDescStrings = new string[] {
                "wrong_shape",
                "fragile_on_hit",
                "fragile_split"
            };
            attackTwo.Level = 2;
            attackTwo.DamagePerPeg = 2;
            attackTwo.CritDamagePerPeg = 2;

            Fragile fragileTwo = _levelTwo.GetComponent<Fragile>();
            fragileTwo.HitToSplitCount = 3;
            fragileTwo.MaxSplits = 2;

            fragileTwo.OnValidate();
            _levelTwo.transform.SetParent(Plugin.PromethiumPrefabHolder.transform);
            _levelTwo.HideAndDontSave();


            ////////////////////

            _levelThree = GameObject.Instantiate(_levelOne);
            _levelThree.name = "Oreb-Lvl3";

            Fragile fragileThree = _levelThree.GetComponent<Fragile>();
            fragileThree.HitToSplitCount = 2;
            fragileThree.MaxSplits = 3;

            Attack attackThree = _levelThree.GetComponent<Attack>();
            attackThree.locDescStrings = new string[] {
                "wrong_shape",
                "fragile_on_hit",
                "fragile_split"
            };
            attackThree.Level = 3;
            attackThree.DamagePerPeg = 2;
            attackThree.CritDamagePerPeg = 3;

            fragileThree.OnValidate();
            _levelThree.transform.SetParent(Plugin.PromethiumPrefabHolder.transform);
            _levelThree.HideAndDontSave();


            ////////////////////

            attackOne.NextLevelPrefab = _levelTwo;
            attackTwo.NextLevelPrefab = _levelThree;
        }
    }

    [HarmonyPatch(typeof(Attack), nameof(Attack.IsStone), MethodType.Getter)]
    public static class NotStone
    {
        public static bool Prefix(Attack __instance, ref bool __result)
        {
            __result = __instance.locNameString == OrbNames.StoneOrb;
            return false;
        }
    }
}
