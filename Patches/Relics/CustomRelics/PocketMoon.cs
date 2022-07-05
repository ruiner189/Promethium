using Battle.Attacks;
using HarmonyLib;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Relics.CustomRelics
{
    public sealed class PocketMoon : CustomRelic
    {
        public static float GRAVITY_REDUCTION = 0.25F;
        public override void OnRelicAdded(RelicManager relicManager)
        {
            Physics2D.gravity *= GRAVITY_REDUCTION;
        }

        public override void OnRelicRemoved(RelicManager relicManager)
        {
            Physics2D.gravity = RealityMarble.DEFAULT_GRAVITY;
        }

        [HarmonyPatch(typeof(BombLob), nameof(BombLob.Shoot))]
        public static class FixBombForce
        {
            public static bool Prefix(BombLob __instance, bool inBoss, int totalThrown)
            {
                __instance._audioSource.volume = Mathf.Clamp(1f / (float)totalThrown, __instance.detonateVolMinMax.x, __instance.detonateVolMinMax.y);
                Vector2 force = inBoss ? new Vector2(UnityEngine.Random.Range(100f, 100f), UnityEngine.Random.Range(120f, 120f)) : new Vector2(UnityEngine.Random.Range(180f, 160f), UnityEngine.Random.Range(220f, 180f));
                force *= (Physics2D.gravity.y / -9.8f);
                __instance._rb.AddForce(force);
                __instance.Rotate();
                return false;
            }
        }
    }
}
