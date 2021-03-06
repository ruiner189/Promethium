using HarmonyLib;
using PeglinUI;
using PeglinUI.OrbDisplay;
using Promethium.Patches.Relics;
using Promethium.Extensions;
using Relics;
using System;
using UnityEngine;
using Battle.Pachinko;

namespace Promethium.Patches.Mechanics
{
    public static class Hold
    {
        private static GameObject _heldObject;
        private static GameObject _heldInfo;
        private static GameObject _heldDeckObject;
        private static int _heldPersists = -1;
        private static long _lastDraw = 0;

        public static GameObject HeldOrb => _heldObject;

        [HarmonyPatch(typeof(BattleController), "Start")]
        public static class OnStart
        {
            public static void Prefix(RelicManager ____relicManager)
            {
                _heldObject = null;
                _heldInfo = null;
                _heldDeckObject = null;
                _heldPersists = -1;

                if (____relicManager != null && ____relicManager.AttemptUseRelic(CustomRelicEffect.HOLSTER))
                {
                    GameObject mask = GameObject.Find("OrbCountMask");
                    GameObject maskClone = GameObject.Instantiate(mask);
                    maskClone.transform.SetParent(mask.transform.parent);
                    maskClone.transform.localScale = new Vector3(0.6f, 0.6f, 1);
                    maskClone.transform.position = new Vector3(-8.4f, 4.6f, -0.1f);
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), "AttemptOrbDiscard")]
        public static class OnDiscard
        {
            public static bool Prefix(BattleController __instance, RelicManager ____relicManager, DeckManager ____deckManager, int ____battleState, ref bool ___currentBallIsPersistBonusOrb, ref GameObject ____ball)
            {
                if (____relicManager == null || ____deckManager == null || ____battleState == 9) return true;
                if (_heldObject != null && ____relicManager.AttemptUseRelic(RelicEffect.NO_DISCARD)) return false;
                if (____relicManager.AttemptUseRelic(CustomRelicEffect.HOLSTER) && ____ball != null && ____ball.GetComponent<PachinkoBall>().available && !DeckInfoManager.populatingDisplayOrb && !GameBlockingWindow.windowOpen)
                {
                    long elapsed = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastDraw;
                    if (DeckInfoManager.populatingDisplayOrb || elapsed < 750)
                    {
                        return false;
                    }
                    _lastDraw = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    ____ball.SetActive(false);

                    DeckInfoManager info = GameObject.Find("OrbDisplay").GetComponent<DeckInfoManager>();
                    GameObject displayOrb = info._currentOrb;
                    ShotDetailsWidget widget = GameObject.Find("OrbDetails").GetComponent<ShotDetailsWidget>();

                    if (_heldObject != null)
                    {
                        //Swap pointers
                        GameObject oldBall = ____ball;
                        ____ball = _heldObject;
                        _heldObject = oldBall;


                        // And again for persist
                        PersistentOrb newPersist = ____ball.GetComponent<PersistentOrb>();
                        PersistentOrb oldPersist = _heldObject.GetComponent<PersistentOrb>();
                        int persist = _heldPersists;
                        ___currentBallIsPersistBonusOrb = false;

                        // And the deck object...
                        GameObject oldDeckObject = _heldDeckObject;

                        if (oldPersist != null)
                        {
                            _heldPersists = oldPersist.remainingPersistence;
                            if (_heldPersists >= 0)
                                _heldDeckObject = ____deckManager.shuffledDeck.Pop();
                        }

                        if (newPersist != null)
                        {
                            BattleController.currentBallPersistLevel = persist;

                            if (newPersist.remainingPersistence < newPersist.modifiedPersistLevel - 1)
                            {
                                ___currentBallIsPersistBonusOrb = true;
                            }

                            info.UpdatePersistentInfo(persist);

                            newPersist.remainingPersistence = persist;
                            if (persist >= 0)
                            {
                                if (oldDeckObject != null)
                                    ____deckManager.shuffledDeck.Push(oldDeckObject);
                            }
                        }

                        ____ball.SetActive(true);

                        info._nextOrb = _heldInfo;
                        _heldInfo = CreateShotDetails(info, GameObject.Find("OrbDisplay").transform, _heldObject);
                        AddOrbToBattleDeck(____deckManager, ____ball);
                        ____deckManager.RemoveOrbFromBattleDeck(_heldObject);

                        info._currentOrb.SetActive(false);
                        __instance.InitializeAttack(____ball);
                        Action onBallCreationComplete = BattleController.OnBallCreationComplete;
                        if (onBallCreationComplete != null)
                        {
                            onBallCreationComplete();
                        }

                        info.BallDrawFinished();
                        return false;
                    }
                    else
                    {
                        _heldObject = ____ball;
                        _heldObject.SetActive(false);
                        _heldInfo = CreateShotDetails(info, GameObject.Find("OrbDisplay").transform, _heldObject);
                        GameObject.Destroy(info._currentOrb);
                        ____deckManager.RemoveOrbFromBattleDeck(_heldObject);
                        ____ball = null;
                        if (____deckManager.shuffledDeck.Count == 0)
                        {
                            __instance.ShuffleDeck();
                        }
                        else
                        {
                            PersistentOrb persist = _heldObject.GetComponent<PersistentOrb>();
                            if (persist != null)
                            {
                                _heldPersists = persist.remainingPersistence;
                                if (____deckManager.shuffledDeck.Count > 0 && persist.remainingPersistence >= 0 && persist.remainingPersistence < persist.modifiedPersistLevel)
                                {
                                    _heldDeckObject = ____deckManager.shuffledDeck.Pop();
                                }
                            }
                            __instance.DrawBall();
                        }
                        return false;
                    }
                }
                return true;
            }


            private static GameObject CreateShotDetails(DeckInfoManager manager, Transform parent, GameObject ball)
            {
                GameObject gameObject = manager.CreatePreviewSprite(ball, 0.0f);
                gameObject.transform.SetParent(parent);
                gameObject.transform.position = new Vector3(-8.4f, 4.6f, gameObject.transform.position.z);
                return gameObject;
            }

            private static void AddOrbToBattleDeck(DeckManager manager, GameObject orb)
            {
                GameObject container = manager.battleDeckOrbContainerInstance.gameObject;
                foreach (Transform child in container.transform)
                {
                    GameObject gameObject = child.gameObject;
                    Attack component = gameObject.GetComponent<Attack>();
                    Attack component2 = orb.GetComponent<Attack>();
                    if (Attack.IsEquivalent(component, component2))
                    {
                        manager.battleDeck.Add(gameObject);
                        break;
                    }
                }
            }
        }
    }
}
