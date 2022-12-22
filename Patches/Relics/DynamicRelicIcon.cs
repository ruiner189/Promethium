using ProLib.Relics;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BattleController;
using System.Linq;
using Battle;

namespace Promethium.Patches.Relics
{
    public class DynamicRelicIcon
    {
        private static Dictionary<RelicEffect, DynamicRelicIcon> _effectDictionary = new Dictionary<RelicEffect, DynamicRelicIcon>();
        private static Dictionary<String, DynamicRelicIcon> _idDictionary = new Dictionary<String, DynamicRelicIcon>();

        private static List<RelicIcon> _icons = new List<RelicIcon>();
        private static BattleState _state = BattleState.SHOULD_SPAWN;

        public readonly RelicEffect Effect;
        public readonly String Id;

        public bool Prepare;
        public bool Attacking;
        public bool Navigating;
        public bool TreasureNavigation;

        public DynamicRelicIcon(RelicEffect effect, bool prepare = true, bool attacking = true, bool navigating = true, bool treasureNavigation = false) {
            Effect = effect;
            Id = null;
            Prepare = prepare;
            Attacking = attacking;
            Navigating = navigating;

            _effectDictionary[effect] = this;
        }

        public DynamicRelicIcon(String id, bool prepare = true, bool attacking = true, bool navigating = true, bool treasureNavigation = false)
        {
            Effect = RelicEffect.NONE;
            Id = id;
            Prepare = prepare;
            Attacking = attacking;
            Navigating = navigating;

            _idDictionary[id] = this;
        }

        public static DynamicRelicIcon GetRelicIcon(RelicEffect effect)
        {
            if(Plugin.DynamicIconActive && _icons.Count >= Plugin.DynamicIconMinimum)
                if (_effectDictionary.ContainsKey(effect)) 
                    return _effectDictionary[effect];
            return null;
        }

        public static DynamicRelicIcon GetRelicIcon(String id)
        {
            if (Plugin.DynamicIconActive && _icons.Count >= Plugin.DynamicIconMinimum)
                if (_idDictionary.ContainsKey(id))
                    return _idDictionary[id];
            return null;
        }

        [HarmonyPatch(typeof(PostBattleController), nameof(PostBattleController.TriggerVictory))]
        public static class PostBattleVictory
        {
            public static void Prefix()
            {
                foreach (RelicIcon icon in _icons)
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
                            _icons.Add(icon);
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
                    foreach(RelicIcon icon in _icons)
                    {
                        if (icon == null || icon.gameObject == null || icon.relic == null) continue;
                        DynamicRelicIcon dynamicRelicIcon = DynamicRelicIcon.GetRelicIcon(icon.relic.effect);
                        if (dynamicRelicIcon == null && icon.relic is CustomRelic customRelic)
                            dynamicRelicIcon = DynamicRelicIcon.GetRelicIcon(customRelic.Id);
                        bool keep = dynamicRelicIcon == null ? true : dynamicRelicIcon.Attacking;
                        icon.gameObject.SetActive(keep);
                    }
                } else if (state == BattleState.AWAITING_POST_BATTLE_CONTROLLER)
                {
                    foreach (RelicIcon icon in _icons)
                    {
                        if (icon == null || icon.gameObject == null || icon.relic == null) continue;
                        icon.gameObject.SetActive(true);
                    }
                } else if (state == BattleState.NAVIGATION)
                {
                    foreach (RelicIcon icon in _icons)
                    {
                        if (icon == null || icon.gameObject == null || icon.relic == null) continue;

                        DynamicRelicIcon dynamicRelicIcon = DynamicRelicIcon.GetRelicIcon(icon.relic.effect);
                        if (dynamicRelicIcon == null && icon.relic is CustomRelic customRelic)
                            dynamicRelicIcon = DynamicRelicIcon.GetRelicIcon(customRelic.Id);
                        bool keep = dynamicRelicIcon == null ? true : dynamicRelicIcon.Navigating;
                        icon.gameObject.SetActive(keep);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.DoEndOfTurnBattleCleanup))]
        public static class BattleControllerCleanup
        {
            public static void Prefix()
            {
                if (_icons.Any(icon => icon == null))
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
                                _icons.Add(icon);
                            }
                        }
                    }
                }

                foreach (RelicIcon icon in _icons)
                {
                    DynamicRelicIcon dynamicRelicIcon = GetRelicIcon(icon.relic.effect);
                    if (dynamicRelicIcon == null && icon.relic is CustomRelic customRelic)
                        dynamicRelicIcon = GetRelicIcon(customRelic.Id);
                    bool keep = dynamicRelicIcon == null ? true : dynamicRelicIcon.Prepare;
                    icon.gameObject.SetActive(keep);
                }
            }
        }
    }


}
