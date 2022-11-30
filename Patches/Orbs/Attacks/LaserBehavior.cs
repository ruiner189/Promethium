using Battle.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs.Attacks
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LaserBehavior : MonoBehaviour
    {
        public delegate void LaserHit();

        public LaserHit OnLaserHit;
        public LaserHit OnLaserEnd;

        private int _frame = 0;
        private bool _ending = false;

        private int _setupFrames = 4;
        private int[] _damageFrames = {9,11,13};
        private float _fps = 1 / 10f;
        private Sprite[] _sprites;

        private float _timeElapsed = 0;

        private SpriteRenderer _renderer;

        public void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _sprites = Plugin.LasorbAttack;
        }

        public void OnEnable()
        {
            _frame = 0;
            _renderer.sprite = _sprites[_frame];
            _ending = false;
        }

        public void Update()
        {
            _timeElapsed += Time.deltaTime;

            if (_timeElapsed > _fps)
            {
                _timeElapsed = 0;
                if (!_ending)
                {
                    _frame++;
                    if (_frame == _sprites.Length)
                    {
                        _ending = true;
                        _frame = _setupFrames - 1;
                    }
                } else
                {
                    _frame--;
                    if(_frame <= -1)
                    {
                        OnLaserEnd?.Invoke();
                        gameObject.SetActive(false);
                        return;
                    }
                }

                if (_damageFrames.Contains(_frame))
                {
                    OnLaserHit?.Invoke();
                }

                _renderer.sprite = _sprites[_frame];
            }


        }
    }
}
