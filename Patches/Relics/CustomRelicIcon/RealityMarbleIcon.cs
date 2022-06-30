using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Relics.CustomRelicIcon
{
    public class RealityMarbleIcon : RelicIcon
    {
        private float _time = 0;
        private int _frame = 0;
        private int _framesPerSecond = 8;
        private Sprite[] spriteSheet = Plugin.RealityMarble;

        public new void Update()
        {
            base.Update();
            // As a relic that affects space, it too should ignore time. (And prevent it from being too flashy)
            _time += Time.deltaTime / Time.timeScale; 

            if(_time >= 1f / _framesPerSecond)
            {
                _time -= 1f / _framesPerSecond;

                _frame++;
                if (_frame == spriteSheet.Length)
                    _frame = 0;
                _image.sprite = spriteSheet[_frame];
            }
        }
    }
}
