using Battle.Enemies;
using Battle.StatusEffects;
using Cruciball;
using DG.Tweening;
using HarmonyLib;
using Promethium.Patches.Mechanics;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Worldmap;
using Random = System.Random;

namespace Promethium.Patches.Mechanics
{

	public static class CurseRun
	{
		public static bool Victory = false;
		public static bool LoadElite = false;

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

            if (!Plugin.PruneRelicsOnNewCurseRunOn)
            {
				manager.Reset();
				relics.ForEach(manager.AddRelic);
				return;
            }

			Random rand = new Random();
			List<Relic> curses = relics.FindAll(relic => CurseRelic.AllCurseRelics.Contains(relic));
			List<Relic> nonCurses = relics.FindAll(relic => !curses.Contains(relic)).OrderBy(relic => rand.Next()).ToList();
			List<Relic> relicsToKeep = new List<Relic>();

			int commonRelics = 2;
			int rareRelics = 2;
			int bossRelics = 1;

			if (CurseRelic.IsCurseLevelActive(manager, 1))
			{
				commonRelics++;
				rareRelics++;
			}

			if (CurseRelic.IsCurseLevelActive(manager, 2))
			{
				commonRelics++;
				rareRelics++;
			}

			if (CurseRelic.IsCurseLevelActive(manager, 3))
			{
				bossRelics++;
			}

			if (CurseRelic.IsCurseLevelActive(manager, 4))
			{
				commonRelics++;
				rareRelics++;
			}

			if (CurseRelic.IsCurseLevelActive(manager, 5))
			{
				commonRelics++;
				rareRelics++;
				bossRelics++;
			}

			int totalRelics = commonRelics + rareRelics + bossRelics;

			for (int i = 0; i < commonRelics; i++)
			{
				Relic relic = GetRelicOfRarity(manager, nonCurses, RelicRarity.COMMON, 10);
				if (relic != null)
				{
					relicsToKeep.Add(relic);
					nonCurses.Remove(relic);
				}
			}

			for (int i = 0; i < rareRelics; i++)
			{
				Relic relic = GetRelicOfRarity(manager, nonCurses, RelicRarity.RARE, 5);
				if (relic != null)
				{
					relicsToKeep.Add(relic);
					nonCurses.Remove(relic);
				}
			}

			for (int i = 0; i < bossRelics; i++)
			{
				Relic relic = GetRelicOfRarity(manager, nonCurses, RelicRarity.BOSS);
				if (relic != null)
				{
					relicsToKeep.Add(relic);
					nonCurses.Remove(relic);
				}
			}

			while (relicsToKeep.Count < totalRelics && nonCurses.Count > 0)
			{
				relicsToKeep.Add(nonCurses[0]);
				nonCurses.RemoveAt(0);
			}

			manager.Reset();
			curses.ForEach(manager.AddRelic);
			relicsToKeep.ForEach(manager.AddRelic);
		}

		public static List<Relic> GetCurseRelics(RelicManager manager)
		{
            if (CurseRelic.IsCurseLevelActive(manager, 5))
            {
				return new Relic[] {manager.consolationPrize, manager.consolationPrize, manager.consolationPrize}.ToList();
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

		public static Relic GetRelicOfRarity(RelicManager manager, List<Relic> relics, RelicRarity rarity, int upgradeChance = 0)
		{
			if (relics.Count == 0) return null;
			if (upgradeChance > 0)
			{
				Random rand = new Random();
				if (rand.Next(1, 1000) == upgradeChance)
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

	[HarmonyPriority(Priority.Last)]
	[HarmonyPatch(typeof(GameInit), "Start")]
	public static class GameInitPatch
	{
		public static bool Prefix(GameInit __instance, DeckManager ____deckManager,
			DeckData ____initialDeck, RelicManager ____relicManager, CruciballManager ____cruciballManager,
			GameObject ____chooseRelicCanvas, float ____chooseRelicCanvasFadeInTime,
			ref List<Relic> ____chosenRelics, RelicIcon[] ____chooseRelicIcons)
		{
			CurseRun.LoadElite = false;
			if (CurseRun.ShouldStartNextCurseRun())
			{
				CurseRun.Victory = false;
				if (CurseRelic.IsCurseLevelActive(____relicManager, 4)) CurseRun.LoadElite = true;
				FloatVariable floatVariable = __instance.playerHealth;
				if (floatVariable != null)
				{
					floatVariable.Reset();
				}
				FloatVariable floatVariable2 = __instance.maxPlayerHealth;
				if (floatVariable2 != null)
				{
					floatVariable2.Reset();
				}

				CurseRun.PruneRelics(____relicManager);
				CurseRun.PruneDeck(____deckManager);

				PopulateSuggestionOrbs.ShouldForceNewOrb = true;
				CanvasGroup component = GameObject.FindGameObjectWithTag("PersistentUI").GetComponent<CanvasGroup>();

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
}
