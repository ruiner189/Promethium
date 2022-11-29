using Battle.Attacks;
using ProLib.Attributes;
using ProLib.Loaders;
using ProLib.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Relics.CustomRelics
{
    [SceneModifier]
    public sealed class AnvilRelic : CustomRelic
    {
        private List<GameObject> _originalCommonOrbs;
        private List<GameObject> _originalUncommonOrbs;
        private List<GameObject> _originalRareOrbs;

        public override void OnRelicAdded(RelicManager relicManager)
        {
            UpgradeOrbs(relicManager._deckManager);
        }

        public void UpgradeOrbs(DeckManager deckManager)
        {
            if (deckManager == null) return;

            _originalCommonOrbs = new List<GameObject>(deckManager.CommonOrbPool);
            _originalUncommonOrbs = new List<GameObject>(deckManager.UncommonOrbPool);
            _originalRareOrbs = new List<GameObject>(deckManager.RareOrbPool);

            deckManager.CommonOrbPool = GetUpgradedOrbPool(_originalCommonOrbs);
            deckManager.UncommonOrbPool = GetUpgradedOrbPool(_originalUncommonOrbs);
            deckManager.RareOrbPool = GetUpgradedOrbPool(_originalRareOrbs);
        }

        public List<GameObject> GetUpgradedOrbPool(List<GameObject> orbPool)
        {
            List<GameObject> upgradedOrbs = new List<GameObject>();
            foreach (GameObject obj in orbPool)
            {
                if (obj == null) continue;
                Attack attack = obj.GetComponent<Attack>();
                GameObject next = attack.NextLevelPrefab;

                upgradedOrbs.Add(next ?? obj);
            }
            return upgradedOrbs;
        }


        public override void OnRelicRemoved(RelicManager relicManager)
        {
            RemoveUpgradedOrbs(relicManager._deckManager);
        }

        public void RemoveUpgradedOrbs(DeckManager deckManager)
        {
            if (deckManager == null) return;
            deckManager.CommonOrbPool = new List<GameObject>(_originalCommonOrbs);
            deckManager.UncommonOrbPool = new List<GameObject>(_originalUncommonOrbs);
            deckManager.RareOrbPool = new List<GameObject>(_originalRareOrbs);
        }

        public static void OnSceneLoaded(String sceneName, bool firstLoad)
        {
            if(sceneName == SceneLoader.MainMenu)
            {
                if (CustomRelicManager.RelicActive(RelicNames.UPGRADED_ORBS))
                {
                    AnvilRelic relic = (AnvilRelic) GetCustomRelic(RelicNames.UPGRADED_ORBS);
                    relic.RemoveUpgradedOrbs(Resources.FindObjectsOfTypeAll<DeckManager>().FirstOrDefault());
                }
            }
        }
    }
}
