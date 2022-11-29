using HarmonyLib;
using PeglinUI;
using PeglinUI.OrbDisplay;
using Relics;
using System;
using UnityEngine;
using Battle.Pachinko;
using UI.OrbDisplay;
using Promethium.UI;
using System.Linq;
using ProLib.Attributes;
using ProLib.Loaders;
using ProLib.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Battle.Attacks;

namespace Promethium.Patches.Mechanics
{
    [SceneModifier]
    public static class Hold
    {
        private static GameObject _heldObject;
        private static GameObject _heldInfo;
        private static GameObject _heldDeckObject;
        private static int _heldPersists = -1;
        private static long _lastDraw = 0;

        public static GameObject HeldOrb => _heldObject;

        public static void LateOnSceneLoaded(String scene, bool firstLoad)
        {
            if(scene == SceneLoader.Battle)
            {
                _heldObject = null;
                _heldInfo = null;
                _heldDeckObject = null;
                _heldPersists = -1;

                RelicManager relicManager = Resources.FindObjectsOfTypeAll<RelicManager>().FirstOrDefault();

                if (relicManager != null && CustomRelicManager.AttemptUseRelic(RelicNames.HOLSTER))
                {
                    GameObject mask = GameObject.Find("OrbCountMask");
                    GameObject maskClone = GameObject.Instantiate(mask);
                    maskClone.transform.SetParent(mask.transform.parent);
                    maskClone.transform.localScale = new Vector3(0.35f, 0.35f, 1);
                    maskClone.transform.position = new Vector3(-8.4f, 4.6f, -0.1f);
                }
            }
        }

        [HarmonyPatch(typeof(OrbDiscardButton), nameof(OrbDiscardButton.Update))]
        public static class HoldDiscard
        {
            private static float _holdTime;
            private static float _targetTime = 1;
            private static bool _complete = false;
            private static bool _startPress = false;

            public static bool Prefix(OrbDiscardButton __instance)
            {
                BattleController controller = Resources.FindObjectsOfTypeAll<BattleController>().FirstOrDefault();
                if(controller != null)
                {
                    if (!Plugin.HoldDiscard && !CustomRelicManager.RelicActive(RelicNames.HOLSTER))
                        return true;
                }

                RadialBar bar = RadialBar.GarbageBar;
                if (__instance._player.GetButton("Back") && !PauseMenu.Paused && !GameBlockingWindow.windowOpen)
                {
                    _startPress = true;
                    _holdTime += Time.deltaTime;

                    if (__instance.CanDiscard() && !_complete)
                    {
                        if (bar != null)
                        {
                            bar.gameObject.SetActive(true);
                            bar.FillPercent = _holdTime / _targetTime;
                        }

                        if (_holdTime >= _targetTime)
                        {
                            _complete = true;
                            bar.gameObject.SetActive(false);
                            OrbDiscardButton.OnOrbDiscardButtonClicked();
                        }
                    }
                } 
                else
                {
                    if (bar != null)
                    {
                        bar.gameObject.SetActive(false);
                    }

                    if (_startPress && !_complete)
                    {
                        HoldOrb(controller);
                    }

                    _complete = false;
                    _startPress = false;
                    _holdTime = 0f;
                }

                if (__instance._cachedCanDiscard != __instance.CanDiscard())
                {
                    __instance._cachedCanDiscard = __instance.CanDiscard();
                    __instance.gamepadPrompt.SetActive(__instance._cachedCanDiscard);
                }
                return false;
            }
        }

