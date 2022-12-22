using HarmonyLib;
using Promethium.Components;
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
        public ConfigEntry<bool> EnabledConfig { internal set; get; }

        private Oreb() : base(Name) {
        
        }

        public static void Register()
        {
            GetInstance();
        }

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
                .SetRarity(PachinkoBall.OrbRarity.RARE)
                .SetDescription(new string[] { "wrong_shape", "fragile_on_hit" });

            CustomOrbBuilder levelTwo = levelOne.Clone()
                .SetLevel(2)
                .SetDamage(2,2)
                .IncludeInOrbPool(false)
                .AddToDescription("fragile_split");

            CustomOrbBuilder levelThree = levelTwo.Clone()
                .SetLevel(3)
                .SetDamage(2, 3);


           this[1] = levelOne.Build();
           this[2] = levelTwo.Build();
           this[3] = levelThree.Build();

            Fragile fOne = this[1].AddComponent<Fragile>();
            fOne.HitToSplitCount = 3;
            fOne.OnValidate();

            Fragile fTwo = this[2].AddComponent<Fragile>();
            fTwo.HitToSplitCount = 3;
            fTwo.MaxSplits = 2;
            fTwo.OnValidate();

            Fragile fThree = this[3].AddComponent<Fragile>();
            fThree.HitToSplitCount = 2;
            fThree.MaxSplits = 3;
            fThree.OnValidate();

            CustomOrbBuilder.JoinLevels(this[1], this[2], this[3]);
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
