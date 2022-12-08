using Battle;
using HarmonyLib;
using ProLib.Orbs;
using Promethium.Components.Managers;
using Promethium.Patches.Orbs.Attacks;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs.Potions
{
    public sealed class IronSkinPotion : Potion
    {
        private static IronSkinPotion _instance;

        private IronSkinPotion() : base("ironskinpotion")
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
                .SetName("IronSkinPotion")
                .SetDescription(new string[] { "potion_ironskin", "potion_duration", "once_per_battle" })
                .AddParameter(ParamKeys.DURATION, "3")
                .AddParameter(ParamKeys.DAMAGE_REDUCTION, "1")
                .SetLevel(1)
                .SetRarity(PachinkoBall.OrbRarity.UNCOMMON)
                .SetSprite(Plugin.IronSkinPotion)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .IncludeInOrbPool(true)
                .WithAttack<PotionAttack>();

            this[1] = levelOne.Build();

            PotionAttack attack = this[1].GetComponent<PotionAttack>();
            attack.AffectOtherPotions = false;
            attack.Temporary = true;
            attack.Duration = 3;
        }

        public static IronSkinPotion GetInstance()
        {
            if (_instance == null)
                _instance = new IronSkinPotion();
            return _instance;
        }

        [HarmonyPatch(typeof(PlayerHealthController), nameof(PlayerHealthController.Damage))]
        [HarmonyPrefix]
        public static void PatchPlayerDamage(PlayerHealthController __instance, ref float damage)
        {
            foreach (GameObject obj in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = obj.GetComponent<PotionAttack>();
                if (attack != null)
                {
                    if (attack.locNameString == GetInstance().GetName())
                    {
                        damage = Mathf.Max(damage - 1, 0);
                    }
                }
            }
        }

    }
}
