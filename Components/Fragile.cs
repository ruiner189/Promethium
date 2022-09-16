using Battle.StatusEffects;
using HarmonyLib;
using I2.Loc;
using Relics;
using System.Collections;
using UnityEngine;

namespace Promethium.Components
{
    [RequireComponent(typeof(PachinkoBall))]
    public class Fragile : MonoBehaviour
    {
        public int HitCount = 0;
        public int HitToSplitCount = 3;
        public int MaxSplits = 1;

        public float Reduction = 0.8f;

        public bool Original = true;

        private PachinkoBall _pachinko;
        private RelicManager _relicManager;
        private PredictionManager _predictionManager;
        private PlayerStatusEffectController _playerStatusEffectController;
        private LocalizationParamsManager _localParams;

        public bool Duplicate = true;

        private void Awake()
        {
            _pachinko = GetComponent<PachinkoBall>();
        }

        private void Start()
        {
            _relicManager = _pachinko._relicManager;
            _predictionManager = _pachinko._predictionManager;
            _playerStatusEffectController = _pachinko._playerStatusEffectController;
        }

        public void OnValidate()
        {
            _localParams = gameObject.GetComponent<LocalizationParamsManager>();
            if (_localParams == null) gameObject.AddComponent<LocalizationParamsManager>();

            if (_localParams != null)
            {
                _localParams.SetParameterValue(ParamKeys.FRAGILE_FRAGMENTS, $"{MaxSplits - 1}", true);
                _localParams.SetParameterValue(ParamKeys.FRAGILE_HITS, $"{HitToSplitCount}", true);
            }
        }


        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (_pachinko.IsDummy || _pachinko.CurrentState != PachinkoBall.FireballState.FIRING || !Duplicate) return;
            if (collision.collider.CompareTag("Peg") || collision.collider.CompareTag("Bomb"))
            {
                HitCount++;
                if (HitCount == HitToSplitCount)
                {
                    HitCount = 0;
                    if (!Original)
                    {
                        MaxSplits--;
                        if (MaxSplits == 0)
                        {
                            Duplicate = false;
                            StartCoroutine(DelayedDestroy());
                            return;
                        }
                    }

                    Peg peg = collision.collider.GetComponent<Peg>();
                    Bomb bomb = collision.collider.GetComponent<Bomb>();
                    Vector2 vector = Vector2.Perpendicular(base.transform.right).normalized;
                    if (vector.y < 0f)
                    {
                        vector *= -1f;
                    }
                    Vector2 vector2 = collision.contacts[0].point + vector * 0.58f;
                    if (vector2.x > StaticGameData.battleXBounds.y)
                    {
                        vector2.x = 7.75f;
                    }
                    else if (vector2.x < StaticGameData.battleXBounds.x)
                    {
                        vector2.x = -7.75f;
                    }
                    if (vector2.y > 4.5f)
                    {
                        vector2.y = 4f;
                    }
                    float num = 0f;
                    foreach (ContactPoint2D contactPoint2D in collision.contacts)
                    {
                        num += contactPoint2D.normalImpulse;
                    }
                    num /= Time.fixedDeltaTime;
                    if (peg is RegularPeg && !_pachinko._refreshPegHitThisTurn && this._relicManager != null && this._relicManager.RelicEffectActive(RelicEffect.BOMB_FORCE_ALWAYS))
                    {
                        RegularPeg regularPeg = peg as RegularPeg;
                        num += regularPeg.bombRelicFlingForce;
                    }
                    else if (bomb != null && bomb.ShouldExplode())
                    {
                        num += collision.collider.GetComponent<Bomb>().bombFlingForce;
                    }

                    GameObject gameObject = Instantiate<GameObject>(base.gameObject, vector2, base.transform.rotation);
                    PachinkoBall pachinko = gameObject.GetComponent<PachinkoBall>();
                    pachinko.Init(_relicManager, _pachinko.aimVector, _predictionManager, _playerStatusEffectController);
                    pachinko.SetFiring();
                    gameObject.GetComponent<Rigidbody2D>().AddForce(vector * num * _pachinko.MultiballForceMod);
                    IceOrbPachinko iceOrb = gameObject.GetComponent<IceOrbPachinko>();
                    if (iceOrb != null)
                    {
                        iceOrb.enabled = false;
                        iceOrb.GetComponentInChildren<SpriteRenderer>().sprite = _pachinko.sprite;
                    }

                    gameObject.transform.localScale *= Reduction;


                    if (!Original)
                    {
                        this.gameObject.transform.localScale *= Reduction;
                        this.gameObject.GetComponent<Rigidbody2D>().mass *= Reduction;
                    }

                    Fragile fragile = gameObject.GetComponent<Fragile>();
                    if (fragile != null)
                    {
                        fragile.Original = false;
                        fragile.HitToSplitCount = 2;
                    }
                    PachinkoBall.OnAdditionalPachinkoBallCreated();
                }
            }
        }

        public IEnumerator DelayedDestroy()
        {
            yield return new WaitForEndOfFrame();
            _pachinko.StartDestroy();
            GameObject.Destroy(this.gameObject);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Destroyer>() != null)
            {
                Duplicate = false;
            }
        }

        [HarmonyPatch(typeof(PachinkoBall), "StartDestroy")]
        private class NoDuplicateOnDeath
        {
            public static void Prefix(PachinkoBall __instance)
            {
                PolygonCollider2D polyCollider = __instance.GetComponent<PolygonCollider2D>();
                if (polyCollider != null) polyCollider.enabled = false;

                Fragile fragile = __instance.GetComponent<Fragile>();
                if (fragile != null) fragile.Duplicate = false;
            }
        }
    }
}
