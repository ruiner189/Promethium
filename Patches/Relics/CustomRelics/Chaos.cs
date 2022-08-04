using ProLib.Relics;
using Relics;
using System.Collections.Generic;
using System.Linq;

namespace Promethium.Patches.Relics.CustomRelics
{
    public sealed class Chaos : CustomRelic
    {
        public override void OnRelicAdded(RelicManager relicManager)
        {
            MixRelicPools(relicManager);
        }

        public static void MixRelicPools(RelicManager relicManager)
        {
            List<Relic> combinedRelics = new List<Relic>();
            combinedRelics = relicManager._availableCommonRelics.Union(relicManager._availableRareRelics).Union(relicManager._availableBossRelics).ToList();

            foreach (Relic relic in relicManager.RareScenarioRelicPool)
            {
                if (relic is not CurseRelic)
                {
                    if (relic is CustomRelic customRelic && !CustomRelicManager.RelicActive(customRelic))
                        combinedRelics.Add(relic);

                    else if (!relicManager.RelicEffectActive(relic.effect))
                        combinedRelics.Add(relic);
                }
            }

            relicManager._availableCommonRelics = combinedRelics;
            relicManager._availableRareRelics = combinedRelics;
            relicManager._availableBossRelics = combinedRelics;
        }

        public override void OnRelicRemoved(RelicManager relicManager)
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
        }
    }
}
