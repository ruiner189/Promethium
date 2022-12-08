using ProLib.Relics;
using HarmonyLib;
using Relics;
using System;
using UnityEngine;
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
                CustomRelicManager.Instance.AttemptUseRelic(RelicNames.GRAVITY_CHANGE);

                float force = 9.8f;
                float xSign = Random.Range(0, 2) == 0 ? 1 : -1;
                float ySign = Random.Range(0, 5) == 0 ? 1 : -1;

                

                float y = ySign * Random.Range(2, force);
                float x = xSign * (force - Math.Abs(y));

                gravity = new Vector2(x, y);
            }

            if (CustomRelicManager.Instance.RelicActive(RelicNames.REDUCED_GRAVITY))
                gravity *= PocketMoon.GRAVITY_REDUCTION;

            Physics2D.gravity = gravity;
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
                    if (CustomRelicManager.Instance.RelicActive(RelicNames.GRAVITY_CHANGE))
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
                    if (CustomRelicManager.Instance.RelicActive(RelicNames.GRAVITY_CHANGE))
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
                    if (CustomRelicManager.Instance.RelicActive(RelicNames.GRAVITY_CHANGE))
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
