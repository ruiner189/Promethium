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

        private static List<CustomRelic> _allCustomRelics = new List<CustomRelic>();
        private String _name;
        private Sprite _sprite;
        private CustomRelicEffect _effect = CustomRelicEffect.NONE;
        private RelicPool _pool = RelicPool.RARE_SCENARIO;

        public static List<CustomRelic> GetAllCustomRelics()
        {
            return _allCustomRelics;
        }

        public static CustomRelic GetCustomRelic(CustomRelicEffect effect)
        {
            return _allCustomRelics.Find(relic => relic.effect == (RelicEffect) effect);
        }

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
            _allCustomRelics.Add(relic);
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
    }

    [HarmonyPatch(typeof(GameInit),"Start")]
    public static class AddRelicsToPool
    {
        public static void Prefix(RelicManager ____relicManager)
        {
            List<CustomRelic> relics = CustomRelicBuilder.GetAllCustomRelics();
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
                        commonPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE:
                        rarePool.relics.Add(relic);
                        break;
                    case RelicPool.BOSS:
                       bossPool.relics.Add(relic);
                        break;
                    case RelicPool.RARE_SCENARIO:
                        rareScenarioPool.relics.Add(relic);
                        break;
                }
                Plugin.Log.LogMessage($"{relic.locKey} was successfully registered.");
            }
        }
    }


    
}
