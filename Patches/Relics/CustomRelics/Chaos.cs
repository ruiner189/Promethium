using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Relics.CustomRelics
{
    public sealed class Chaos : CustomRelic
    {
        private List<Relic> _commonRelics;
        private List<Relic> _rareRelics;
        private List<Relic> _bossRelics;
        private List<Relic> _combinedRelics;

        public override void OnRelicAdded(RelicManager relicManager)
        {
            _commonRelics = relicManager._availableCommonRelics;
            _rareRelics = relicManager._availableRareRelics;
            _bossRelics = relicManager._availableBossRelics;

            _combinedRelics = new List<Relic>();
            _combinedRelics = _commonRelics.Union(_rareRelics).Union(_bossRelics).ToList();

            foreach(Relic relic in relicManager._rareScenarioRelics.relics)
            {
                if (relic is not CurseRelic)
                {
                    if (!relicManager.RelicEffectActive(relic.effect))
                    {
                        _combinedRelics.Add(relic);

                    }
                }
            }

            relicManager._availableCommonRelics = _combinedRelics;
            relicManager._availableRareRelics = _combinedRelics;
            relicManager._availableBossRelics = _combinedRelics;
        }

        public override void OnRelicRemoved(RelicManager relicManager)
        {
            relicManager._availableCommonRelics = _combinedRelics.Where(relic => _commonRelics.Contains(relic)).ToList();
            relicManager._availableRareRelics = _combinedRelics.Where(relic => _rareRelics.Contains(relic)).ToList();
            relicManager._availableBossRelics = _combinedRelics.Where(relic => _bossRelics.Contains(relic)).ToList();
        }
    }
}
