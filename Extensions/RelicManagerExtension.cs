using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Extensions
{
    public static class RelicManagerExtension
    {
        public static bool RelicEffectActive(this RelicManager relicManager, CustomRelicEffect effect){
            return relicManager.RelicEffectActive((RelicEffect)effect);
        }
    }
}
