using HarmonyLib;
using PeglinUI;
using PeglinUI.OrbDisplay;
using Relics;
using System;
using System.Collections.Generic;
using UnityEngine;
using Battle.Pachinko;
using UI.OrbDisplay;
using Promethium.UI;
using System.Linq;
using ProLib.Attributes;
using ProLib.Managers;
using ProLib.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Battle.Attacks;
using DG.Tweening.Core;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using ProLib.Orbs;
using Promethium.Patches.Orbs.Attacks;

using static BattleController;
using Promethium.Patches.Mechanics;

namespace Promethium.Components.Managers
{
    [SceneModifier]
    [HarmonyPatch]
    public class HoldManager : MonoBehaviour
    {
        public static HoldManager Instance;

        // Slots
        private Slot _holdSlot = new Slot();
        private Slot _slotOne = new Slot();
        private Slot _slotTwo = new Slot();
        private Slot _slotThree = new Slot();

        // Animations
        public static bool Swapping;

        // Hold for discard
        private float _holdTime;
        private float _targetTime = 1;
        private bool _complete = false;
        private bool _startPress = false;
        private long _lastDraw = 0;

        public GameObject HeldOrb => _holdSlot.Orb;

        public void Awake()
        {
            if (Instance == null) Instance = this;
            if (this != Instance) Destroy(this);
        }

        public static void LateOnSceneLoaded(String scene, bool firstLoad)
        {
            if(scene == SceneInfoManager.Battle)
            {
                Instance?.CreateSlots();
            }
        }

        public void CreateSlots()
        {
            _holdSlot.Reset();
            _slotOne.Reset();
            _slotTwo.Reset();
            _slotThree.Reset();

            GameObject mask = GameObject.Find("OrbCountMask");
            GameObject levelRing = GameObject.Find("OrbLevelRing");


            CreateSlot(mask, new Vector3(-8.6f, 4.6f, -0.1f), out _holdSlot.Container);
            CreateSlot(mask, new Vector3(-10.55f, 1f, -0.1f), out _slotOne.Container);
            CreateSlot(mask, new Vector3(-10.55f, 0f, -0.1f), out _slotTwo.Container);
            CreateSlot(mask, new Vector3(-10.55f, -1f, -0.1f), out _slotThree.Container);

            mask.transform.localScale *= new Vector2(0.8f, 0.8f);
            mask.transform.localPosition -= new Vector3(0, 0.3f);
            levelRing.transform.localScale *= new Vector2(0.8f, 0.8f);
            levelRing.transform.localPosition -= new Vector3(0, 0.3f);
        }

        public GameObject[] GetPotions()
        {
            List<GameObject> objects = new List<GameObject>();
            foreach(GameObject obj in new GameObject[] {_slotOne.Orb, _slotTwo.Orb, _slotThree.Orb })
            {
                if (obj != null) objects.Add(obj);
            }
            return objects.ToArray();
        }

        public void RemovePotion(GameObject potion)
        {
            foreach(Slot slot in new Slot[] {_slotOne, _slotTwo, _slotThree })
            {
                if(slot.Orb == potion)
                {
                    GameObject.Destroy(slot.DeckObject);
                    GameObject.Destroy(slot.Details);
                    slot.Orb = null;
                    slot.DeckObject = null;
                    slot.Details = null;
                }
            }
        }

        public void UpdatePotionInfo()
        {
            foreach (Slot slot in new Slot[] { _slotOne, _slotTwo, _slotThree })
            {
                if(slot.Details != null && slot.Orb != null)
                {
                    UpcomingOrbDisplay display = slot.Details.GetComponentInChildren<UpcomingOrbDisplay>();
                    PotionAttack attack = slot.Orb.GetComponent<PotionAttack>();
                    if(display != null)
                    {
                        display.UpdatePersistentOrb(attack.Duration-2);
                    }
                }
            }
        }

        public bool SlotAvailable()
        {
            if (_slotOne.Available()) return true;
            if (_slotTwo.Available()) return true;
            if (_slotThree.Available()) return true;

            return false;
        }

