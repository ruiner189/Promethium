using Battle;
using BepInEx.Configuration;
using ProLib.Orbs;
using HarmonyLib;
using I2.Loc;
using Promethium.Extensions;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.CustomOrbs
{
    public sealed class OrbofGreed : CustomOrb
    {
        private static OrbofGreed _instance;
        private static GameObject _levelOne;
        private static GameObject _levelTwo;
        private static GameObject _levelThree;

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
            if (attack.Level == 2)
                return 1f;
            if (attack.Level == 3)
                return 3f;
            return 0f;
        }

        public override void CreatePrefabs()
        {
            GameObject prefab = Resources.Load<GameObject>("$Prefabs/Orbs/StoneOrb-Lvl1");
            _levelOne = GameObject.Instantiate(prefab);
            _levelOne.name = "OrbOfGreed-Lvl1";
            GameObject sprite = _levelOne.transform.GetChild(0).gameObject;
            SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
            renderer.sprite = Plugin.OrbOfGreed;
            sprite.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            FireballAttack attackOne = _levelOne.GetComponent<FireballAttack>();
            attackOne.locName = "orbofgreed";
            attackOne.locNameString = "orbofgreed";
            attackOne.DamagePerPeg = 1;
            attackOne.CritDamagePerPeg = 1;
            attackOne.locDescStrings = new string[] { "shuffle_deck_on_discard", "remove_on_discard" };
            attackOne.Level = 1;

            GameObject shotPrefab = GameObject.Instantiate(attackOne._shotPrefab);
            shotPrefab.name = "orbofgreedThrow";
            shotPrefab.GetComponent<SpriteRenderer>().sprite = Plugin.OrbOfGreedAttack;
            attackOne._shotPrefab = shotPrefab;
            attackOne._criticalShotPrefab = shotPrefab;

            _levelTwo = GameObject.Instantiate(_levelOne);
            _levelTwo.name = "OrbOfGreed-Lvl2";
            Attack attackTwo = _levelTwo.GetComponent<Attack>();
            attackTwo.locDescStrings = new string[] { "shuffle_deck_on_discard", "skip_enemy_turn_on_discard", "damage_self_on_discard", "remove_on_discard"};
            attackTwo.Level = 2;
            

            _levelThree = GameObject.Instantiate(_levelTwo);
            _levelThree.name = "OrbOfGreed-Lvl3";
            Attack attackThree = _levelThree.GetComponent<Attack>();
            attackThree.Level = 3;
            attackThree.locDescStrings = new string[] {"add_crit_refresh_on_discard", "shuffle_deck_on_discard", "skip_enemy_turn_on_discard", "damage_self_on_discard", "remove_on_discard" };

            attackOne.NextLevelPrefab = _levelTwo;
            attackTwo.NextLevelPrefab = _levelThree;
            attackThree.NextLevelPrefab = null;


            foreach(GameObject obj in new GameObject[] {_levelOne, _levelTwo, _levelThree, shotPrefab})
            {
                obj.transform.SetParent(Plugin.PromethiumPrefabHolder.transform);
                obj.HideAndDontSave();
            }
        }

        public override GameObject GetPrefab(int level)
        {
            if (_levelOne == null || _levelTwo == null || _levelThree == null) CreatePrefabs();

            if (level == 1) return _levelOne;
            else if (level == 2) return _levelTwo;
            else if (level == 3) return _levelThree;

            return null;
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
