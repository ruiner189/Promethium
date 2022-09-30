using HarmonyLib;
using System;
using UnityEngine;

namespace Promethium.Patches.Mechanics
{
    [HarmonyPatch(typeof(BattleController), "Update")]
    public static class SpeedUp
    {
        private static float timeElapsed = 0f;
        private static bool speedUpActive = false;
        public static void Postfix(BattleController __instance)
        {
            if (Plugin.SpeedUpOn)
            {
                float startSpeed = TimescaleManager.Instance._isSpedUp ? SettingsManager.Instance.SpeedupLevel : 1;
                if (BattleController._battleState == BattleController.BattleState.AWAITING_SHOT_COMPLETION)
                {
                    if (startSpeed >= Plugin.SpeedUpMax) return;

                    timeElapsed += Time.deltaTime;
                    if (timeElapsed > Plugin.SpeedUpDelay)
                    {
                        float speedUp = (float) Math.Pow(1 + (0.05f * Plugin.SpeedUpRate), timeElapsed - (Plugin.SpeedUpDelay - 1)) - 1;
                        Time.timeScale = Mathf.Clamp(startSpeed + speedUp, startSpeed, Plugin.SpeedUpMax);
                        speedUpActive = true;
                    }
                }
                else
                {
                    timeElapsed = 0f;
                    if (speedUpActive)
                    {
                        Time.timeScale = startSpeed;
                        speedUpActive = false;
                    }
                }
            }
        }
    }
}
