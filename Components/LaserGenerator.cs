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
        public static List<LaserGenerator> ActiveLazers = new List<LaserGenerator>();
        public static List<LaserGenerator> AllLazers = new List<LaserGenerator>();
        public static GameObject LazerPrefab;

        public PachinkoBall Pachinko;
        public bool ShouldLazer = true;

        public int HitsForLazer = 5;
        public float LaserDuration = 1.5f;
        public int _currentHits = 0;

        public void Awake()
        {
            if (LazerPrefab == null) CreatePrefab();
            Pachinko = GetComponent<PachinkoBall>();
        }

        private void CreatePrefab()
        {
            LazerPrefab = new GameObject("Lazer");
            LazerPrefab.AddComponent<Laser>();
            LazerPrefab.transform.SetParent(Plugin.PromethiumPrefabHolder.transform);
        }

        private void CreateLazer(Vector3 end)
        {
            GameObject obj = GameObject.Instantiate(LazerPrefab);
            Laser lazer = obj.GetComponent<Laser>();
            if(lazer != null)
            {
                lazer.Initialize(transform.position, end, LaserDuration);
            }
        }


        public void OnEnable()
        {
            ActiveLazers.Add(this);
        }

        public void OnDisable()
        {
            ActiveLazers.Remove(this);
        }

        public void OnDestroy()
        {
            ActiveLazers.Remove(this);
            AllLazers.Remove(this);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (Pachinko.IsDummy || Pachinko.CurrentState != PachinkoBall.FireballState.FIRING) return;
            if (collision.collider.CompareTag("Peg") || collision.collider.CompareTag("Bomb"))
            {
                _currentHits++;
                if(_currentHits == HitsForLazer)
                {
                    _currentHits = 0;
                    foreach (LaserGenerator generator in ActiveLazers)
                    {
                        if (generator.gameObject != gameObject && generator.gameObject.activeInHierarchy && generator.Pachinko.CurrentState == PachinkoBall.FireballState.FIRING && generator.ShouldLazer && this.ShouldLazer)
                        {
                            CreateLazer(generator.gameObject.transform.position);
                        }
                    }
                }

            }
        }
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Destroyer>() != null)
            {
                ShouldLazer = false;
            }
        }
    }
}
