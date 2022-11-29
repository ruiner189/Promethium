using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Components
{
    public class Laser : MonoBehaviour
    {
        public LineRenderer Line;
        private Vector3 _start;
        private Vector3 _end;
        private Vector3 _direction;
        private float _distance;

        private float _cooldown = 0.5f;
        private float _remainingCooldown = 0f;
        
        public void Awake()
        {
            CreateLineRenderer();
        }

        public void FixedUpdate()
        {
            _remainingCooldown -= Time.fixedDeltaTime;

            if(_remainingCooldown <= 0)
            {
                _remainingCooldown = _cooldown;


                RaycastHit2D[] hits = Physics2D.RaycastAll(_start, _direction, _distance);

                foreach (RaycastHit2D hit in hits)
                {

                    GameObject obj = hit.collider.gameObject;
                    /*
                    if (collisions.Contains(obj)) return;
                    collisions.Add(obj);
                    */

                    if (hit.collider.CompareTag("Peg") || hit.collider.CompareTag("Bomb"))
                    {
                        Peg peg = obj.GetComponent<Peg>();
                        if (peg != null)
                        {
                            peg.PegActivated(true);
                        }
                    }
                }
            }

            //Destroy(gameObject);
        }

        public void Initialize(Vector3 start, Vector3 end, float fadeTime)
        {
            _start = start;
            _end = end;

             _direction = (_end - _start).normalized;
             _distance = Vector2.Distance(_start, _end);

            Line.SetPositions(new Vector3[] { start, end });
            TweenerCore<Color, Color, ColorOptions> tweenerCore = Line.material.DOFade(0f, fadeTime).From(1f, true, false);
            tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(DestroyOnComplete));
        }

        public void DestroyOnComplete()
        {
            Destroy(gameObject);
        }

        public void OnDestroy()
        {
            Destroy(Line.material); // Prevent memory leaks as this does not get destroyed otherwise.
        }

        public void CreateLineRenderer()
        {
            LineRenderer line = GetComponent<LineRenderer>();

            if (line == null)
            {
                line = gameObject.AddComponent<LineRenderer>();
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                line.startColor = Color.red;
                line.endColor = Color.red;
                Material whiteDiffuseMat = new Material(Shader.Find("Sprites/Default"));
                line.material = whiteDiffuseMat;
                Line = line;
            }
        }

        public void GenerateMeshCollider()
        {
            MeshCollider collider = GetComponent<MeshCollider>();
            if (collider == null)
                collider = gameObject.AddComponent<MeshCollider>();

            Mesh mesh = new Mesh();
            Line.BakeMesh(mesh);
            collider.sharedMesh = mesh;
        }

        public void GenerateCollider()
        {
            EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
            if (collider == null)
                collider = gameObject.AddComponent<EdgeCollider2D>();
            List<Vector2> points = new List<Vector2>();
            for(int point = 0; point < Line.positionCount; point++)
            {
                points.Add(Line.GetPosition(point));
            }

            collider.points = points.ToArray();
        }

        private List<GameObject> collisions = new List<GameObject>();

        public void OnCollisionEnter2D(Collision2D collision)
        {

        }
    }
}
