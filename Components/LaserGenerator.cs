using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Components
{
    [RequireComponent(typeof(PachinkoBall))]
    public class LaserGenerator : MonoBehaviour
    {
        public static List<LaserGenerator> ActiveLasers = new List<LaserGenerator>();
        public static List<LaserGenerator> AllLasers = new List<LaserGenerator>();
        public static GameObject LaserPrefab;

        public PachinkoBall Pachinko;
        public bool ShouldLaser = true;

        public int HitsForLazer = 5;
        public float LaserDuration = 1.5f;
        public int _currentHits = 0;

        public void Awake()
        {
            if (LaserPrefab == null) CreatePrefab();
            Pachinko = GetComponent<PachinkoBall>();
        }

        private void CreatePrefab()
        {
            LaserPrefab = new GameObject("Laser");
            LaserPrefab.AddComponent<Laser>();
            LaserPrefab.transform.SetParent(Plugin.PromethiumPrefabHolder.transform);
        }

        private void CreateLaser(Vector3 end)
        {
            GameObject obj = GameObject.Instantiate(LaserPrefab);
            Laser laser = obj.GetComponent<Laser>();
            if(laser != null)
            {
                laser.Initialize(transform.position, end, LaserDuration);
            }
        }

        public void OnEnable()
        {
            ActiveLasers.Add(this);
        }

        public void OnDisable()
        {
            ActiveLasers.Remove(this);
        }

        public void OnDestroy()
        {
            ActiveLasers.Remove(this);
            AllLasers.Remove(this);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (Pachinko.IsDummy || Pachinko.CurrentState != PachinkoBall.FireballState.FIRING) return;
            if (collision.collider.CompareTag("Peg") || collision.collider.CompareTag("Bomb"))
            {
                Peg peg = collision.collider.GetComponent<Peg>();
                if (peg is LongPeg longPeg && longPeg.hit)
                {
                    return;
                }

                _currentHits++;
                if(_currentHits == HitsForLazer)
                {
                    _currentHits = 0;
                    foreach (LaserGenerator generator in ActiveLasers)
                    {
                        if (generator.gameObject != gameObject && generator.gameObject.activeInHierarchy && generator.Pachinko.CurrentState == PachinkoBall.FireballState.FIRING && generator.ShouldLaser && this.ShouldLaser)
                        {
                            CreateLaser(generator.gameObject.transform.position);
                        }
                    }
                }

            }
        }
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Destroyer>() != null)
            {
                ShouldLaser = false;
            }
        }
    }
}
