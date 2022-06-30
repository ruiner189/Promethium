using HarmonyLib;
using Relics;
using System;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public class CustomRelicBuilder
    {

        private String _name;
        private Sprite _sprite;
        private CustomRelicEffect _effect;
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
            relic.name = _name;
            relic.locKey = _name;
            relic.sprite = _sprite;
            relic.effect = (RelicEffect)_effect;
            relic.SetPoolType(_pool);
            relic.Register();
            Plugin.Log.LogDebug($"{relic.locKey} successfully registered.");
            return relic;
        }

        public T Build<T>() where T : CustomRelic
        {
            T relic = ScriptableObject.CreateInstance<T>();
            relic.name = _name;
            relic.locKey = _name;
            relic.sprite = _sprite;
            relic.effect = (RelicEffect)_effect;
            relic.SetPoolType(_pool);
            relic.Register();
            Plugin.Log.LogDebug($"{relic.locKey} successfully registered.");
            return relic;
        }

        public CurseRelic BuildAsCurse(int CurseLevel)
        {
            CurseRelic relic = ScriptableObject.CreateInstance<CurseRelic>();
            relic.name = _name;
            relic.locKey = _name;
            relic.sprite = _sprite;
            relic.effect = (RelicEffect)_effect;
            relic.SetPoolType(_pool);
            relic.CurseLevel = CurseLevel;
            relic.Register();
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

        public static T Build<T>(String name, Sprite sprite, CustomRelicEffect effect, RelicPool pool = RelicPool.RARE_SCENARIO) where T : CustomRelic
        {
            return new CustomRelicBuilder()
                 .SetName(name)
                 .SetSprite(sprite)
                 .SetRelicEffect(effect)
                 .SetRelicPool(pool)
                 .Build<T>();
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

    [HarmonyPatch(typeof(GameInit), "Start")]
    public static class AddRelicsToPool
    {

    }
}
