using HarmonyLib;
using ProLib.Orbs;
using Promethium.Components.Managers;
using Promethium.Patches.Orbs.Attacks;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs.Potions
{
    public sealed class CriticalPotion : Potion
    {
        private static CriticalPotion _instance;

        private CriticalPotion() : base("criticalpotion")
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
                .SetName("CriticalPotion")
                .SetDescription(new string[] { "potion_critical", "potion_duration", "once_per_battle" })
                .AddParameter(ParamKeys.DURATION, "3")
                .SetLevel(1)
                .SetRarity(PachinkoBall.OrbRarity.COMMON)
                .SetSprite(Plugin.CriticalPotion)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .IncludeInOrbPool(true)
                .WithAttack<PotionAttack>();

            this[1] = levelOne.Build();

            PotionAttack attack = this[1].GetComponent<PotionAttack>();
            attack.AffectOtherPotions = false;
            attack.Temporary = true;
            attack.Duration = 3;
        }

        public static CriticalPotion GetInstance()
        {
            if (_instance == null)
                _instance = new CriticalPotion();
            return _instance;
        }

        

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.CheckForForcedCritical))]
        [HarmonyPostfix]
        private static void PatchForceCritical(BattleController __instance)
        {
            foreach (GameObject obj in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = obj.GetComponent<PotionAttack>();
                if (attack != null)
                {
                    if (attack.locNameString == GetInstance().GetName())
                    {
                        if (BattleController._criticalHitCount > 0)
                        {
                            BattleController._criticalHitCount++;
                        }
                        else
                        {
                            __instance.ActivateCrit();
                        }

                    }
                }
            }
        }
    }
}
