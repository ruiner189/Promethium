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

        public override void CreatePrefabs()
        {
            CustomOrbBuilder levelOne = new CustomOrbBuilder("Oreb-Lvl1")
                .SetName("Oreb")
                .SetDamage(1,2)
                .IncludeInOrbPool(true)
                .SetLevel(1)
                .SetDescription(new string[] { "wrong_shape", "fragile_on_hit" });

            CustomOrbBuilder levelTwo = levelOne.Clone()
                .SetLevel(2)
                .SetDamage(2,2)
                .IncludeInOrbPool(false)
                .AddToDescription("fragile_split");

            CustomOrbBuilder levelThree = levelTwo.Clone()
                .SetLevel(3)
                .SetDamage(2, 3);


            GameObject one = levelOne.Build();
            GameObject two = levelTwo.Build();
            GameObject three = levelThree.Build();

            Fragile fOne = one.AddComponent<Fragile>();
            fOne.HitToSplitCount = 3;
            fOne.OnValidate();

            Fragile fTwo = two.AddComponent<Fragile>();
            fTwo.HitToSplitCount = 3;
            fTwo.MaxSplits = 2;
            fTwo.OnValidate();

            Fragile fThree = three.AddComponent<Fragile>();
            fThree.HitToSplitCount = 2;
            fThree.MaxSplits = 3;
            fThree.OnValidate();

            CustomOrbBuilder.JoinLevels(one, two, three);

            Prefabs[1] = one;
            Prefabs[2] = two;
            Prefabs[3] = three;
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
