using Battle;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private OrbofGreed() : base("orbofgreed"){ }

        public static OrbofGreed GetInstance()
        {
            if (_instance == null)
                _instance = new OrbofGreed();
            return _instance;
        }
        public override void CreatePrefabs()
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Orbs/StoneOrb-Lvl1");
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
            attackTwo.locDescStrings = new string[] { "shuffle_deck_on_discard", "skip_enemy_turn_on_discard", "remove_on_discard"};
            attackTwo.Level = 2;
            

            _levelThree = GameObject.Instantiate(_levelTwo);
            _levelThree.name = "OrbOfGreed-Lvl3";
            Attack attackThree = _levelThree.GetComponent<Attack>();
            attackThree.Level = 3;
            attackThree.locDescStrings = new string[] {"add_crit_refresh_on_discard", "shuffle_deck_on_discard", "skip_enemy_turn_on_discard", "remove_on_discard" };

            attackOne.NextLevelPrefab = _levelTwo;
            attackTwo.NextLevelPrefab = _levelThree;
            attackThree.NextLevelPrefab = null;


            foreach(GameObject obj in new GameObject[] {_levelOne, _levelTwo, _levelThree, shotPrefab})
            {
                GameObject.DontDestroyOnLoad(obj);
                obj.hideFlags = HideFlags.HideAndDontSave;
                obj.SetActive(false);
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

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.StartAttacking))]
        public static class NoFreeTurn
        {
            [HarmonyPriority(Priority.HigherThanNormal)]
            private static bool Prefix(BattleController __instance)
            {
                if (SkipReloadTurn)
                {
                    SkipReloadTurn = false;
                    return false;
                }
                return true;
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
