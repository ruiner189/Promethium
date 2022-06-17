using Cruciball;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using Promethium.Extensions;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Worldmap;
using Random = System.Random;

namespace Promethium.Patches.Mechanics
{

    public static class CurseRun
    {
        public static bool Victory = false;
        public static bool LoadElite = false;

        private static List<Relic> _oldRelics;
        private static int _commonRelics = 3;
        private static int _rareRelics = 3;
        private static int _bossRelics = 3;

        // Block other mods from modifiying the deck when we are keeping/pruning it.
        public static bool BlockDeckInit = false;


        public static void PruneDeck(DeckManager manager)
        {
            if (Plugin.PruneOrbsOnNewCurseRunOn)
            {
                int amountToKeep = 4;
                while (DeckManager.completeDeck != null && DeckManager.completeDeck.Count > amountToKeep)
                {
                    manager.RemoveRandomOrbFromDeck();
                }
            }
        }

        public static void PruneRelics(RelicManager manager)
        {
            Dictionary<RelicEffect, Relic> relicDict = manager._ownedRelics;
            List<Relic> relics = relicDict.Values.ToList();

            Random rand = new Random();
            List<Relic> curses = relics.FindAll(relic => CurseRelic.AllCurseRelics.Contains(relic));
            _oldRelics = relics.FindAll(relic => !curses.Contains(relic)).OrderBy(relic => rand.Next()).ToList();

            _commonRelics = 3;
            _rareRelics = 2;
            _bossRelics = 1;

            if (CurseRelic.IsCurseLevelActive(manager, 1))
            {
                _commonRelics++;
                _rareRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(manager, 2))
            {
                _commonRelics+=2;
                _rareRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(manager, 3))
            {
                _rareRelics++;
                _bossRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(manager, 4))
            {
                _commonRelics+=2;
                _rareRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(manager, 5))
            {
                _commonRelics++;
                _rareRelics++;
                _bossRelics++;
            }

            manager.ResetSilently();
            curses.ForEach(manager.AddRelic);
        }

        public static bool HasAnotherRelic()
        {
            if (_oldRelics == null) return false;
            if (_commonRelics == 0 && _rareRelics == 0 && _bossRelics == 0) return false;
            if (_oldRelics.Count >= 3) return true;
            return false;
        }

        public static List<Relic> GetNextRelicSet(RelicManager relicManager)
        {
            if (_commonRelics == 0 && _rareRelics == 0 && _bossRelics == 0) 
                return new List<Relic>();
            if (_oldRelics.Count <= 3) 
                return _oldRelics;

            if(_commonRelics != 0)
            {
                _commonRelics--;
                return GetRelicsOfRarity(relicManager, 3, _oldRelics, RelicRarity.COMMON, 100);
            }

            if(_rareRelics != 0)
            {
                _rareRelics--;
                return GetRelicsOfRarity(relicManager, 3, _oldRelics, RelicRarity.RARE, 10);
            }

            if (_bossRelics != 0)
            {
                _bossRelics--;
                return GetRelicsOfRarity(relicManager, 3, _oldRelics, RelicRarity.BOSS);
            }

            return new List<Relic>();
        }

        public static void RemoveRelicFromList(Relic relic)
        {
            _oldRelics.Remove(relic);
        }

        public static List<Relic> GetCurseRelics(RelicManager manager)
        {
            if (CurseRelic.IsCurseLevelActive(manager, 5))
            {
                return new Relic[] { manager.consolationPrize, manager.consolationPrize, manager.consolationPrize }.ToList();
            }
            Dictionary<RelicEffect, Relic> relicDict = manager._ownedRelics;
            List<Relic> relics = relicDict.Values.ToList();

            int currentCurse = 0;
            foreach (Relic relic in relics)
            {
                if (relic is CurseRelic)
                    currentCurse++;
            }

            List<Relic> curses = new List<Relic>(CurseRelic.GetCurseRelicOfLevel(currentCurse + 1));

            while (curses.Count > 3) curses.RemoveAt(curses.Count - 1);
            return curses;
        }

        public static bool ShouldStartNextCurseRun()
        {
            return Victory && Plugin.CurseRunOn;
        }

        public static List<Relic> GetRelicsOfRarity(RelicManager manager, int amount, List<Relic> relics, RelicRarity rarity, int upgradeChance = 0)
        {
            if (relics.Count <= amount)
                return new List<Relic>(relics);

            List<Relic> list = new List<Relic>();
            List<Relic> pool = new List<Relic>(relics);

            for(int i = 0; i < amount; i++)
            {
                Relic relic = GetRelicOfRarity(manager, pool, rarity, upgradeChance);
                list.Add(relic);
                pool.Remove(relic);
            }
            return list;
        }

        public static Relic GetRelicOfRarity(RelicManager manager, List<Relic> relics, RelicRarity rarity, int upgradeChance = 0)
        {
            if (relics.Count == 0) return null;
            if (relics.Count == 1) return relics[0];
            if (upgradeChance > 0)
            {
                Random rand = new Random();
                if (rand.Next(0, 1000) <= upgradeChance)
                {
                    RelicRarity newRarity = RelicRarity.RARE;
                    if (rarity == RelicRarity.RARE) newRarity = RelicRarity.BOSS;
                    return GetRelicOfRarity(manager, relics, newRarity);
                }
            }

            RelicSet relicPool = null;
            if (rarity == RelicRarity.COMMON)
                relicPool = manager._commonRelicPool;
            else if (rarity == RelicRarity.RARE)
                relicPool = manager._rareRelicPool;
            else if (rarity == RelicRarity.BOSS)
                relicPool = manager._bossRelicPool;

            Relic relic = relics.Find(r => relicPool.relics.Contains(r));
            if (relic == null && rarity != RelicRarity.COMMON)
            {
                if (rarity == RelicRarity.RARE)
                    return GetRelicOfRarity(manager, relics, RelicRarity.COMMON);
                else if (rarity == RelicRarity.BOSS)
                    return GetRelicOfRarity(manager, relics, RelicRarity.RARE);
            }
            return relic;
        }