        public void AddPotion(BattleController battleController, GameObject potion)
        {

            Slot slot = null;
            if (_slotOne.Available()) slot = _slotOne;
            else if (_slotTwo.Available()) slot = _slotTwo;
            else if (_slotThree.Available()) slot = _slotThree;

            if(slot != null)
            {
                DeckInfoManager info = GameObject.Find("OrbDisplay").GetComponent<DeckInfoManager>();
                DeckManager deckManager = battleController._deckManager;

                slot.Orb = potion;
                potion.SetActive(false);
                slot.Details = CreateShotDetails(info, GameObject.Find("OrbDisplay").transform,slot. Orb.GetComponent<Attack>(), slot);
                if (deckManager.shuffledDeck.Count != 0)
                {
                    PersistentOrb persist = slot.Orb.GetComponent<PersistentOrb>();
                    if (persist != null)
                    {
                        if (deckManager.shuffledDeck.Count > 0 && persist.remainingPersistence >= 0 && persist.remainingPersistence < persist.modifiedPersistLevel)
                        {
                            slot.DeckObject = deckManager.shuffledDeck.Pop();
                        }
                    }
                }
                UpdatePotionInfo();
            }
        }

        public void CreateSlot(GameObject original, Vector3 position, out GameObject result)
        {
            result = GameObject.Instantiate(original);
            result.transform.SetParent(original.transform.parent);
            result.transform.localScale = new Vector3(0.42f, 0.42f, 1f);
            result.transform.position = position;
        }

