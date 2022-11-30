using BepInEx.Configuration;
using ProLib.Extensions;
using ProLib.Orbs;
using Promethium.Components;
using Promethium.Components.Animations;
using Promethium.Patches.Orbs.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public sealed class Lasorb : CustomOrb
    {
        public ConfigEntry<bool> EnabledConfig { internal set; get; }
        private static Lasorb _instance;

        private Lasorb() : base("lasorb")
        {

        }
        public static Lasorb GetInstance()
        {
            if (_instance == null)
                _instance = new Lasorb();
            return _instance;
        }

        public override void CreatePrefabs()
        {
            GameObject laserAttackPrefab = new GameObject("laserAttack");
            SpriteRenderer renderer = laserAttackPrefab.AddComponent<SpriteRenderer>();
            renderer.sortingLayerName = "SidescrollerUI";

            laserAttackPrefab.AddComponent<LaserBehavior>();

            laserAttackPrefab.transform.SetParent(Plugin.PromethiumPrefabHolder.transform);
            laserAttackPrefab.HideAndDontSave();

            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("Lasorb")
                .SetSprite(Plugin.Lasorb[0])
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .SetDamage(1, 2)
                .SetLevel(1)
                .SetDescription(new string[] { "enemy_hit_row", "enemy_multihit_split", "laser_on_hit", "multiball_desc2" })
                .AddParameter(ParamKeys.ENEMY_HIT_COUNT, "3")
                .AddParameter(ParamKeys.LASER_HITS, "5")
                .AddParameter(ParamKeys.MULTIBALL_LEVEL, "1")
                .IncludeInOrbPool(true)
                .SetRarity(PachinkoBall.OrbRarity.UNCOMMON)
                .WithAttack<LaserAttack>();

            this[1] = levelOne.Build();

            LaserAttack attack = this[1].GetComponent<LaserAttack>();
            attack.SetAttackPrefabs(laserAttackPrefab);

            LaserGenerator laserOne = this[1].AddComponent<LaserGenerator>();
            laserOne.HitsForLazer = 5;

            this[1].AddComponent<LasorbAnimation>();
            this[1].AddComponent<Multiball>().multiballLevel = 1;

            CustomOrbBuilder levelTwo = levelOne
                .Clone()
                .WithPrefab(this[1])
                .SetLevel(2)
                .AddParameter(ParamKeys.ENEMY_HIT_COUNT, "3")
                .AddParameter(ParamKeys.LASER_HITS, "4")
                .AddParameter(ParamKeys.MULTIBALL_LEVEL, "1")
                .IncludeInOrbPool(false);

            this[2] = levelTwo.Build();
            LaserGenerator laserTwo = this[2].GetComponent<LaserGenerator>();
            laserTwo.HitsForLazer = 4;

            CustomOrbBuilder levelThree = levelTwo
                .Clone()
                .SetDamage(2,3)
                .SetDescription(new string[] { "enemy_hit_row", "enemy_multihit_split", "laser_on_hit", "increased_laser_duration", "multiball_desc2" })
                .AddParameter(ParamKeys.ENEMY_HIT_COUNT, "3")
                .AddParameter(ParamKeys.LASER_HITS, "3")
                .AddParameter(ParamKeys.LASER_DURATION_INCREASE, "100%")
                .AddParameter(ParamKeys.MULTIBALL_LEVEL, "1")
                .SetLevel(3);

            this[3] = levelThree.Build();
            LaserGenerator laserThree = this[3].GetComponent<LaserGenerator>();
            laserThree.HitsForLazer = 3;
            laserThree.LaserDuration = 3f;

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
    }
}
