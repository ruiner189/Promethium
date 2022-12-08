using Battle;
using HarmonyLib;
using ProLib.Orbs;
using Promethium.Components.Managers;
using Promethium.Patches.Orbs.Attacks;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs.Potions
{
    public sealed class BerserkPotion : Potion
    {
        private static BerserkPotion _instance;

        private BerserkPotion() : base("berserkpotion")
        {
            ShouldSkip = true;
        }

        public static void Register()
        {
            GetInstance();
        }

        public override void CreatePrefabs()
        {
            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("BerserkPotion")
                .SetDescription(new string[] { "potion_berserk", "potion_duration", "once_per_battle" })
                .AddParameter(ParamKeys.DURATION, "3")
                .AddParameter(ParamKeys.DAMAGE_DEALT_MULTIPLIER, "1.5x")
                .AddParameter(ParamKeys.DAMAGE_RECEIVED_MULTIPLIER, "2x")
                .SetLevel(1)
                .SetRarity(PachinkoBall.OrbRarity.UNCOMMON)
                .SetSprite(Plugin.BerserkPotion)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .IncludeInOrbPool(true)
                .WithAttack<PotionAttack>();

            this[1] = levelOne.Build();

            PotionAttack attack = this[1].GetComponent<PotionAttack>();
            attack.AffectOtherPotions = false;
            attack.Temporary = true;
            attack.Duration = 3;
        }

        public static BerserkPotion GetInstance()
        {
            if (_instance == null)
                _instance = new BerserkPotion();
            return _instance;
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.ShotFired))]
        [HarmonyPostfix]
        private static void PatchShotFired(BattleController __instance)
        {
            foreach (GameObject obj in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = obj.GetComponent<PotionAttack>();
                if (attack != null)
                {
                    if (attack.locNameString == GetInstance().GetName())
                    {
                        __instance.AddDamageMultiplier(1.5f);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerHealthController), nameof(PlayerHealthController.Damage))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static void PatchPlayerDamage(PlayerHealthController __instance, ref float damage)
        {
            foreach (GameObject obj in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = obj.GetComponent<PotionAttack>();
                if (attack != null)
                {
                    if (attack.locNameString == GetInstance().GetName())
                    {
                        damage *= 2;
                    }
                }
            }
        }

    }
}
