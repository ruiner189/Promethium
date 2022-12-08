using ProLib.Relics;
using HarmonyLib;
using Promethium.Patches.Relics.CustomRelics;
using UnityEngine;
using System.Collections.Generic;
using Battle.PegBehaviour;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;

namespace Promethium.Components
{
    [RequireComponent(typeof(PachinkoBall))]
    public class Plasma : MonoBehaviour
    {
        public const int AmountForZaps = 7;
        public const int Zaps = 3;
        public float ZapDistance = 2.5f;
        public float ZapFadeTime = 1f;
        private List<Peg> _hitPegs = new List<Peg>();


        private PachinkoBall _pachinkoBall;
        private static LineRenderer _linePrefab;
        private LineRenderer _line;

        public void Start()
        {
            _pachinkoBall = gameObject.GetComponent<PachinkoBall>();


            if (_linePrefab == null)
            {
                GameObject gameObject = Resources.Load<GameObject>("Prefabs/Orbs/LightningBall-Lvl3");
                if (gameObject != null)
                {
                    if (_linePrefab == null)
                    {
                        ThunderOrbPachinko component = gameObject.GetComponent<ThunderOrbPachinko>();
                        if (component != null)
                        {
                            _linePrefab = component._line;
                        }
                    }
                }
            }
            _line = Instantiate<GameObject>(_linePrefab.gameObject, transform).GetComponent<LineRenderer>();
        }

        public void ActivateEffect(Peg peg)
        {

            if (peg != null)
            {
                if (_hitPegs.Contains(peg))
                {
                    return;
                }

                _hitPegs.Add(peg);

                List<Vector3> list = new List<Vector3>();

                for (int i = 0; i < Zaps; i++)
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(peg.GetCenterOfPeg(), ZapDistance);
                    float closestDistance = float.PositiveInfinity;
                    Peg pegToZap = null;
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        Peg pegToCheck;
                        if (colliders[j].TryGetComponent<Peg>(out pegToCheck) && !_hitPegs.Contains(pegToCheck) && (pegToCheck is not LongPeg longPeg || !longPeg.hit))
                        {
                            PegGridObscurer componentInParent = pegToCheck.GetComponentInParent<PegGridObscurer>();
                            if ((!(componentInParent != null) || componentInParent.isRevealed) && pegToCheck != peg && !pegToCheck.IsDisabled() && (!pegToCheck.IsDelayedDeath() || !pegToCheck.IsWaitingForDeath()))
                            {
                                float distance = Vector2.Distance(pegToCheck.GetCenterOfPeg(), base.transform.position);
                                if (distance < closestDistance)
                                {
                                    pegToZap = pegToCheck;
                                    closestDistance = distance;
                                }
                            }
                        }
                    }
                    if (!(pegToZap != null))
                    {
                        break;
                    }
                    pegToZap.PegActivated(true);
                    list.Add(peg.GetCenterOfPeg());
                    list.Add(pegToZap.GetCenterOfPeg());
                    _hitPegs.Add(pegToZap);
                    peg = pegToZap;
                }
                if (list.Count > 0)
                {
                    _line.enabled = true;
                    _line.positionCount = list.Count;
                    _line.SetPositions(list.ToArray());
                    _line.material.DOKill(false);
                    TweenerCore<Color, Color, ColorOptions> tweenerCore = _line.material.DOFade(0f, ZapFadeTime).From(1f, true, false);
                    tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(this.DisableRenderer));
                }
            }
            _hitPegs.Clear();
        }

        public void DisableRenderer()
        {
            _line.enabled = false;
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_pachinkoBall == null || _pachinkoBall.IsDummy)
            {
                return;
            }
            Peg peg = other.collider.GetComponent<Peg>();

            if (peg is LongPeg longPeg && longPeg.hit)
            {
                return;
            }

            if (CustomRelicManager.Instance.AttemptUseRelic(RelicNames.PLASMA_BALL))
            {
                ActivateEffect(peg);
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
