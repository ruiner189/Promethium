using HarmonyLib;
using Promethium.Extensions;
using Promethium.Patches.Relics;
using UnityEngine;

namespace Promethium.Components
{
    [RequireComponent(typeof(PachinkoBall))]
    public class Plasma : MonoBehaviour
    {
        public const int AdditionalZaps = 3;
        public const int PegsToHit = 5;
        private ThunderOrbPachinko _thunder;
        private PachinkoBall _pachinkoBall;
        private int _pegsHit = 0;
        private int _defaultZaps = 0;
        private static LineRenderer _line;

        public void Start()
        {
            _pachinkoBall = gameObject.GetComponent<PachinkoBall>();
            _thunder = gameObject.GetComponent<ThunderOrbPachinko>();
            _pegsHit = 0;
            if (_thunder == null)
            {
                _thunder = gameObject.AddComponent<ThunderOrbPachinko>();
                _thunder.numZaps = 0;

                if(_line == null)
                {
                    GameObject gameObject = Resources.Load<GameObject>("Prefabs/Orbs/LightningBall-Lvl3");
                    if(gameObject != null)
                    {
                        if(_line == null) {
                            ThunderOrbPachinko component = gameObject.GetComponent<ThunderOrbPachinko>();
                            if (component != null)
                            {
                                _line = component._line;
                            }
                        }

                    }
                }

                GameObject line = GameObject.Instantiate<GameObject>(_line.gameObject, transform);
                _thunder._line = line.GetComponent<LineRenderer>();
            }

            _defaultZaps = _thunder.numZaps;
        }

        public void AddToDefault(int amount)
        {
            _defaultZaps += amount;
        }

        public void ActivateEffect()
        {
            if (_thunder != null)
            {
                if (_pachinkoBall._relicManager != null)
                    _pachinkoBall._relicManager.AttemptUseRelic(CustomRelicEffect.PLASMA_BALL);
                _thunder.numZaps = _defaultZaps + AdditionalZaps;
            }
        }

        public void DeactivateEffect()
        {
            if (_thunder != null)
            {
                _thunder.numZaps = _defaultZaps;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_pachinkoBall == null || _pachinkoBall.IsDummy)
            {
                return;
            }
            Peg component = other.collider.GetComponent<Peg>();
            if (component != null)
            {
                _pegsHit++;
                if(_pegsHit == PegsToHit)
                {
                    _pegsHit = 0;
                    ActivateEffect();
                } else
                {
                    DeactivateEffect();
                }
            }
        }
    }

    [HarmonyPatch(typeof(ThunderOrbPachinko), nameof(ThunderOrbPachinko.OnCollisionEnter2D))]
    public static class FixThunderPredictionOrbs
    {
        public static bool Prefix(ThunderOrbPachinko __instance) 
        {
            if (__instance._pachinkoBall == null) return false;
            return true;
        }
    }
}
