using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PeglinMod.Patches.Mechanics
{
    [HarmonyPatch(typeof(BattleController), "Update")]
    public static class SpeedUp
    {
        private static float timeElapsed = 0f;
        private static bool speedUpActive = false;
        public static void Postfix(BattleController __instance, int ____battleState) 
        {
            if(____battleState == 3)
            {
                int secondsTillSpeedUp = 10;
                timeElapsed += Time.deltaTime;

                if(timeElapsed > secondsTillSpeedUp)
                {
                    float speedUp = Math.Min((float)(1f + Math.Log10(timeElapsed - (secondsTillSpeedUp - 1))),3);
                    speedUpActive = true;
                    Time.timeScale = speedUp;
                }
            } else
            {
                timeElapsed = 0f;
                if (speedUpActive)
                {
                    Time.timeScale = 1f;
                }
            }
        }
    }
}