        public static void HoldOrb(BattleController battleController)
        {
            RelicManager relicManager = battleController._relicManager;
            DeckManager deckManager = battleController._deckManager;
            ref GameObject ball = ref battleController._activePachinkoBall;
            ref bool currentBallIsPersistBonusOrb = ref battleController.currentBallIsPersistBonusOrb;

            if (relicManager == null || deckManager == null || BattleController._battleState == BattleController.BattleState.NAVIGATION) return;
            if (_heldObject != null && relicManager.AttemptUseRelic(RelicEffect.NO_DISCARD)) return;
            if (CustomRelicManager.RelicActive(RelicNames.HOLSTER) && ball != null && ball.GetComponent<PachinkoBall>().available && !DeckInfoManager.populatingDisplayOrb && !GameBlockingWindow.windowOpen)
            {
                CustomRelicManager.AttemptUseRelic(RelicNames.HOLSTER);

                long elapsed = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastDraw;
                if (DeckInfoManager.populatingDisplayOrb || elapsed < 750)
                {
                    return;
                }
                _lastDraw = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                ball.SetActive(false);

                DeckInfoManager info = GameObject.Find("OrbDisplay").GetComponent<DeckInfoManager>();
                GameObject displayOrb = info._currentOrb;
                ShotDetailsWidget widget = GameObject.Find("OrbDetails").GetComponent<ShotDetailsWidget>();

                if (_heldObject != null)
                {
                    //Swap pointers
                    GameObject oldBall = ball;
                    ball = _heldObject;
                    _heldObject = oldBall;


                    // And again for persist
                    PersistentOrb newPersist = ball.GetComponent<PersistentOrb>();
                    PersistentOrb oldPersist = _heldObject.GetComponent<PersistentOrb>();
                    int persist = _heldPersists;
                    currentBallIsPersistBonusOrb = false;

                    // And the deck object...
                    GameObject oldDeckObject = _heldDeckObject;

                    if (oldPersist != null)
                    {
                        _heldPersists = oldPersist.remainingPersistence;
                        if (_heldPersists >= 0)
                            _heldDeckObject = deckManager.shuffledDeck.Pop();
                    }

                    if (newPersist != null)
                    {
                        BattleController.currentBallPersistLevel = persist;

                        if (newPersist.remainingPersistence < newPersist.modifiedPersistLevel - 1)
                        {
                            currentBallIsPersistBonusOrb = true;
                        }

                        info.UpdatePersistentInfo(persist);

                        newPersist.remainingPersistence = persist;
                        if (persist >= 0)
                        {
                            if (oldDeckObject != null)
                                deckManager.shuffledDeck.Push(oldDeckObject);
                        }
                    }

                    ball.SetActive(true);

                    info._nextOrb = _heldInfo;
                    _heldInfo = CreateShotDetails(info, GameObject.Find("OrbDisplay").transform, _heldObject);
                    AddOrbToBattleDeck(deckManager, ball);
                    deckManager.RemoveOrbFromBattleDeck(_heldObject);

                    info._currentOrb.SetActive(false);
                    battleController.InitializeAttack(ball);
                    Action onBallCreationComplete = BattleController.OnBallCreationComplete;
                    if (onBallCreationComplete != null)
                    {
                        onBallCreationComplete();
                    }

                    info.BallDrawFinished();
                    return;
                }
                else
                {
                    _heldObject = ball;
                    _heldObject.SetActive(false);
                    _heldInfo = CreateShotDetails(info, GameObject.Find("OrbDisplay").transform, _heldObject);
                    GameObject.Destroy(info._currentOrb);
                    deckManager.RemoveOrbFromBattleDeck(_heldObject);
                    ball = null;
                    if (deckManager.shuffledDeck.Count == 0)
                    {
                        battleController.ShuffleDeck();
                    }
                    else
                    {
                        PersistentOrb persist = _heldObject.GetComponent<PersistentOrb>();
                        if (persist != null)
                        {
                            _heldPersists = persist.remainingPersistence;
                            if (deckManager.shuffledDeck.Count > 0 && persist.remainingPersistence >= 0 && persist.remainingPersistence < persist.modifiedPersistLevel)
                            {
                                _heldDeckObject = deckManager.shuffledDeck.Pop();
                            }
                        }
                        battleController.DrawBall();
                    }
                }
            }
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
