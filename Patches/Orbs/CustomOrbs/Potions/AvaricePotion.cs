using Currency;
using HarmonyLib;
using ProLib.Orbs;
using Promethium.Components.Managers;
using Promethium.Patches.Orbs.Attacks;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs.Potions
{
    public sealed class AvaricePotion : Potion
    {
        private static AvaricePotion _instance;
        private AvaricePotion() : base("avaricepotion")
        {
            ShouldSkip = true;
        }

        public static AvaricePotion GetInstance()
        {
            if (_instance == null)
                _instance = new AvaricePotion();
            return _instance;
        }

        public static void Register()
        {
            GetInstance();
        }

        public override void CreatePrefabs()
        {
            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("AvaricePotion")
                .SetDescription("potion_avarice", "potion_duration", "once_per_battle")
                .AddParameter(ParamKeys.DURATION, "2")
                .AddParameter(ParamKeys.GOLD_MULTIPLIER, "2x")
                .AddParameter(ParamKeys.DAMAGE_MULTIPLIER, "75%")
                .SetRarity(PachinkoBall.OrbRarity.RARE)
                .SetSprite(Plugin.AvaricePotion)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .IncludeInOrbPool(true)
                .WithAttack<PotionAttack>();

            this[1] = levelOne.Build();

            PotionAttack attack = this[1].GetComponent<PotionAttack>();
            attack.AffectOtherPotions = false;
            attack.Temporary = true;
            attack.Duration = 2;
        }

        [HarmonyPatch(typeof(CurrencyManager), nameof(CurrencyManager.AddGold))]
        [HarmonyPrefix]
        private static void PatchAddGold(ref int amount)
        {
            foreach (GameObject obj in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = obj.GetComponent<PotionAttack>();
                if (attack != null)
                {
                    if (attack.locNameString == GetInstance().GetName())
                    {
                        amount *= 2;
                    }
                }
            }
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
                        __instance.AddDamageMultiplier(0.25f);
                    }
                }
            }
        }
    }
}
