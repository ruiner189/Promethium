using Battle.StatusEffects;
using HarmonyLib;
using I2.Loc;
using Relics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public bool RefreshPegHitThisTurn => (bool)(typeof(PachinkoBall).GetField("_refreshPegHitThisTurn", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_pachinko));
		public int State => (int)(typeof(PachinkoBall).GetField("_state", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_pachinko));

		public void OnValidate()
		{
			_localParams = gameObject.GetComponent<LocalizationParamsManager>();

			if(_localParams != null)
            {
				_localParams.SetParameterValue(ParamKeys.FRAGILE_FRAGMENTS, $"{MaxSplits - 1}", true);
				_localParams.SetParameterValue(ParamKeys.FRAGILE_HITS, $"{HitToSplitCount}", true);
            }
		}

		public void Init()
        {
			_relicManager = typeof(PachinkoBall).GetField("_relicManager", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_pachinko) as RelicManager;
			_predictionManager = typeof(PachinkoBall).GetField("_predictionManager", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_pachinko) as PredictionManager;
			_playerStatusEffectController = typeof(PachinkoBall).GetField("_playerStatusEffectController", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_pachinko) as PlayerStatusEffectController;
		}
		public void OnCollisionEnter2D(Collision2D collision)
        {
			if (_pachinko == null)
			{
				_pachinko = GetComponent<PachinkoBall>();
				if (_pachinko != null && !_pachinko.IsDummy)
					Init();
				else
					return;
			}
			if (_pachinko.IsDummy || State != 2 || !Duplicate) return;
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

					Peg component = collision.collider.GetComponent<Peg>();
					Bomb component2 = collision.collider.GetComponent<Bomb>();
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
					if (component is RegularPeg && !this.RefreshPegHitThisTurn && this._relicManager != null && this._relicManager.RelicEffectActive(RelicEffect.BOMB_FORCE_ALWAYS))
					{
						RegularPeg regularPeg = component as RegularPeg;
						num += regularPeg.bombRelicFlingForce;
					}
					else if (component2 != null && component2.ShouldExplode())
					{
						num += collision.collider.GetComponent<Bomb>().bombFlingForce;
					}

					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(base.gameObject, vector2, base.transform.rotation);
					PachinkoBall component3 = gameObject.GetComponent<PachinkoBall>();
					component3.Init(this._relicManager, _pachinko.aimVector, this._predictionManager, this._playerStatusEffectController);
					component3.SetFiring();
					gameObject.GetComponent<Rigidbody2D>().AddForce(vector * num * _pachinko.MultiballForceMod);
					IceOrbPachinko component4 = gameObject.GetComponent<IceOrbPachinko>();
					if (component4 != null)
					{
						component4.enabled = false;
						component4.GetComponentInChildren<SpriteRenderer>().sprite = _pachinko.sprite;
					}

					gameObject.transform.localScale *= Reduction;
					

					if (!Original)
					{
						this.gameObject.transform.localScale *= Reduction;
						this.gameObject.GetComponent<Rigidbody2D>().mass *= Reduction;
					}

					Fragile component5 = gameObject.GetComponent<Fragile>();
					if(component5 != null)
                    {
						component5.Original = false;
						component5.HitToSplitCount = 2;
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
				if(fragile != null)
                {
					fragile.Duplicate = false;
                }
            }
        }

	}
}
