using Promethium.Patches.Relics;
using Relics;
using System.Collections.Generic;

namespace Promethium.Extensions
{
    public static class RelicManagerExtension
    {
        public static bool RelicEffectActive(this RelicManager relicManager, CustomRelicEffect effect)
        {
            return relicManager.RelicEffectActive((RelicEffect)effect);
        }

        public static bool AttemptUseRelic(this RelicManager relicManager, CustomRelicEffect effect)
        {
            return relicManager.AttemptUseRelic((RelicEffect)effect);
        }

        // Used instead of RelicManager::Reset so other mods do not modify the reset. 
        public static void ResetSilently(this RelicManager relicManager)
        {
            foreach (Relic relic in relicManager._ownedRelics.Values)
            {
                if (relic is CustomRelic customRelic)
                {
                    customRelic.OnRelicRemoved(relicManager);
                }
            }

            relicManager._ownedRelics = new Dictionary<RelicEffect, Relic>();
            relicManager._relicRemainingCountdowns = new Dictionary<RelicEffect, int>();
            relicManager._relicRemainingUsesPerBattle = new Dictionary<RelicEffect, int>();
            relicManager._orderOfRelicsObtained.Clear();
            relicManager._orderCounter = 0;
            RelicManager.OnRelicsReset(null);
            relicManager._availableCommonRelics = new List<Relic>(relicManager._commonRelicPool.relics);
            relicManager._availableRareRelics = new List<Relic>(relicManager._rareRelicPool.relics);
            relicManager._availableBossRelics = new List<Relic>(relicManager._bossRelicPool.relics);
        }
    }
}
