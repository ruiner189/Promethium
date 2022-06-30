using DG.Tweening;
using HarmonyLib;
using Promethium.Extensions;
using Promethium.Patches.Relics.CustomRelicIcon;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Promethium.Patches.Relics.CustomRelics
{
    public sealed class RealityMarble : CustomRelic
    {
        public static readonly int GRAVITY_CHANGE_SECONDS = 5;
        public static readonly Vector2 DEFAULT_GRAVITY = new Vector2(0, -9.8f);
        private static float _time = 0;

        public override void OnRelicAdded(RelicManager relicManager)
        {
        }

        public override void OnRelicRemoved(RelicManager relicManager)
        {
            ChangeGravity(relicManager, true);
        }

        public static void ChangeGravity(RelicManager relicManager, bool normal = false)
        {
            Vector2 gravity = DEFAULT_GRAVITY;
            if (!normal)
            {
                relicManager.AttemptUseRelic(CustomRelicEffect.GRAVITY_CHANGE);

                float force = 9.8f;
                float xSign = Random.Range(0, 2) == 0 ? 1 : -1;
                float ySign = Random.Range(0, 5) == 0 ? 1 : -1;

                

                float y = ySign * Random.Range(2, force);
                float x = xSign * (force - Math.Abs(y));

                gravity = new Vector2(x, y);
            }

            if (relicManager.RelicEffectActive(CustomRelicEffect.REDUCED_GRAVITY))
                gravity *= PocketMoon.GRAVITY_REDUCTION;

            Physics2D.gravity = gravity;
        }

        [HarmonyPatch(typeof(RelicUI), nameof(RelicUI.AddRelic))]
        public static class AddAnimatedVersion
        {
            public static bool Prefix(RelicUI __instance, Relic toAdd)
            {
                if(toAdd is RealityMarble)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.relicIconPrefab);
                    GameObject.DestroyImmediate(gameObject.GetComponent<RelicIcon>());

                    RelicIcon component = gameObject.AddComponent<RealityMarbleIcon>();

                    component.SetRelic(toAdd);
                    if (!__instance.icons.ContainsKey(toAdd.effect))
                    {
                        __instance.icons.Add(toAdd.effect, component);
                    }
                    gameObject.transform.SetParent(__instance.gameObject.transform, false);
                    gameObject.GetComponent<Image>().DOFade(1f, 0.5f).From(0f, true, false);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.Update))]
        public static class Update
        {
            private static Vector2 _currentGravity;
            private static bool _restoreGravity = false;
            public static void Prefix(BattleController __instance)
            {

                if (BattleController._battleState == BattleController.BattleState.AWAITING_SHOT_COMPLETION)
                {
                    if (__instance._relicManager.RelicEffectActive(CustomRelicEffect.GRAVITY_CHANGE))
                    {
                        _time += Time.deltaTime;
                        if (_time >= GRAVITY_CHANGE_SECONDS)
                        {
                            _time -= GRAVITY_CHANGE_SECONDS;
                            ChangeGravity(__instance._relicManager);
                        }
                    }
                }
                else if (BattleController._battleState == BattleController.BattleState.AWAITING_ENEMY_CLEANUP)
                {
                    if (__instance._relicManager.RelicEffectActive(CustomRelicEffect.GRAVITY_CHANGE))
                    {
                        _time = 0;
                        if (_restoreGravity)
                        {
                            Physics2D.gravity = _currentGravity;
                            _restoreGravity = false;
                        }

                    }
                } 
                else if (BattleController._battleState == BattleController.BattleState.THROW_BOMBS)
                {
                    if (__instance._relicManager.RelicEffectActive(CustomRelicEffect.GRAVITY_CHANGE))
                    {
                        _currentGravity = new Vector2() + Physics2D.gravity;
                        _restoreGravity = true;
                        ChangeGravity(__instance._relicManager, true);
                    }
                } else if (BattleController._battleState == BattleController.BattleState.NAVIGATION)
                {
                    ChangeGravity(__instance._relicManager, true);
                }
            }
        }
    }
}
