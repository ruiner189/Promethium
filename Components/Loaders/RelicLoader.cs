using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Loaders
{
    public class RelicLoader : MonoBehaviour
    {
        private static bool _relicsRegistered = false;
        public void Start()
        {
            if (!_relicsRegistered)
                RegisterCustomRelics();
            StartCoroutine(DelayedStart());
        }

        public IEnumerator DelayedStart()
        {
            yield return new WaitForEndOfFrame();
            AddRelicsToPools();
        }

        private void RegisterCustomRelics()
        {
            CustomRelicBuilder.Build("holster", Plugin.Holster, CustomRelicEffect.HOLSTER, RelicPool.BOSS);
            CustomRelicBuilder.Build("mini", Plugin.MiniBelt, CustomRelicEffect.MINI, RelicPool.RARE);
            CustomRelicBuilder.Build("wumbo", Plugin.WumboBelt, CustomRelicEffect.WUMBO, RelicPool.RARE);
            CustomRelicBuilder.Build("kill_button", Plugin.KillButtonRelic, CustomRelicEffect.KILL_BUTTON, RelicPool.COMMON);
            CustomRelicBuilder.Build("plasma_ball", Plugin.PlasmaBall, CustomRelicEffect.PLASMA_BALL, RelicPool.RARE);

            CustomRelicBuilder.BuildAsCurse("curse_one_balance", Plugin.CurseOne, CustomRelicEffect.CURSE_ONE_BALANCE, 1);
            CustomRelicBuilder.BuildAsCurse("curse_one_attack", Plugin.CurseOne, CustomRelicEffect.CURSE_ONE_ATTACK, 1);
            CustomRelicBuilder.BuildAsCurse("curse_one_crit", Plugin.CurseOne, CustomRelicEffect.CURSE_ONE_CRIT, 1);

            CustomRelicBuilder.BuildAsCurse("curse_two_health", Plugin.CurseTwo, CustomRelicEffect.CURSE_TWO_HEALTH, 2);
            CustomRelicBuilder.BuildAsCurse("curse_two_armor", Plugin.CurseTwo, CustomRelicEffect.CURSE_TWO_ARMOR, 2);
            CustomRelicBuilder.BuildAsCurse("curse_two_equip", Plugin.CurseTwo, CustomRelicEffect.CURSE_TWO_EQUIP, 2);

            CustomRelicBuilder.BuildAsCurse("curse_three_bomb", Plugin.CurseThree, CustomRelicEffect.CURSE_THREE_BOMB, 3);
            CustomRelicBuilder.BuildAsCurse("curse_three_attack", Plugin.CurseThree, CustomRelicEffect.CURSE_THREE_ATTACK, 3);
            CustomRelicBuilder.BuildAsCurse("curse_three_crit", Plugin.CurseThree, CustomRelicEffect.CURSE_THREE_CRIT, 3);

            CustomRelicBuilder.BuildAsCurse("curse_four_health", Plugin.CurseFour, CustomRelicEffect.CURSE_FOUR_HEALTH, 4);
            CustomRelicBuilder.BuildAsCurse("curse_four_armor", Plugin.CurseFour, CustomRelicEffect.CURSE_FOUR_ARMOR, 4);
            CustomRelicBuilder.BuildAsCurse("curse_four_equip", Plugin.CurseFour, CustomRelicEffect.CURSE_FOUR_EQUIP, 4);

            CustomRelicBuilder.BuildAsCurse("curseFiveA", Plugin.CurseFive, CustomRelicEffect.CURSE_FIVE_A, 5);
            CustomRelicBuilder.BuildAsCurse("curseFiveB", Plugin.CurseFive, CustomRelicEffect.CURSE_FIVE_B, 5);
            CustomRelicBuilder.BuildAsCurse("curseFiveC", Plugin.CurseFive, CustomRelicEffect.CURSE_FIVE_C, 5);
            _relicsRegistered = true;
        }

        private void AddRelicsToPools()
        {
            List<CustomRelic> relics = CustomRelic.AllCustomRelics;

            RelicSet[] pools = Resources.FindObjectsOfTypeAll<RelicSet>();
            RelicSet commonPool = null;
            RelicSet rarePool = null;
            RelicSet bossPool = null;
            RelicSet rareScenarioPool = null;

            foreach (RelicSet pool in pools)
            {
                if (pool.name == "CommonRelics") commonPool = pool;
                else if (pool.name == "RareRelics") rarePool = pool;
                else if (pool.name == "BossRelics") bossPool = pool;
                else if (pool.name == "RareRelicsScenarioOnly") rareScenarioPool = pool;
            }

            foreach (CustomRelic relic in relics)
            {
                if (!relic.IsEnabled())
                {
                    if (rareScenarioPool != null)
                    {
                        rareScenarioPool.relics.Add(relic);
                    }
                }
                switch (relic.GetPoolType())
                {
                    case RelicPool.COMMON:
                        if (commonPool != null)
                            if (!commonPool.relics.Contains(relic))
                                commonPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE:
                        if (rarePool != null)
                            if (!rarePool.relics.Contains(relic))
                                rarePool.relics.Add(relic);
                        break;
                    case RelicPool.BOSS:
                        if (bossPool != null)
                            if (!bossPool.relics.Contains(relic))
                                bossPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE_SCENARIO:
                    case RelicPool.CURSE:
                        if (rareScenarioPool != null)
                            if (!rareScenarioPool.relics.Contains(relic))
                                rareScenarioPool.relics.Add(relic);
                        break;
                }
            }
        }
    }
}
