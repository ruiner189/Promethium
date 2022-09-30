using Battle;
using BepInEx.Configuration;
using ProLib.Orbs;
using HarmonyLib;
using I2.Loc;
using Promethium.Extensions;
using Relics;
using UnityEngine;
using Battle.Attacks;
using ProLib.Loaders;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public sealed class OrbofGreed : CustomOrb
    {
        private static OrbofGreed _instance;

        public static bool SkipReloadTurn = false;
        private static int critCount = 0;
        private static int resetCount = 0;

        private OrbofGreed() : base("orbofgreed"){
            LocalVariables = true;
        }

        public ConfigEntry<bool> EnabledConfig { internal set; get; }
        public override bool IsEnabled()
        {
            if (EnabledConfig == null)
            {
                EnabledConfig = Plugin.ConfigFile.Bind<bool>("Custom Orbs", GetName(), true, "Disable to remove from orb pool");
            }

            return EnabledConfig.Value;
        }

        public static OrbofGreed GetInstance()
        {
            if (_instance == null)
                _instance = new OrbofGreed();
            return _instance;
        }

        public override void SetLocalVariables(LocalizationParamsManager localParams, GameObject orb, Attack attack)
        {
            localParams.SetParameterValue(ParamKeys.DISCARD_DAMAGE, $"{GetDiscardDamage(attack)}");
        }

        public float GetDiscardDamage(Attack attack)
        {
            return GetDiscardDamage(attack.Level);
        }

        public static float GetDiscardDamage(int level)
        {
            if (level == 2)
                return 1f;
            if (level == 3)
                return 3f;
            return 0f;
        }

        public override void CreatePrefabs()
        {

            GameObject shotPrefab = new CustomShotBuilder()
                .SetName("orbofgreedThrow")
                .SetSprite(Plugin.OrbOfGreedAttack)
                .Build();

            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("OrbOfGreed")
                .SetSprite(Plugin.OrbOfGreed)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .SetDamage(1, 1)
                .SetLevel(1)
                .SetDescription(new string[] { "shuffle_deck_on_discard", "remove_on_discard" })
                .AddParameter(ParamKeys.DISCARD_DAMAGE, $"{GetDiscardDamage(1)}")
                .IncludeInOrbPool(true)
                .SetShot(shotPrefab);

            CustomOrbBuilder levelTwo = levelOne
                .Clone()
                .SetLevel(2)
                .IncludeInOrbPool(false)
                .SetDescription(new string[] { "shuffle_deck_on_discard", "skip_enemy_turn_on_discard", "damage_self_on_discard", "remove_on_discard" })
                .AddParameter(ParamKeys.DISCARD_DAMAGE, $"{GetDiscardDamage(2)}");

            CustomOrbBuilder levelThree = levelTwo
                .Clone()
                .SetLevel(3)
                .SetDescription(new string[] { "add_crit_refresh_on_discard", "shuffle_deck_on_discard", "skip_enemy_turn_on_discard", "damage_self_on_discard", "remove_on_discard" })
                .AddParameter(ParamKeys.DISCARD_DAMAGE, $"{GetDiscardDamage(3)}");

            GameObject one = levelOne.Build();
            GameObject two = levelTwo.Build();
            GameObject three = levelThree.Build();

            CustomOrbBuilder.JoinLevels(one, two, three);

            Prefabs[1] = one;
            Prefabs[2] = two;
            Prefabs[3] = three;
        }

        public override void OnDiscard(RelicManager relicManager, BattleController battleController, GameObject orb, Attack attack)
        {
            battleController._deckManager.RemoveOrbFromBattleDeck(orb);
            battleController._deckManager.shuffledDeck.Clear();

            if(attack.Level > 1)
            {
                SkipReloadTurn = true;
                battleController._playerHealthController.Damage(GetDiscardDamage(attack));
            }

            if(attack.Level > 2)
            {
                Peg peg1 = PegManager.CreateSpecialPeg(battleController.pegManager._nonSpecialPegs, Peg.PegType.CRIT);
                Peg peg2 = PegManager.CreateSpecialPeg(battleController.pegManager._nonSpecialPegs, Peg.PegType.RESET);

                battleController.pegManager._critPegs.Add(peg1);
                battleController.pegManager._resetPegs.Add(peg2);

                critCount++;
                resetCount++;
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.AttemptOrbDiscard))]
        public static class NoFreeTurn
        {
            [HarmonyPriority(Priority.HigherThanNormal)]
            private static void Postfix(BattleController __instance)
            {
                if (SkipReloadTurn)
                {
                    SkipReloadTurn = false;
                    __instance._skipPlayersTurn = false;
                }
            }
        }

        [HarmonyPatch(typeof(PegManager), nameof(PegManager.GetPegCount))]
        public static class AddPegs
        {
            private static void Postfix(Peg.PegType type, ref int __result)
            {
                if (type == Peg.PegType.CRIT) __result += critCount;
                else if (type == Peg.PegType.RESET) __result += resetCount;
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.EndBattle))]
        public static class ResetPegCount
        {
            public static void Prefix()
            {
                critCount = 0;
                resetCount = 0;
            }
        }
    }
}
