using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BattleController;

namespace Promethium.Patches.Relics
{
    public class DynamicRelicIcon
    {
        private static Dictionary<RelicEffect, DynamicRelicIcon> _dictionary = new Dictionary<RelicEffect, DynamicRelicIcon>();
        private static Dictionary<RelicEffect, RelicIcon> _icons = new Dictionary<RelicEffect, RelicIcon>();
        private static BattleState _state = BattleState.SHOULD_SPAWN;

        public readonly RelicEffect Effect;
        public bool Prepare;
        public bool Attacking;
        public bool Navigating;
        public bool TreasureNavigation;

        public DynamicRelicIcon(RelicEffect effect, bool prepare = true, bool attacking = true, bool navigating = true, bool treasureNavigation = false) {
            Effect = effect;
            Prepare = prepare;
            Attacking = attacking;
            Navigating = navigating;

            if (_dictionary.ContainsKey(effect)) _dictionary.Remove(effect);
            _dictionary.Add(effect, this);
        }

        public DynamicRelicIcon(CustomRelicEffect effect, bool prepare = true, bool attacking = true, bool navigating = true, bool treasureNavigation = false) 
            : this((RelicEffect) effect, prepare, attacking, navigating, treasureNavigation){}

        public static DynamicRelicIcon GetRelicIcon(RelicEffect effect)
        {
            if(Plugin.DynamicIconActive && _icons.Count >= Plugin.DynamicIconMinimum)
                if (_dictionary.ContainsKey(effect)) 
                    return _dictionary[effect];
            return null;
        }

        public static DynamicRelicIcon GetRelicIcon(CustomRelicEffect effect)
        {
            return GetRelicIcon((RelicEffect)effect);
        }

        [HarmonyPatch(typeof(PostBattleController), nameof(PostBattleController.TriggerVictory))]
        public static class PostBattleVictory
        {
            public static void Prefix()
            {
                foreach (RelicIcon icon in _icons.Values)
                {
                   icon.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Start))]
        public static class BattleControllerStart
        {
            public static void Prefix()
            {
                _icons.Clear();
                GameObject relicContainer = GameObject.Find("RelicContainer");
                if (relicContainer != null)
                {
                    foreach (Transform child in relicContainer.transform)
                    {
                        RelicIcon icon = child.GetComponent<RelicIcon>();
                        if (icon != null)
                        {
                            icon.gameObject.SetActive(true);
                            _icons.Add(icon.relic.effect, icon);
                        }
                    }
                }
            }
        }


        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Update))]
        public static class BattleControllerUpdate
        {
            public static void Prefix(BattleController __instance)
            {
                BattleState state = BattleController._battleState;
                if (state == _state) return;
                _state = state;
                if (state == BattleState.AWAITING_SHOT_COMPLETION)
                {
                    foreach(RelicEffect effect in _icons.Keys)
                    {
                        DynamicRelicIcon dynamicRelicIcon = DynamicRelicIcon.GetRelicIcon(effect);
                        bool keep = dynamicRelicIcon == null ? true : dynamicRelicIcon.Attacking;
                        _icons[effect].gameObject.SetActive(keep);
                    }
                } else if (state == BattleState.AWAITING_POST_BATTLE_CONTROLLER)
                {
                    foreach (RelicIcon icon in _icons.Values)
                    {
                        icon.gameObject.SetActive(true);
                    }
                } else if (state == BattleState.NAVIGATION)
                {
                    foreach (RelicEffect effect in _icons.Keys)
                    {
                        DynamicRelicIcon dynamicRelicIcon = DynamicRelicIcon.GetRelicIcon(effect);
                        bool keep = dynamicRelicIcon == null ? true : dynamicRelicIcon.Navigating;
                        _icons[effect].gameObject.SetActive(keep);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.DoEndOfTurnBattleCleanup))]
        public static class BattleControllerCleanup
        {
            public static void Prefix()
            {
                foreach (RelicEffect effect in _icons.Keys)
                {
                    DynamicRelicIcon dynamicRelicIcon = DynamicRelicIcon.GetRelicIcon(effect);
                    bool keep = dynamicRelicIcon == null ? true : dynamicRelicIcon.Prepare;
                    _icons[effect].gameObject.SetActive(keep);
                }
            }
        }
    }


}