        public static bool LoadCurseRunData;
    }

    [HarmonyPatch(typeof(RestartOnClick), nameof(RestartOnClick.StartNewGameFromWinScreen))]
    public static class AbandonedRunCheck
    {
        public static void Prefix()
        {
            CurseRun.Victory = true;
        }
    }

    [HarmonyPatch(typeof(GameInit), nameof(GameInit.Start))]
    public static class GameInitPatch
    {
        [HarmonyPriority(Priority.Last)]
        public static bool Prefix(GameInit __instance, DeckManager ____deckManager,
            DeckData ____initialDeck, RelicManager ____relicManager, CruciballManager ____cruciballManager,
            GameObject ____chooseRelicCanvas, float ____chooseRelicCanvasFadeInTime,
            ref List<Relic> ____chosenRelics, RelicIcon[] ____chooseRelicIcons)
        {
            CanvasGroup component = GameObject.FindGameObjectWithTag("PersistentUI").GetComponent<CanvasGroup>();
            CurseRun.BlockDeckInit = false;

            if (CurseRun.HasAnotherRelic())
            {
                CurseRun.BlockDeckInit = true;
                ____chooseRelicCanvas.GetComponent<CanvasGroup>().DOFade(1f, ____chooseRelicCanvasFadeInTime);
                component.DOFade(1f, ____chooseRelicCanvasFadeInTime).From(0f, true, false);
                ____chosenRelics = CurseRun.GetNextRelicSet(____relicManager);
                while(____chosenRelics.Count == 0 && CurseRun.HasAnotherRelic())
                {
                    ____chosenRelics = CurseRun.GetNextRelicSet(____relicManager);
                }

                if(____chosenRelics.Count > 0)
                {
                    for (int j = 0; j < ____chosenRelics.Count; j++)
                    {
                        ____chooseRelicIcons[j].SetRelic(____chosenRelics[j]);
                    }
                    return false;
                }
            }

            CurseRun.LoadElite = false;
            if (CurseRun.ShouldStartNextCurseRun())
            {
                CurseRun.Victory = false;
                CurseRun.BlockDeckInit = true;
                if (CurseRelic.IsCurseLevelActive(____relicManager, 4)) CurseRun.LoadElite = true;

                if (Plugin.PruneRelicsOnNewCurseRunOn)
                {
                    __instance.maxPlayerHealth.Reset();
                    __instance.playerHealth.Reset();
                    CurseRun.PruneRelics(____relicManager);
                }
                else
                {
                    __instance.playerHealth.Set(__instance.maxPlayerHealth.Value);
                }
                CurseRun.PruneDeck(____deckManager);

                PopulateSuggestionOrbs.ShouldForceNewOrb = true;

                StaticGameData.hasReachedBoss = false;
                StaticGameData.specificNodeOverride = null;
                StaticGameData.relicRarityOverride = RelicRarity.NONE;

                List<Relic> cursedRelics = CurseRun.GetCurseRelics(____relicManager);
                if (cursedRelics.Count > 0)
                {
                    GameObject.Find("SkipButton").SetActive(false);
                    ____chooseRelicCanvas.GetComponent<CanvasGroup>().DOFade(1f, ____chooseRelicCanvasFadeInTime);
                    component.DOFade(1f, ____chooseRelicCanvasFadeInTime).From(0f, true, false);
                    ____chosenRelics = cursedRelics;
                    for (int j = 0; j < ____chosenRelics.Count; j++)
                    {
                        ____chooseRelicIcons[j].SetRelic(____chosenRelics[j]);
                    }
                }

                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(GameInit), nameof(GameInit.ChooseRelic))]
    public static class ChooseRelic
    {
        public static bool Prefix(GameInit __instance, int chosenIndex)
        {
            if (!CurseRun.HasAnotherRelic()) return true;


            if (__instance._relicsAvailable)
            {
                Relic relic = __instance._chosenRelics[chosenIndex];
                __instance._relicManager.AddRelic(relic);
                CurseRun.RemoveRelicFromList(relic);
                if (CurseRun.HasAnotherRelic())
                {
                    CanvasGroup component = __instance._chooseRelicCanvas.GetComponent<CanvasGroup>();
                    component.GetComponent<GraphicRaycaster>().enabled = false;
                    TweenerCore<float, float, FloatOptions> tweenerCore = component.DOFade(0f, __instance._chooseRelicCanvasFadeOutTime);
                    tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(LoadPostMainMenu));
                } else
                {
                    __instance.SkipRelic();
                }
            }
            return false;
        }

        public static void LoadPostMainMenu()
        {
            SceneManager.LoadScene("PostMainMenu");
        }
    }

    [HarmonyPatch(typeof(DeckManager), nameof(DeckManager.InstantiateDeck))]
    public static class BlockDeckInit
    {
        public static bool Prefix()
        {
            return !CurseRun.BlockDeckInit;
        }
    }

}
