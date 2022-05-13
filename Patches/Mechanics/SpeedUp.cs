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
        public static void Postfix(BattleController __instance, int ____battleState) 
        {
            if (Plugin.SpeedUpOn)
            {
                if (____battleState == 3)
                {
                    timeElapsed += Time.deltaTime;

                    if (timeElapsed > Plugin.SpeedUpDelay)
                    {
                        float speedUp = Math.Min((float)(1f + Math.Log10((timeElapsed - (Plugin.SpeedUpDelay - 1)) * Plugin.SpeedUpRate)), Plugin.SpeedUpMax);
                        speedUpActive = true;
                        Time.timeScale = speedUp;
                    }
                }
                else
                {
                    timeElapsed = 0f;
                    if (speedUpActive)
                    {
                        Time.timeScale = 1f;
                        speedUpActive = false;
                    }
                }
            }
        }
    }
}