        public void HoldOrb(BattleController battleController)
        {
            Swapping = true;
            RelicManager relicManager = battleController._relicManager;
            DeckManager deckManager = battleController._deckManager;
            ref GameObject ball = ref battleController._activePachinkoBall;
            ref bool currentBallIsPersistBonusOrb = ref battleController.currentBallIsPersistBonusOrb;

            if (relicManager == null || deckManager == null || BattleController._battleState == BattleController.BattleState.NAVIGATION) return;
            if (_holdSlot.Orb != null && relicManager.AttemptUseRelic(RelicEffect.NO_DISCARD)) return;
            if (ball != null && ball.GetComponent<PachinkoBall>().available && !DeckInfoManager.populatingDisplayOrb && !GameBlockingWindow.windowOpen)
            {
                CustomRelicManager.Instance.AttemptUseRelic(RelicNames.HOLSTER);

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
                UpcomingOrbDisplay upcoming = info._nextOrb.GetComponentInChildren<UpcomingOrbDisplay>();

                if (_holdSlot.Orb != null)
                {
                    //Swap pointers
                    GameObject oldBall = ball;
                    ball = _holdSlot.Orb;
                    _holdSlot.Orb = oldBall;


                    // And again for persist
                    PersistentOrb newPersist = ball.GetComponent<PersistentOrb>();
                    PersistentOrb oldPersist = _holdSlot.Orb.GetComponent<PersistentOrb>();
                    int persist = _holdSlot.Persists;
                    currentBallIsPersistBonusOrb = false;

                    // And the deck object...
                    GameObject oldDeckObject = _holdSlot.DeckObject;

                    if (oldPersist != null)
                    {
                        _holdSlot.Persists = oldPersist.remainingPersistence;
                        if (_holdSlot.Persists >= 0)
                            _holdSlot.DeckObject = deckManager.shuffledDeck.Pop();
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

                    info._nextOrb = _holdSlot.Details;
                    _holdSlot.Details = CreateShotDetails(info, GameObject.Find("OrbDisplay").transform, upcoming.attack, _holdSlot);
                    AddOrbToBattleDeck(deckManager, ball);
                    deckManager.RemoveOrbFromBattleDeck(_holdSlot.Orb);

                    info._currentOrb.SetActive(false);
                    battleController.InitializeAttack(ball);
                    Action onBallCreationComplete = BattleController.OnBallCreationComplete;
                    if (onBallCreationComplete != null)
                    {
                        onBallCreationComplete();
                    }

                    int frame = Mathf.Clamp(upcoming.attack.Level - 1, 0, 2);
                    info._currentOrbLevelRingRenderer.sprite = info._orbLevelDisplaySprites[frame];
                    upcoming.mainOrbLevelFrameMask.SetActive(true);

                    info.BallDrawFinished();
                }
                else
                {
                    _holdSlot.Orb = ball;
                    _holdSlot.Orb.SetActive(false);
                    _holdSlot.Details = CreateShotDetails(info, GameObject.Find("OrbDisplay").transform, upcoming.attack, _holdSlot);
                    info._currentOrb.SetActive(false);
                    deckManager.RemoveOrbFromBattleDeck(_holdSlot.Orb);
                    ball = null;
                    if (deckManager.shuffledDeck.Count == 0)
                    {
                        battleController.ShuffleDeck();
                    }
                    else
                    {
                        PersistentOrb persist = _holdSlot.Orb.GetComponent<PersistentOrb>();
                        if (persist != null)
                        {
                            _holdSlot.Persists = persist.remainingPersistence;
                            if (deckManager.shuffledDeck.Count > 0 && persist.remainingPersistence >= 0 && persist.remainingPersistence < persist.modifiedPersistLevel)
                            {
                                _holdSlot.DeckObject = deckManager.shuffledDeck.Pop();
                            }
                        }
                        battleController.DrawBall();
                    }
                }
            }
            Swapping = false;
        }

        private GameObject CreateShotDetails(DeckInfoManager manager, Transform parent, Attack attack, Slot slot)
        {
            GameObject gameObject = manager.CreatePreviewSprite(attack.gameObject, 0.0f);
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = slot.Container.transform.position;
            return gameObject;
        }

        private void AddOrbToBattleDeck(DeckManager manager, GameObject orb)
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


        #region Harmony Patches
        [HarmonyPatch(typeof(OrbDiscardButton), nameof(OrbDiscardButton.Update))]
        [HarmonyPrefix]
        public static bool PatchOrbDiscardUpdate(OrbDiscardButton __instance)
        {
            if (Instance == null || !Instance.isActiveAndEnabled) return true;
            BattleController controller = Resources.FindObjectsOfTypeAll<BattleController>().FirstOrDefault();

            RadialBar bar = RadialBar.GarbageBar;
            if (__instance._player.GetButton("Back") && !PauseMenu.Paused && !GameBlockingWindow.windowOpen)
            {
                Instance._startPress = true;
                Instance._holdTime += Time.deltaTime;

                if (__instance.CanDiscard() && !Instance._complete)
                {
                    if (bar != null)
                    {
                        bar.gameObject.SetActive(true);
                        bar.FillPercent = Instance._holdTime / Instance._targetTime;
                    }

                    if (Instance._holdTime >= Instance._targetTime)
                    {
                        Instance._complete = true;
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

                if (Instance._startPress && !Instance._complete)
                {
                    Instance.HoldOrb(controller);
                }

                Instance._complete = false;
                Instance._startPress = false;
                Instance._holdTime = 0f;
            }

            if (__instance._cachedCanDiscard != __instance.CanDiscard())
            {
                __instance._cachedCanDiscard = __instance.CanDiscard();
                __instance.gamepadPrompt.SetActive(__instance._cachedCanDiscard);
            }
            return false;
        }

        [HarmonyPatch(typeof(DeckInfoManager), nameof(DeckInfoManager.BallDrawFinished))]
        [HarmonyPrefix]
        private static bool PatchBallDrawFinished(DeckInfoManager __instance, bool __runOriginal)
        {
            if (!__runOriginal) return false;
            DeckInfoManager.onActiveOrbScaleStarted(__instance._nextOrb);
            UpcomingOrbDisplay componentInChildren = __instance._nextOrb.GetComponentInChildren<UpcomingOrbDisplay>();
            componentInChildren.StartTextCanvasFade(__instance.moveAndScaleActiveOrbTime * 0.5f);
            componentInChildren.PreSetActive();
            __instance._nextOrb.transform.DOMove(__instance._currentOrbDisplayPos.transform.position, __instance.moveAndScaleActiveOrbTime, false);
            TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = __instance._nextOrb.transform.DOScale(Vector3.one * DeckInfoManager.ACTIVE_ORB_DISPLAY_HEIGHT * 0.75f, __instance.moveAndScaleActiveOrbTime);
            tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(__instance.EndCurrentOrbScale));
            return false;
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.ShotFired))]
        [HarmonyPrefix]
        public static void Prefix(BattleController __instance)
        {
            if (BattleController._battleState == BattleState.NAVIGATION || Instance == null || !CustomRelicManager.Instance.AttemptUseRelic(RelicNames.HOLSTER)) return;
            if (Instance._holdSlot.Orb != null)
            {
                Attack attack = Instance._holdSlot.Orb.GetComponent<Attack>();
                if (attack != null)
                {
                    List<ModifiedOrb> orbs = ModifiedOrb.GetOrbs(attack.locNameString);
                    foreach (ModifiedOrb orb in orbs)
                    {
                        orb.ShotWhileInHolster(__instance._relicManager, __instance, __instance._activePachinkoBall, Instance._holdSlot.Orb);
                    }
                }
            }
        }

        #endregion
    }
}
