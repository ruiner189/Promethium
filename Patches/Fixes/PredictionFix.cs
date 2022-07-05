using Battle.Pachinko;
using Battle.Pachinko.BallBehaviours;
using Battle.PegBehaviour;
using HarmonyLib;
using Relics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Promethium.Patches.Fixes
{
    [HarmonyPatch(typeof(PachinkoBall), nameof(PachinkoBall.DoUpdate))]
    public static class PredictionFix
    {
        public class PegState
        {
            public Peg Peg;
            public int NumOfBounces;
            public Peg.PegType Type;
            public bool IsDead;
            public bool FakeDestroy = false;

            public PegState(Peg peg, int numOfBounces, Peg.PegType type)
            {
                Peg = peg;
                NumOfBounces = numOfBounces;
                Type = type;
                IsDead = IsPegDead(peg);
            }
        }

        private static readonly List<PegState> States = new List<PegState>();

        public static bool IsPegDead(Peg peg)
        {
            if (peg is Bomb bomb && bomb._detonated) return true;
            if (peg._collider != null && !peg._collider.enabled) return true;
            if (peg._trigger != null && !peg._trigger.enabled) return true;
            return false;
        }

        [HarmonyPatch(typeof(PredictionManager), nameof(PredictionManager.CopyChildren))]
        public static class CopyPegStates
        {
            public static void Postfix(PredictionManager __instance)
            {
                if (!Plugin.UseCustomPrediction) return;
                States.Clear();
                foreach (GameObject obj in __instance._dummyPegs)
                {
                    foreach (Peg peg in obj.GetComponentsInChildren<Peg>(true))
                    {
                        if(peg.pegType == Peg.PegType.BOMB && peg is Bomb bomb)
                        {
                            States.Add(new PegState(bomb, bomb.HitCount, bomb.pegType));
                        }
                        else
                        {
                            States.Add(new PegState(peg, peg._numBounces, peg.pegType));
                        }
                    }

                    foreach(FlickeringPeg flickeringPeg in obj.GetComponentsInChildren<FlickeringPeg>(true))
                    {
                        flickeringPeg.SetDummyStatus(true);
                    }
                }
            }
        }

        public static void SoftResetPegs(PredictionManager __instance)
        {
            foreach (PegState state in States)
            {
                if (state.Peg != null)
                {
                    if(BattleController._battleState != BattleController.BattleState.NAVIGATION)
                        state.Peg.gameObject.SetActive(true);
                    state.Peg._numBounces = state.NumOfBounces;

                    if(state.Peg._collider != null)
                        state.Peg._collider.enabled = !state.IsDead;

                    if(state.Peg._trigger != null)
                        state.Peg._trigger.enabled = !state.IsDead;

                    if (state.Peg._poppedPegCollider != null)
                        state.Peg._poppedPegCollider.enabled = state.IsDead;

                    if (state.Peg._specialPegCollider != null)
                        state.Peg._specialPegCollider.enabled = state.IsDead;

                    state.Peg.pegType = state.Type;

                    if (state.FakeDestroy)
                    {
                        state.Peg.gameObject.SetActive(true);
                        state.FakeDestroy = false;
                    }

                    if(state.Peg is LongPeg longPeg)
                    {
                        longPeg._hit = state.IsDead;
                    }

                    if(state.Peg is Bomb bombPeg)
                    {
                        bombPeg.HitCount = state.NumOfBounces;
                        bombPeg._detonated = state.IsDead;
                    }
                }
            }

        }

        [HarmonyPatch(typeof(LongPeg), nameof(LongPeg.OnEnable))]
        public static class FixLongPegType
        {
            public static void Prefix(LongPeg __instance, out Peg.PegType __state)
            {
                __state = __instance.pegType;
            }

            public static void Postfix(LongPeg __instance, ref Peg.PegType __state)
            {
                if(__state != Peg.PegType.NONE)
                {
                    __instance.pegType = __state;
                } else
                {
                    __instance.pegType = Peg.PegType.REGULAR;
                }
            }
        }

        [HarmonyPatch(typeof(PachinkoBall), nameof(PachinkoBall.DoMagnetAttraction))]
        public static class FixMagnetAttraction
        {
            public static bool Prefix(PachinkoBall __instance, PhysicsScene2D physicsScene)
            {
                if (!Plugin.UseCustomPrediction) return true;

                if ((__instance._state == PachinkoBall.FireballState.FIRING || __instance.IsDummy) && __instance._relicManager != null && __instance._relicManager.RelicEffectActive(RelicEffect.PEG_MAGNET) && __instance._shotTime < 12f)
                {
                    float t = (__instance._shotTime <= 4f) ? 0f : ((__instance._shotTime - 4f) / 8f);

                    physicsScene.OverlapCircle(__instance.transform.position, 3f, __instance._nearbyPegs, __instance._circleCastLayerMask);

                    HashSet<Collider2D> set = new HashSet<Collider2D>();
                    foreach (Collider2D collider2D in __instance._nearbyPegs)
                    {
                        if (collider2D != null)
                            set.Add(collider2D);
                    }

                    foreach (Collider2D collider2D in set)
                    {  
                        if (collider2D != null)
                        {                   
                            Peg peg = collider2D.GetComponent<Peg>();

                            Vector2 position = new Vector2(peg.transform.position.x, peg.transform.position.y);
                            float strength = 0;
                            if (peg is LongPeg && !IsPegDead(peg))
                            {
                                position = new Vector2(peg.GetCenterOfPeg().x, peg.GetCenterOfPeg().y);
                                strength = (peg.pegType == Peg.PegType.RESET || peg.pegType == Peg.PegType.CRIT) ? 6.25f : 0.9f;
                            } 
                            else if (peg is Bomb)
                            {
                                strength = 1.1f;
                            } 
                            else if (peg is RegularPeg && peg.pegType != Peg.PegType.DULL)
                            {
                                strength = (peg.pegType == Peg.PegType.RESET || peg.pegType == Peg.PegType.CRIT) ? 6.25f : 1.1f;
                            }
                            double distance = Vector2.Distance(__instance.transform.position, position);
                            if (distance > 2.5f) continue;

                            Vector2 pachinkoPosition = new Vector2(__instance.transform.position.x, __instance.transform.position.y);
                            Vector2 direction = position - pachinkoPosition;
                            float magnitude = direction.magnitude / 2.5f;

                            if(distance > 0.05 && strength != 0)
                            {
                                strength = Mathf.Lerp(strength, 0f, t);
                                Vector2 force = direction.normalized * (strength / (magnitude * magnitude));
                                __instance._rigid.AddForce(force);
                            }
                        }
                    }
                }
                return false;
            }

            [HarmonyPatch(typeof(PredictionManager), nameof(PredictionManager.Predict))]
            public static class HijackPrediction
            {
                public static bool Prefix(PredictionManager __instance, GameObject subject, Vector3 currentPosition, Vector3 force)
                {
                    if (!Plugin.UseCustomPrediction) return true;

                    if (__instance._currentPhysicsScene.IsValid() && __instance._predictionPhysicsScene.IsValid())
                    {
                        if (__instance._dummy == null)
                        {
                            __instance._dummy = UnityEngine.Object.Instantiate<GameObject>(subject);
                            SceneManager.MoveGameObjectToScene(__instance._dummy, __instance._predictionScene);
                        }
                        foreach (GameObject gameObject in __instance._bounceIndicatorList)
                        {
                            gameObject.SetActive(false);
                        }
                        PachinkoBall component = __instance._dummy.GetComponent<PachinkoBall>();
                        component.InitializeMembers();
                        component.MaxBounceCount = __instance._bounceCount;
                        component.IsDummy = true;
                        component.gameObject.transform.position = currentPosition;
                        PachinkoBall pachinkoBall = component;
                        pachinkoBall.OnPachinkoBallDummyBounce = (PachinkoBall.PachinkoBallDummyBounce)Delegate.Combine(pachinkoBall.OnPachinkoBallDummyBounce, new PachinkoBall.PachinkoBallDummyBounce(__instance.CreateBounceIndicator));
                        component.SetRelicManager(__instance._relicManager);
                        IUpdateableBySimulation[] componentsInChildren = component.GetComponentsInChildren<IUpdateableBySimulation>();
                        Rigidbody2D component2 = __instance._dummy.GetComponent<Rigidbody2D>();

                        component2.simulated = true;
                        component2.gravityScale = component.GravityScale;

                        component2.AddForce(force);
                        __instance._predictionPhysicsScene.Simulate(Time.fixedDeltaTime);

                        __instance._lineRenderer.positionCount = 0;
                        __instance._lineRenderer.positionCount = __instance._maxIterations;

                        int num = 0;
                        while (num < __instance._maxIterations && __instance._dummy != null)
                        {
                            IUpdateableBySimulation[] array = componentsInChildren;
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i].DoUpdate(__instance._predictionPhysicsScene);
                            }

                            foreach (IUpdateableBySimulation updateableBySimulation in __instance._updateableSceneObjs)
                            {
                                updateableBySimulation.DoUpdate(__instance._predictionPhysicsScene);
                            }

                            __instance.UpdatePredictionPegs();
                            if (__instance._dummy.activeInHierarchy)
                            {
                                __instance._lineRenderer.SetPosition(num, new Vector3(__instance._dummy.transform.position.x, __instance._dummy.transform.position.y, -1f));
                            }
                            else if (num > 0)
                            {
                                Vector3 position = __instance._lineRenderer.GetPosition(num - 1);
                                __instance._lineRenderer.SetPosition(num, position);
                            }
                            __instance._predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
                            num++;
                        }
                        if (__instance._dummy != null)
                        {
                            UnityEngine.Object.Destroy(__instance._dummy);
                        }
                        SoftResetPegs(__instance);
                    }
                    return false;
                }
            }

            [HarmonyPatch(typeof(RegularPeg), nameof(RegularPeg.DoPegCollision))]
            public static class FixRegularPeg
            {
                public static bool Prefix(RegularPeg __instance, PachinkoBall pachinko, Peg.CollisionType cType)
                {

                    if (!Plugin.UseCustomPrediction || pachinko == null || !pachinko.IsDummy )
                    {
                        return true;
                    }

                    __instance.SetBlockPopping(pachinko);
                    Multihit component2 = pachinko.gameObject.GetComponent<Multihit>();
                    if (component2 != null && component2.multihitLevel > 0)
                    {
                        __instance._numBounces += component2.multihitLevel;
                    }
                    if (pachinko.GetComponent<PierceBehavior>() && cType == Peg.CollisionType.TRIGGER)
                    {
                        __instance._numBounces = __instance.BouncesToPop;
                    }
                    if (pachinko.GetComponent<DestroyPegsOnHit>() != null)
                    {
                        PegState state = States.Find(state => state.Peg == __instance);
                        if(state != null)
                        {
                            __instance.gameObject.SetActive(false);
                            state.FakeDestroy = true;
                        }
                        return false;
                    }
                    __instance._numBounces++;
                    if (__instance.ShouldDestroyPegOnHit())
                    {
                        __instance._collider.enabled = false;
                        __instance._trigger.enabled = false;
                       

                        __instance._poppedPegTrigger.enabled = true;
                        __instance._poppedPegCollider.enabled = true;
                        
                        __instance.pegType = Peg.PegType.REGULAR;
                    }
                    return false;
                }
            }

            [HarmonyPatch(typeof(LongPeg), nameof(LongPeg.DoPegCollision))]
            public static class FixLongPeg
            {
                public static bool Prefix(LongPeg __instance, PachinkoBall pachinko)
                {
                    if (!Plugin.UseCustomPrediction || pachinko == null || !pachinko.IsDummy)
                    {
                        return true;
                    }
                    __instance.pegType = Peg.PegType.REGULAR;
                    __instance._hit = true;
                    return false;
                }
            }

            [HarmonyPatch(typeof(Bomb), nameof(Bomb.DoPegCollision))]
            public static class FixBombPeg
            {
                public static bool Prefix(Bomb __instance, PachinkoBall pachinko)
                {
                    if(!Plugin.UseCustomPrediction || pachinko == null || !pachinko.IsDummy)
                    {
                        return true;
                    }

                    if (__instance.HitCount == 0 && (pachinko.gameObject.GetComponent<DetonateBomb>() || pachinko.gameObject.GetComponent<Multihit>()))
                    {
                        __instance.HitCount = 1;
                    }
                    __instance.HitCount++;
                    if (__instance.ShouldExplode() && !__instance._detonated)
                    {
                        __instance._detonated = true;
                        __instance._collider.enabled = false;
                        Vector2 vector = pachinko.transform.position - __instance.transform.position;
                        pachinko.gameObject.GetComponent<Rigidbody2D>().AddForce(vector.normalized * __instance.bombFlingForce);
                    }

                    return false;
                }
            }

            [HarmonyPatch(typeof(SlimeOnlyPeg), nameof(SlimeOnlyPeg.DoPegCollision))]
            public static class FixSlimePeg
            {
                public static bool Prefix(SlimeOnlyPeg __instance, PachinkoBall pachinko)
                {
                    if (!Plugin.UseCustomPrediction || pachinko == null || !pachinko.IsDummy)
                    {
                        return true;
                    }
                    PierceBehavior component = pachinko.GetComponent<PierceBehavior>();
                    if (component == null || !component.CanPierce)
                    {
                        pachinko.GetComponent<Rigidbody2D>().velocity *= __instance._pachinkoBallVelocityReduction;
                    }

                    __instance._cleared = true;
                    __instance._collider.enabled = false;
                    __instance._trigger.enabled = false;

                    return false;
                }
            }
        }
    }
}
