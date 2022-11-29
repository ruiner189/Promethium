using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Components.Animations
{
    [RequireComponent(typeof(LaserGenerator))]
    public class LasorbAnimation : MonoBehaviour
    {
        private LaserGenerator _laser;
        private SpriteRenderer _renderer;

        public void Awake()
        {
            _laser = gameObject.GetComponent<LaserGenerator>();
            _renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        }

        private float GetHitPercent()
        {
            return _laser._currentHits / (float) _laser.HitsForLazer;
        }


        public void Update()
        {
            int selection = (int) Math.Floor(Plugin.Lasorb.Length * GetHitPercent());
            Sprite sprite = Plugin.Lasorb[selection];
            if (_renderer.sprite != sprite)
                _renderer.sprite = sprite;
        }
    }
}
