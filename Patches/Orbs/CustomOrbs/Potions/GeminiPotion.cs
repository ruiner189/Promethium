using Battle.Attacks;
using Currency;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using ProLib.Orbs;
using Promethium.Patches.Mechanics;
using Promethium.Patches.Orbs.Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleController;

namespace Promethium.Patches.Orbs.CustomOrbs.Potions
{
    public sealed class GeminiPotion : Potion
    {
        private static GeminiPotion _instance;
        private GeminiPotion() : base("geminipotion")
        {
            ShouldSkip = true;
        }

        public static GeminiPotion GetInstance()
        {
            if (_instance == null)
                _instance = new GeminiPotion();
            return _instance;
        }

        public static void Register()
        {
            GetInstance();
        }

        public override void CreatePrefabs()
        {
            CustomOrbBuilder levelOne = new CustomOrbBuilder()
                .SetName("GeminiPotion")
                .SetDescription("potion_clone", "potion_no_clone", "potion_instant", "once_per_battle")
                .AddParameter(ParamKeys.CLONE_AMOUNT, "2")
                .SetRarity(PachinkoBall.OrbRarity.RARE)
                .SetSprite(Plugin.GeminiPotion)
                .SetSpriteScale(new Vector3(0.6f, 0.6f, 1f))
                .IncludeInOrbPool(true)
                .WithAttack<PotionAttack>();

            this[1] = levelOne.Build();

            PotionAttack attack = this[1].GetComponent<PotionAttack>();
            attack.AffectOtherPotions = true;
            attack.Temporary = false;
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            base.OnShotFired(battleController, orb, attack);
            int amountOfClones = 2;
            DeckManager deckManager = battleController._deckManager;
            DeckInfoManager info = Resources.FindObjectsOfTypeAll<DeckInfoManager>().FirstOrDefault();
            if (deckManager.shuffledDeck.Count != 0)
            {
                GameObject next = deckManager.shuffledDeck.Peek();

                Attack nextAttack = next.GetComponent<Attack>();

                if(nextAttack.locNameString != attack.locNameString)
                {
                    deckManager.RemoveOrbFromBattleDeck(orb);
                    for (int i = 0; i < amountOfClones; i++)
                    {
                        deckManager.PushOrbToShuffleDeck(next, false);
                        MoveNextOrbToLast(deckManager);
                    }

                    info._currentOrb.SetActive(false);
                    info.StartShuffleAnimation(deckManager.shuffledDeck.Count);
                    battleController.StartCoroutine(WaitShuffleComplete());
                }
            }
        }

        public void MoveNextOrbToLast(DeckManager deckManager)
        {
            Stack <GameObject> stack = deckManager.shuffledDeck;
            GameObject next = stack.Pop();
            List<GameObject> list = stack.ToList();
            list.Insert(list.Count, next);

            stack.Clear();
            for(int i = list.Count - 1; i >= 0; i--)
            {
                stack.Push(list[i]);
            }
        }

        public IEnumerator WaitShuffleComplete()
        {
            BattleController._battleState = BattleState.AWAITING_SHOT_COMPLETION;
            yield return new WaitUntil(() => !DeckInfoManager.animating);
            UsedPotion = true;
            BattleController._battleState = BattleState.AWAITING_ENEMY_CLEANUP;
        }
    }
}
