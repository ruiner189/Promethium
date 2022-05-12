using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public class CustomRelicBuilder{

        private String _name;
        private Sprite _sprite;
        private CustomRelicEffect _effect = CustomRelicEffect.NONE;
        private RelicPool _pool = RelicPool.RARE_SCENARIO;


        public CustomRelicBuilder SetName(String name)
        {
            _name = name;
            return this;
        }

        public CustomRelicBuilder SetSprite(Sprite sprite)
        {
            _sprite = sprite;
            return this;
        }

        public CustomRelicBuilder SetRelicEffect(CustomRelicEffect effect)
        {
            _effect = effect;
            return this;
        }

        public CustomRelicBuilder SetRelicPool(RelicPool pool)
        {
            _pool = pool;
            return this;
        }

        public CustomRelic Build()
        {
            CustomRelic relic = ScriptableObject.CreateInstance<CustomRelic>();
            relic.locKey = _name;
            relic.sprite = _sprite;
            relic.effect = (RelicEffect) _effect;
            relic.SetPoolType(_pool);
            return relic;
        }

        public CurseRelic BuildAsCurse(int CurseLevel)
        {
            CurseRelic relic = ScriptableObject.CreateInstance<CurseRelic>();
            relic.name = _name;
            relic.locKey = _name;
            relic.sprite = _sprite;
            relic.effect = (RelicEffect) _effect;
            relic.SetPoolType(_pool);
            relic.CurseLevel = CurseLevel;
            return relic;
        }

        public static CustomRelic Build(String name, Sprite sprite, CustomRelicEffect effect, RelicPool pool = RelicPool.RARE_SCENARIO)
        {
           return new CustomRelicBuilder()
                .SetName(name)
                .SetSprite(sprite)
                .SetRelicEffect(effect)
                .SetRelicPool(pool)
                .Build();
        }

        public static CurseRelic BuildAsCurse(String name, Sprite sprite, CustomRelicEffect effect, int curseLevel, RelicPool pool = RelicPool.CURSE)
        {
            return new CustomRelicBuilder()
                 .SetName(name)
                 .SetSprite(sprite)
                 .SetRelicEffect(effect)
                 .SetRelicPool(pool)
                 .BuildAsCurse(curseLevel);
        }
    }

    [HarmonyPatch(typeof(GameInit),"Start")]
    public static class AddRelicsToPool
    {
        public static void Prefix(RelicManager ____relicManager)
        {
            List<CustomRelic> relics = CustomRelic.AllCustomRelics;
            ____relicManager.ToString();
            RelicSet commonPool = (RelicSet)____relicManager.GetType().GetField("_commonRelicPool", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(____relicManager);
            RelicSet rarePool = (RelicSet)____relicManager.GetType().GetField("_rareRelicPool", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(____relicManager);
            RelicSet bossPool = (RelicSet)____relicManager.GetType().GetField("_bossRelicPool", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(____relicManager);
            RelicSet rareScenarioPool = (RelicSet)____relicManager.GetType().GetField("_rareScenarioRelics", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(____relicManager);

            foreach (CustomRelic relic in relics)
            {
                switch (relic.GetPoolType())
                {
                    case RelicPool.COMMON:
                        if(!commonPool.relics.Contains(relic))
                            commonPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE:
                        if (!rarePool.relics.Contains(relic))
                            rarePool.relics.Add(relic);
                        break;
                    case RelicPool.BOSS:
                        if (!bossPool.relics.Contains(relic))
                            bossPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE_SCENARIO:
                    case RelicPool.CURSE:
                        if (!rareScenarioPool.relics.Contains(relic))
                            rareScenarioPool.relics.Add(relic);
                        break;
                }
            }
        }
    }


    
}
