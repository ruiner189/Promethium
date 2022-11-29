using CustomChallenges;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ProLib.Relics;
using HarmonyLib;
using Promethium.Patches.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
            List<Relic> relics = relicDict.Values.Union(CustomRelicManager.OwnedRelics).ToList();

            Random rand = new Random();
            List<Relic> curses = relics.FindAll(relic => relic is CurseRelic);
            _oldRelics = relics.FindAll(relic => !curses.Contains(relic)).OrderBy(relic => rand.Next()).ToList();

            _commonRelics = 3;
            _rareRelics = 2;
            _bossRelics = 1;

            if (CurseRelic.IsCurseLevelActive(1))
            {
                _commonRelics++;
                _rareRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(2))
            {
                _rareRelics++;
                _bossRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(3))
            {
                _commonRelics+=2;
                _rareRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(4))
            {
                _commonRelics+=2;
                _rareRelics++;
            }

            if (CurseRelic.IsCurseLevelActive(5))
            {
                _commonRelics++;
                _rareRelics++;
                _bossRelics++;
            }

            manager.Reset();
            curses.ForEach(manager.AddRelic);
        }

        public static bool HasAnotherRelic()
        {
            if (_oldRelics == null) return false;
            if (_commonRelics == 0 && _rareRelics == 0 && _bossRelics == 0) return false;
            if (_oldRelics.Count >= 1) return true;
            return false;
        }

        public static List<Relic> GetNextRelicSet(RelicManager relicManager)
        {
            if (_commonRelics == 0 && _rareRelics == 0 && _bossRelics == 0) 
                return new List<Relic>();

            if (_commonRelics != 0)
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

        public static void RegenerateRelicPools(RelicManager relicManager)
        {
            relicManager._availableCommonRelics = relicManager.CommonRelicPool.Where(relic => {
                if (relicManager.RelicEffectActive(relic.effect)) return false;
                if (relic is CustomRelic customRelic && CustomRelicManager.RelicActive(customRelic)) return false;
                return true;
            }).ToList();
            relicManager._availableRareRelics = relicManager.RareRelicPool.Where(relic => {
                if (relicManager.RelicEffectActive(relic.effect)) return false;
                if (relic is CustomRelic customRelic && CustomRelicManager.RelicActive(customRelic)) return false;
                return true;
            }).ToList();
            relicManager._availableBossRelics = relicManager.BossRelicPool.Where(relic => {
                if (relicManager.RelicEffectActive(relic.effect)) return false;
                if (relic is CustomRelic customRelic && CustomRelicManager.RelicActive(customRelic)) return false;
                return true;
            }).ToList();

            if (CustomRelicManager.RelicActive(RelicNames.SINGLE_ITEM_POOL))
                Chaos.MixRelicPools(relicManager);
        }

        public static void RemoveRelicFromList(Relic relic)
        {
            _oldRelics.Remove(relic);
        }

        public static List<Relic> GetCurseRelics(RelicManager manager)
        {
            if (CurseRelic.IsCurseLevelActive(5))
            {
                return new Relic[] { manager.consolationPrize, manager.consolationPrize, manager.consolationPrize }.ToList();
            }
            Dictionary<RelicEffect, Relic> relicDict = manager._ownedRelics;
            List<CustomRelic> ownedCurseRelics = CustomRelicManager.OwnedRelics.Where(relic => relic is CurseRelic).ToList();

            List<Relic> curses = new List<Relic>(CurseRelic.GetCurseRelicOfLevel(ownedCurseRelics.Count + 1));

            while (curses.Count > 3) curses.RemoveAt(curses.Count - 1);
            return curses;
        }

        public static bool ShouldStartNextCurseRun()
        {
            if(Victory && Plugin.CurseRunOn)
            {
                if (Plugin.CustomChallengesPlugin)
                {
                    return CheckChallengeAllows();
                }
                return true;
            }
            return false;
        }

        private static bool CheckChallengeAllows()
        {
            if(ChallengeManager.ChallengeActive)
                return ChallengeManager.CurrentChallenge.TryGetEntry<bool>("allowCurseRuns", out bool allowCurseRuns) && allowCurseRuns;
            return true;
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

            List<Relic> relicPool = null;
            if (rarity == RelicRarity.COMMON)
                relicPool = manager.CommonRelicPool;
            else if (rarity == RelicRarity.RARE)
                relicPool = manager.RareRelicPool;
            else if (rarity == RelicRarity.BOSS)
                relicPool = manager.BossRelicPool;

            Relic relic = relics.Find(r => relicPool.Contains(r));
            if (relic == null)
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
        [HarmonyPriority(Priority.Low)]
        public static bool Prefix(GameInit __instance)
        {
            CanvasGroup component = GameObject.FindGameObjectWithTag("PersistentUI").GetComponent<CanvasGroup>();
            CurseRun.BlockDeckInit = false;

            if (CurseRun.HasAnotherRelic())
            {
                CurseRun.BlockDeckInit = true;
                __instance._chooseRelicCanvas.GetComponent<CanvasGroup>().DOFade(1f, __instance._chooseRelicCanvasFadeInTime);
                component.DOFade(1f, __instance._chooseRelicCanvasFadeInTime).From(0f, true, false);
                __instance._chosenRelics = CurseRun.GetNextRelicSet(__instance._relicManager);
                while (__instance._chosenRelics.Count == 0 && CurseRun.HasAnotherRelic())
                {
                    __instance._chosenRelics = CurseRun.GetNextRelicSet(__instance._relicManager);
                }

                if (__instance._chosenRelics.Count > 0)
                {
                    for (int j = 0; j < __instance._chosenRelics.Count; j++)
                    {
                        __instance._chooseRelicIcons[j].SetRelic(__instance._chosenRelics[j]);
                    }
                    return false;
                }
            }

            CurseRun.LoadElite = false;
            if (CurseRun.ShouldStartNextCurseRun())
            {
                CurseRun.Victory = false;
                CurseRun.BlockDeckInit = true;
                if (CurseRelic.IsCurseLevelActive(4)) CurseRun.LoadElite = true;

                if (Plugin.PruneRelicsOnNewCurseRunOn)
                {
                    __instance.maxPlayerHealth.Reset();
                    __instance.playerHealth.Reset();
                    CurseRun.PruneRelics(__instance._relicManager);
                    CurseRun.RegenerateRelicPools(__instance._relicManager);
                }
                else
                {
                    __instance.playerHealth.Set(__instance.maxPlayerHealth.Value);
                }
                CurseRun.PruneDeck(__instance._deckManager);

                StaticGameData.hasReachedBoss = false;
                StaticGameData.specificNodeOverride = null;
                StaticGameData.relicRarityOverride = RelicRarity.NONE;

                List<Relic> cursedRelics = CurseRun.GetCurseRelics(__instance._relicManager);
                if (cursedRelics.Count > 0)
                {
                    GameObject.Find("SkipButton").SetActive(false);
                    __instance._chooseRelicCanvas.GetComponent<CanvasGroup>().DOFade(1f, __instance._chooseRelicCanvasFadeInTime);
                    component.DOFade(1f, __instance._chooseRelicCanvasFadeInTime).From(0f, true, false);
                    __instance._chosenRelics = cursedRelics;
                    for (int j = 0; j < __instance._chosenRelics.Count; j++)
                    {
                        __instance._chooseRelicIcons[j].SetRelic(__instance._chosenRelics[j]);
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
